using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MacintoshBED.DTO;
using MacintoshBED.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MacintoshBED.Models;
using MacintoshBED.Services;
using MacintoshBED.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MacintoshBED.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class UsersController : Controller
    {
        private IUserService _userService;
        private IMapper _mapper;
        public IConfiguration Configuration;
        private readonly Context _context;
        private readonly IEmailService _emailService;

        public UsersController(
            Context context,
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            Configuration = configuration;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.AccessLevel ?? "null")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [Authorize(Roles = AccessLevel.Admin)]
        [HttpPost("accesslevel/{id}")]
        public IActionResult ChangeAccess(int id, UpdateAccessLevelDTO model)
        {
            // You should check if the user exists or not and then check what is their current access level. As well as you need to create an enum or make sure that user does not pass any 
            // value except the allowed values which are: NULL, Admin, Support, Student Lead
            _context.User.Find(id).AccessLevel = model.AccessLevel;
            _context.SaveChanges();
            return Ok("User Access Level has been updated!");
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (model.AccessLevel != "Employer" && model.AccessLevel!="Candidate")
            {
                return BadRequest(new { message = "You can only create a profile as an Employer or a Candidate" });
            }
            // map model to entity
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                _userService.Create(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("Update{id}")]
        public IActionResult Update(int id, UpdateModel model)
        {
            //Finding who is logged in
            int logged_in_user = int.Parse(User.Identity.Name);
            User loginuser = _context.User.ToList().Find(a=>a.Id == logged_in_user);
            // map model to entity and set id
            var user = _mapper.Map<User>(model);
            user.Id = id;

            //Rejecting access if the logged in user is not same as the user updating information
            if (loginuser.AccessLevel !="Admin"||logged_in_user != id)
            {
                return BadRequest(new { message = "Access Denied" });
            }

            try
            {
                // update user 
                _userService.Update(user, model.CurrentPassword, model.NewPassword, model.ConfirmNewPassword);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("forgotpassword")]
        public IActionResult ForgotPassword(ForgotPassword model)
        {
            return Ok(_userService.ForgotPassword(model.Username));
        }



        [Authorize(Roles = AccessLevel.Admin)]
        [HttpPost("email")]
        public async Task<IActionResult> SendEmail(SendEmailDTO model)
        {
            var emails = new List<string>();
            foreach (var item in model.emails)
            {
                emails.Add(item);
            }
            
            var response = await _emailService.SendEmailAsync("hamza.gaizi@epita.com",emails, model.Subject, model.Message);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return Ok("Email sent " + response.StatusCode);
            }
            else
            {
                return BadRequest("Email sending failed " + response.StatusCode);
            }
        }

        //TODO Delete your own account
    }
}

