using System.Collections.Generic;

namespace MacintoshBED.Models
{
    public class Employer
    {
        public int Id {get;set;}
        public int IdUser {get;set;}
        public bool Advertise {get;set;}
        public List<JobDescription> Jobs {get;set;}
    }
}