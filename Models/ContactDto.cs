using System.ComponentModel.DataAnnotations;

namespace MyStoreApi.Models
{
    public class ContactDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Messege { get; set; } = string.Empty;
    }
}
