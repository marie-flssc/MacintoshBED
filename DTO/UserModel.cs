using System;
namespace MacintoshBED.DTO
{
    public class UserModel
    {
        //FOR CANDIDATES ONLY
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Skillset {get;set;}
        public bool Available {get;set;}
        public int Rating {get;set;}
    }
}
