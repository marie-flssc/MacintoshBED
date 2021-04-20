using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MacintoshBED.Models
{
    public class User
    {

        //[Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        //password between 6 and 12, with 1 number and 1 uppercase
        [RegularExpression(@"/^(?=.*[A-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@])(?!.*[iIoO])\S{6,12}$/")]
        public string Password {get;set;}
        public string PasswordHash { get; set; }
        public string AccessLevel { get; set; }
        public string Token { get; set; }
        public int Rating {get;set;}
        //for candidates
        public string Skillset {get;set;}
        public bool Available {get;set;}
        public bool Advertise {get;set;}
        //for employers
        public List<JobDescription> Jobs {get;set;}
    }
}
