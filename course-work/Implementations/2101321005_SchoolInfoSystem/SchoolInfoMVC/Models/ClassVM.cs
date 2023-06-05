using ApplicationService.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SchoolInfoMVC.Models
{
    public class ClassVM : BaseVM
    {
        [Required(ErrorMessage = "The grade is required field!")]
        [Range(1, 12, ErrorMessage = "The grade can be from 1st to 12th!")]
        public int Grade { get; set; }

        public string? Name { get; set; }

        [Required(ErrorMessage = "The capacity is required field!")]
        [Range(20, 30, ErrorMessage = "The capacity can be between 20 and 30 students!")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "The average score is required field!")]
        [Range(2.0, 6.0, ErrorMessage = "The average score can be between 2.00 and 6.00!")]
        [Display(Name = "Average score")]
        public double AverageScore { get; set; }

        [Display(Name = "Educational stage")]
        public string EducationalStage
        {
            get
            {
                if (Grade >= 1 && Grade <= 4)
                {
                    return "Elementary";
                }
                else if (Grade >= 5 && Grade <= 8)
                {
                    return "Secondary";
                }
                else if (Grade >= 9 && Grade <= 12)
                {
                    return "High";
                }

                return "Unknown";
            }
        }

        [StringLength(50, ErrorMessage = "The class specialization cannot be longer than 50 characters!")]
        public string? Specialization { get; set; }

        [Required(ErrorMessage = "Form teacher is required field!")]
        [Display(Name = "Form teacher")]
        public int FormTeacherId { get; set; }

        [Display(Name = "Form teacher")]
        public TeacherDTO? FormTeacher { get; set; }

        public string GetFormTeacherName
        {
            get
            {
                return $"{FormTeacher?.FirstName} {FormTeacher?.LastName}";
            }
        }
    }
}
