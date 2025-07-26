using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RegistrationAPI.Models
{
    public class RegistrationModel
    {
        public int Id { get; set; }

        [Required]
        public string FName { get; set; }

        [Required]
        public string LName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string MobileNo { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImagePath { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class CandidateDetailsModel
    {
        public int? Details_ID { get; set; }
        public int Candt_ID { get; set; }
        
        [Required]
        public string? Aadhar_No { get; set; }

        [Required]
        public string? Pan_No { get; set; }

        [Required]
        public string? Gender { get; set; }

        [Required]
        public string? Highest_Qualification { get; set; }

        [Required]
        public string? Company_Name { get; set; }

        [Required]
        public string? Dept { get; set; }
        
        [Required]
        public string? Post { get; set; }

        [Required]
        public string? Mode { get; set; }
    }

    public class CandidateFullDetailsModel
    {
        public RegistrationModel Registration { get; set; }
        public CandidateDetailsModel? CandidateDetails { get; set; }
    }
}
