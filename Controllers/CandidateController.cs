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


        [Authorize(Roles = AccessLevel.Candidate)]
        [HttpPut("Advertise/{Advertise}")]
        public IActionResult Avertise(bool Advertise)
        {
            int id = int.Parse(User.Identity.Name);
            var user = _context.User.ToList().Find(x => x.Id == id);
            user.Advertise = Advertise;
            _context.User.Update(user);
            _context.SaveChanges();
            return Ok();
        }
    }
}