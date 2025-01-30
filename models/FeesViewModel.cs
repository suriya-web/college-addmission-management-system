using System.ComponentModel.DataAnnotations;

namespace collegeAdmission.Models
{
    public class FeesViewModel
    {
        
            [Required]
            public string StudentName { get; set; }

            [Required]
            [Range(0, double.MaxValue, ErrorMessage = "Fee amount must be a positive value.")]
            public decimal FeeAmount { get; set; }

            [Required]
            public string PaymentMode { get; set; }

            [Required]
            public DateTime? PaymentDate { get; set; }
        }

    }

