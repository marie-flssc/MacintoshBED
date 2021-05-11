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
using Microsoft.EntityFrameworkCore;

namespace MacintoshBED.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AdminController: Controller
    {
        private IUserService _userService;
        private IMapper _mapper;
        public IConfiguration Configuration;
        private readonly Context _context;

        public AdminController(Context context, IUserService userService,IMapper mapper,IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            Configuration = configuration;
        }

        [Authorize(Roles = AccessLevel.Admin)]
        [HttpPost("Ban")]
        public async Task<ActionResult> BanUser(int userId)
        {
            if (_userService.ValidId(userId))
            {
                User user = _userService.GetById(userId);
                user.Banned = true;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully banned user " + userId });
            }
            return BadRequest(new { message = "The user id is invalid !" });
        }

        [Authorize(Roles = AccessLevel.Admin)]
        [HttpDelete("HardDelete")]
        public async Task<ActionResult<User>> HardDelete(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                return user;
            }
        }


        [Authorize(Roles = AccessLevel.Admin)]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        [Authorize(Roles = AccessLevel.Admin)]
        [HttpGet("GetUserById{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }
    }
}