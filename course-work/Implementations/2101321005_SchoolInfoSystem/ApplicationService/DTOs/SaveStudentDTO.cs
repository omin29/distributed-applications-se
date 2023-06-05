using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class SaveStudentDTO : BaseDTO, IValidate
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
        [Range(0, 1000)]
        public int ExcusedAbsences { get; set; }

        [Required]
        [Range(0, 1000)]
        public int UnexcusedAbsences { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int ClassId { get; set; }

        public bool Validate()
        {
            try
            {
                Validator.ValidateObject(this, new ValidationContext(this));
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
