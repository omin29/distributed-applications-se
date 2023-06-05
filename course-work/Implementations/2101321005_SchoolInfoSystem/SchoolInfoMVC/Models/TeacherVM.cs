using System.ComponentModel.DataAnnotations;

namespace SchoolInfoMVC.Models
{
    public class TeacherVM:BaseVM
    {
        [Required(ErrorMessage = "First name is required field!")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters!")]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required field!")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters!")]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = null!;

        public string GetFullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        [Required(ErrorMessage = "Birth date is required field!")]
        [Display(Name = "Birth date")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Phone number is required field!")]
        [StringLength(15, ErrorMessage = "The phone number cannot be longer than 15 digits")]
        [Display(Name = "Phone number")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Years of experience is required field!")]
        [Range(0, 60, ErrorMessage = "The years of experience can be between 0 and 60!")]
        [Display(Name = "Years of experience")]
        public int YearsExperience { get; set; }

        [Display(Name = "Is on leave")]
        public bool IsOnLeave { get; set; }

        public ICollection<TeacherClassVM>? TaughtClasses { get; set; } = null;
    }
}
