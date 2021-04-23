using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MacintoshBED.Data;
using MacintoshBED.DTO;
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

    }
}