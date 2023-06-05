using ApplicationService.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SchoolInfoMVC.Models
{
    public class TeacherClassVM : BaseVM
    {
        [Required(ErrorMessage = "Teacher field is required!")]
        [Display(Name = "Teacher")]
        public int TeacherId { get; set; }

        public TeacherDTO? Teacher { get; set; }

        [Required(ErrorMessage = "Class field is required!")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        public ClassDTO? Class { get; set; }

        public string? GetClassName
        {
            get
            {
                return Class?.Name;
            }
        }
    }
}
