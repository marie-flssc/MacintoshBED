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

namespace MacintoshBED.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CandidateController: Controller
    {
        private IUserService _userService;
        private IMapper _mapper;
        public IConfiguration Configuration;
        private readonly Context _context;

        public CandidateController(Context context, IUserService userService,IMapper mapper,IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            Configuration = configuration;
        }


        [Authorize(Roles = "Candidate, Admin")]
        [HttpPut("Advertise/{id}")]
        public IActionResult Avertise(bool Advertise)
        {
            int id = int.Parse(User.Identity.Name);
            var user = _context.User.ToList().Find(x => x.Id == id);
            user.Advertise = Advertise;
            _context.User.Update(user);
            _context.SaveChanges();
            return Ok();
        }

        [Authorize(Roles = "Candidate, Admin")]
        [HttpGet("SeeAllEmployers")]
        public IActionResult GetAllEmployers()
        {
            var users = _context.User.ToList().Where(x => x.AccessLevel == "Employer").OrderByDescending(x => x.Advertise);
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        //For the employer to see all the candidates by id

        [Authorize(Roles = "Candidate, Admin")]
        [HttpGet("SeeAnEmployer/{id}")]
        public IActionResult GetEmployerById(int id)
        {
            var user = _userService.GetById(id);
            if (user.AccessLevel != "Employer" || user == null)
            {
                return Ok("Your request has been denied. This is either because the id you have entered is invalid, or is not a candidate's id.");
            }
            var model = new UserModel
            {
                Role = user.AccessLevel,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Skillset = user.Skillset,
                Available = user.Available,
                Rating = user.Rating,
            };


            return Ok(model);
        }
        
        [Authorize(Roles = AccessLevel.Candidate)]
        [HttpGet("SeeEmployersJobs/{id}")]
        public IActionResult GetJobsByEmployerId(int id)
        {
            var user = _userService.GetById(id);
            if (user.AccessLevel != "Employer" || user == null)
            {
                return Ok("Your request has been denied. This is either because the id you have entered is invalid, or is not a employer's id.");
            }

            List<JobDescription> model = _context.Jobs.ToList().FindAll(a => a.IdEmployer == id);

            return Ok(model);
        }
    }
}