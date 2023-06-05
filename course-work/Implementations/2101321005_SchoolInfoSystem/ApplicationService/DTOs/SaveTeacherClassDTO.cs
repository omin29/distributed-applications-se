using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class SaveTeacherClassDTO : BaseDTO, IValidate
    {
        [Required]
        public int TeacherId { get; set; }

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
