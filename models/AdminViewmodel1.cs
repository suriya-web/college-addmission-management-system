using System.ComponentModel.DataAnnotations;

namespace collegeAdmission.Models
{
    public class AdminViewmodel1
    {

        [Required] public string Username { get; set; }
        [Required][DataType(DataType.Password)] public string Password { get; set; }
    }
}
