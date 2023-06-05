using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Teacher : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(15)]
        public required string Phone { get; set; }

        [Required]
        [Range(0, 60)]
        public int YearsExperience { get; set; }

        [Required]
        public bool IsOnLeave { get; set; }

        [Required]
        public required virtual Class TheirClass { get; set; }

        [Required]
        public required virtual ICollection<TeacherClass> TeachersClasses { get; set; }
    }
}
