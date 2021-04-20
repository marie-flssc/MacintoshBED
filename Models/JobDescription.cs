namespace MacintoshBED.Models
{
    public class JobDescription
    {
        public int Id {get;set;}
        public string Description {get;set;}
        public bool Ended{get;set;}
        public bool Accepted{get;set;}
        public int IdEmployee {get;set;}
        public int IdEmployer {get;set;}
        public int Pay {get;set;}
        public bool Premium {get;set;}
    }
}