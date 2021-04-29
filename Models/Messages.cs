using System.ComponentModel.DataAnnotations; 
namespace MacintoshBED.Models
{
    public class Messages
    {
        public int IdMessage { get; set; }
        public int IdFrom { get; set; }
        public int IdTo { get; set; }
        public string Message { get; set; }
                 
    }
}