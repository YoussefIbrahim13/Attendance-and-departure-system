using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos

{
    public class UserUpdateDto
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public bool IsApproved { get; set; }
    }
}
