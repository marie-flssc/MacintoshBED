using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MacintoshBED.DTO
{
    public class SendEmailDTO
    {
        [Required]
        public List<string> emails { get; set; }
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
