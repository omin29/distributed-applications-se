using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class TeacherClass : BaseEntity
    {
        [Required]
        public int TeacherId { get; set; }

        [Required]
        [ForeignKey(nameof(TeacherId))]
        public required virtual Teacher Teacher { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        [ForeignKey(nameof(ClassId))]
        public required virtual Class Class { get; set; }
    }
}
