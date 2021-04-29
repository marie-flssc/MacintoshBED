using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MacintoshBED.Data;
using MacintoshBED.DTO;
using MacintoshBED.Models;
using MacintoshBED.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MacintoshBED.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        public IConfiguration Configuration;
        private readonly Context _context;

        public JobController(Context context, IUserService userService,IMapper mapper,IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            Configuration = configuration;
        }


        // TODO : job offer to a candidate, candidate checks employer profile , apply for a job, accept or reject a job (in that case change the user.Available bool to false)
        [Authorize(Roles= AccessLevel.Employer)]
        [HttpPost("NewJob")]
        public IActionResult NewJob(JobDTO newjob)
        {
            int ID = int.Parse(User.Identity.Name);
            var job = new JobDescription {
                Description = newjob.Description,
                Ended = false,
                Accepted = false,
                IdEmployer = ID,
                Pay = newjob.Pay
            };
            _context.Jobs.Add(job);
            _context.SaveChanges();
            return Ok(job);

        }


    }
}