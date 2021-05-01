using System.Collections.Generic;
using MacintoshBED.Models;

namespace MacintoshBED.DTO
{
    public class MessageDTO 
    {
        public string FromEmail { get; set; }
        public List<string> ToEmails { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}