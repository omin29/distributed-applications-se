using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class StudentDTO:BaseDTO,IValidate
    {
        #region Properties
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

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

        public ClassDTO? Class { get; set; }
        #endregion

        #region Constructors
        public StudentDTO() { }

        public StudentDTO(SaveStudentDTO saveStudentDTO)
        {
            Id = saveStudentDTO.Id;
            FirstName = saveStudentDTO.FirstName;
            LastName = saveStudentDTO.LastName;
            BirthDate = saveStudentDTO.BirthDate;
            ExcusedAbsences = saveStudentDTO.ExcusedAbsences;
            UnexcusedAbsences = saveStudentDTO.UnexcusedAbsences;
            IsActive = saveStudentDTO.IsActive;
            ClassId = saveStudentDTO.ClassId;
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
