using System.ComponentModel.DataAnnotations;

namespace DomainCore.Models
{
    public class User : BaseModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
    }
}
