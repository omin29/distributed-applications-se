using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Class : BaseEntity
    {
        [Required]
        [StringLength(3)]
        public required string Name { get; set; }

        [Required]
        [Range(20,30)]
        public int Capacity { get; set; }

        [Required]
        [Range(2.0, 6.0)]
        public double AverageScore { get; set; }

        [Required]
        [StringLength(50)]
        public required string EducationalStage { get; set; }

        [StringLength(50)]
        public string? Specialization { get; set; }

        [Required]
        public required virtual ICollection<Student> Students { get; set; }
        public required virtual ICollection<TeacherClass> TeachersClasses { get; set; }

        [Required]
        public int FormTeacherId { get; set; }

        [Required]
        [ForeignKey(nameof(FormTeacherId))]
        public required virtual Teacher FormTeacher { get; set; }
    }
}
