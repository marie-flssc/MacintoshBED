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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MacintoshBED.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class EmployerController : Controller
    {
        private IUserService _userService;
        private IMapper _mapper;
        public IConfiguration Configuration;
        private readonly Context _context;

        public EmployerController(Context context, IUserService userService,IMapper mapper,IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            Configuration = configuration;
        }

        //For the employer to see all the candidates in a list

        [Authorize(Roles = "Employer, Admin")]
        [HttpGet("SeeAllCandidates")]
        public IActionResult GetAllCandidates()
        {
            var users = _context.User.ToList().Where(x => x.AccessLevel == "Candidate").OrderByDescending(x => x.Advertise);
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        //For the employer to see all the candidates by id

        [Authorize(Roles = "Employer, Admin")]
        [HttpGet("SeeACandidate/{id}")]
        public IActionResult GetCandidateById(int id)
        {
            var user = _userService.GetById(id);
            if (user.AccessLevel != "Candidate" || user == null)
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

        /*[Authorize(Roles = AccessLevel.Employer)]
        [HttpGet("See A Candidate/{name}")]
        public IActionResult GetCandidateByNames(string name)
        {
            var user = _userService.GetByName(name);
            if (user.AccessLevel != "Candidate" || user == null)
            {
                return Ok("Your request has been denied");
            }
            var model = new UserModel
            {
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
        */
        //For the employer to see a candidates profile


    }
}
