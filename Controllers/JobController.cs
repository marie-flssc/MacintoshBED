using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MacintoshBED.Data;
using MacintoshBED.DTO;
using MacintoshBED.Models;
using MacintoshBED.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        [Authorize(Roles = "Employer, Admin")]
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


        [Authorize(Roles= AccessLevel.Candidate)]
        [HttpPost("JobApply")]
        public IActionResult ApplyJob(JobPropositionDTO jobprop)
        {
            if(!_context.User.ToList().Find(x => x.Id == jobprop.CandidateId).Available)
            {
                return NotFound("You are not available");
            }
            if(_context.Jobs.ToList().Find(x => x.Id == jobprop.JobId).Accepted)
            {
                return NotFound("The job is already taken");
            }
            int ID = int.Parse(User.Identity.Name);
            var job = new JobApplication {
                CandidateId = jobprop.CandidateId,
                JobId = jobprop.JobId
            };
            _context.JobApplication.Add(job);
            _context.SaveChanges();
            return Ok(job);
        }

        [Authorize(Roles= AccessLevel.Employer)]
        [HttpPost("JobOffer")]
        public IActionResult ProposeJob(JobPropositionDTO jobprop)
        {
            if(!_context.User.ToList().Find(x => x.Id == jobprop.CandidateId).Available)
            {
                return NotFound("This user is not available");
            }
            int ID = int.Parse(User.Identity.Name);
            var job = new JobProposition {
                CandidateId = jobprop.CandidateId,
                JobId = jobprop.JobId
            };
            _context.JobsProposition.Add(job);
            _context.SaveChanges();
            return Ok(job);
        }


        [Authorize(Roles= "Candidate,Admin")]
        [HttpGet("SeeAllJobs")]
        public IActionResult SeeAllJobs()
        {
            var jobs = _context.Jobs;
                //.ToList().RemoveAll(x=>x.Accepted);
            //var model = _mapper.Map<IList<JobDescription>>(jobs);
            return Ok(jobs);
        }

        [Authorize(Roles= AccessLevel.Candidate)]
        [HttpGet("GetPersonnalOffers")]
        public IActionResult SeeAllOffers()
        {
            int ID = int.Parse(User.Identity.Name);
            
            var jobs = _context.JobsProposition.ToList().Where(x => x.CandidateId == ID);
            var model = _mapper.Map<IList<JobProposition>>(jobs);
            return Ok(model);
        }

        [Authorize(Roles = AccessLevel.Candidate)]
        [HttpGet("GetOfferById{id}")]
        public IActionResult GetOfferById(int id)
        {
            var job = _context.JobsProposition.ToList().Find(x => x.Id == id);
            var model = _mapper.Map<JobProposition>(job);
            return Ok(model);
        }

        [Authorize(Roles = AccessLevel.Candidate)]
        [HttpPut("AcceptOffer")]
        public IActionResult AcceptOffer(int id)
        {
            var job = _context.JobsProposition.ToList().Find(x => x.Id == id);
            var model = _mapper.Map<JobProposition>(job);
            if(_context.Jobs.ToList().Find(x => x.Id == job.JobId).Accepted)
            {
                return NotFound();
            }
            else{
                var user = _context.User.ToList().Find(x => x.Id == job.CandidateId);
                user.Available = true;
                _context.User.Update(user);
                _context.SaveChanges();
                var accept =  _context.Jobs.ToList().Find(x => x.Id == job.JobId);
                _context.Jobs.Update(accept);
                _context.SaveChanges();
            }
            return Ok();
        }

        [Authorize(Roles = AccessLevel.Employer)]
        [HttpPut("AcceptCandidate")]
        public IActionResult AcceptCandidate(int id)
        {
            var job = _context.JobApplication.ToList().Find(x => x.CandidateId == id);
            var model = _mapper.Map<JobApplication>(job);
            if(_context.Jobs.ToList().Find(x => x.Id == job.JobId).Accepted)
            {
                return NotFound();
            }
            else{
                var user = _context.User.ToList().Find(x => x.Id == job.CandidateId);
                user.Available = true;
                _context.User.Update(user);
                _context.SaveChanges();
                var accept =  _context.Jobs.ToList().Find(x => x.Id == job.JobId);
                _context.Jobs.Update(accept);
                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpPost("RateJob")]
        [Authorize(Roles = "Candidate, Employer")]
        public ActionResult RateJob(int jobOfferId, [FromQuery] int rating)
        {
            int currentId = int.Parse(User.Identity.Name);
            if (rating > 10 || rating < 0)
                return BadRequest("The rate must be between 0 and 10");
            
            var jobOffer = _context.Jobs.Find(jobOfferId);
            if (jobOffer == null)
                return NotFound();
            if(!jobOffer.Ended)
                return BadRequest("This job is not over");
            if (User.IsInRole("Candidate"))
            {
                if (!jobOffer.Accepted || !_context.Jobs.Any(a => a.IdEmployee == currentId && a.Accepted))
                    return BadRequest("You were not accepted for this job offer.");
                if(jobOffer.REmployee)
                    return BadRequest("This job employer is already rated");
                var idemployer = _context.Jobs.ToList().Find(a => a.Id == jobOffer.Id).IdEmployer;
                var employer = _context.User.ToList().Find(a => a.Id == idemployer);
                employer.Rating = employer.Rating + rating / employer.NumberJobs;
                _context.User.Update(employer);
                _context.SaveChanges();
                jobOffer.REmployee = true;
                _context.Jobs.Update(jobOffer);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                if (!jobOffer.Accepted || jobOffer.IdEmployer != currentId)
                    return BadRequest("You are not the employer for this job.");
                if(jobOffer.REmployer)
                    return BadRequest("This job employee is already rated");
                var idemployee = _context.Jobs.ToList().Find(a => a.Id == jobOffer.Id).IdEmployee;
                var employee = _context.User.ToList().Find(a => a.Id == idemployee);
                employee.Rating = employee.Rating + rating / employee.NumberJobs;
                _context.Add(employee);
                _context.SaveChanges();
                jobOffer.REmployer = true;
                _context.Jobs.Update(jobOffer);
                _context.SaveChanges();
                return Ok();
            }
            
        }

        [Authorize(Roles = AccessLevel.Employer)]
        [HttpPut("EndOffer")]
        public IActionResult EndOffer(int id)
        {
            var job = _context.Jobs.ToList().Find(x => x.Id == id);
            job.Ended = true;
            _context.Jobs.Update(job);
            _context.SaveChanges();
            var user = _context.User.ToList().Find(x => x.Id == job.IdEmployee);
            user.Available = true;
            _context.User.Update(user);
            _context.SaveChanges();
            return Ok(job);
        }

        [Authorize(Roles = AccessLevel.Employer)]
        [HttpDelete("DeleteJob")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            int iduser = getUserId();
            var job = await _context.Jobs.SingleOrDefaultAsync(x => x.Id ==id);
            if(job == null)
            {
                return NotFound();
            }
            //If the user is not the employer of the given job or if they are not an admin
            if(_context.Jobs.ToList().Find(w=>w.Id == id).IdEmployer != iduser && _context.User.ToList().Find(w=>w.Id == iduser).AccessLevel != "Admin")
            {
                return NotFound("You do not have the authorization to delete this");
            }
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            var jobap = _context.JobsProposition.ToList().FindAll(x => x.JobId ==id);
            foreach(var j in jobap)
            {
                _context.JobsProposition.Remove(j);
                await _context.SaveChangesAsync();
            }

            return Ok(job);
        }

        [Authorize(Roles = "Employer")]
        [HttpPut("AdvertiseJob/{id}")]
        public IActionResult Advertise(int id, bool adv)
        {
            var job = _context.Jobs.ToList().Find(x => x.Id == id);
            job.Premium = adv;
            _context.Jobs.Update(job);
            _context.SaveChanges();
            return Ok();
        }

        private int getUserId()
        {
            try
            {
                return int.Parse(User.Identity.Name);
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}