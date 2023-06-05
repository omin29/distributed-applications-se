using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class TeacherClassDTO : BaseDTO, IValidate
    {
        #region Properties
        [Required]
        public int TeacherId { get; set; }

        public TeacherDTO? Teacher { get; set; }

        [Required]
        public int ClassId { get; set; }

        public ClassDTO? Class { get; set; }
        #endregion

        #region Constructors
        public TeacherClassDTO() { }

        public TeacherClassDTO(SaveTeacherClassDTO saveTeacherClassDTO)
        {
            Id = saveTeacherClassDTO.Id;
            ClassId = saveTeacherClassDTO.ClassId;
            TeacherId = saveTeacherClassDTO.TeacherId;
        }

        #endregion

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
