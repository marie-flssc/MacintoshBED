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

        [Authorize(Roles= AccessLevel.Employer)]
        [HttpGet("SeeAllCandidates")]
        public IActionResult GetAllCandidates()
        {
            var users = _context.User.ToList().Where(x => x.AccessLevel == "Candidate").OrderByDescending(x => x.Advertise);
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        [Authorize(Roles= AccessLevel.Employer)]
        [HttpGet("SeeACandidate/{id}")]
        public IActionResult GetCandidateById(int id)
        {
            var user = _userService.GetById(id);
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }
    }
}
