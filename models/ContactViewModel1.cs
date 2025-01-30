using System.ComponentModel.DataAnnotations;

namespace collegeAdmission.Models
{
    public class ContactViewModel1
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
       
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Phone]
        public string Contact { get; set; }

    }
}

