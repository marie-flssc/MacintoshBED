using System.ComponentModel.DataAnnotations;

namespace MacintoshBED.Models
{
    public class ForgotPassword
    {
        [Required]
        public string Username { get; set; }
    }
}
