using ApplicationService.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SchoolInfoMVC.Models
{
    public class StudentVM : BaseVM
    {
        [Required(ErrorMessage = "First name is required field!")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters!")]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required field!")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters!")]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Birth date is required field!")]
        [Display(Name = "Birth date")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Excused absences is required field!")]
        //[Range(0, 1000, ErrorMessage = "The excused absences can be between 0 and 1000!")]
        [Display(Name = "Excused absences")]
        public int ExcusedAbsences { get; set; }

        [Required(ErrorMessage = "Unexcused absences is required field!")]
        [Range(0, 1000, ErrorMessage = "The unexcused absences can be between 0 and 1000!")]
        [Display(Name = "Unexcused absences")]
        public int UnexcusedAbsences { get; set; }

        [Display(Name = "Is active")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Class is required field!")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        public ClassDTO? Class { get; set; }
    }
}
