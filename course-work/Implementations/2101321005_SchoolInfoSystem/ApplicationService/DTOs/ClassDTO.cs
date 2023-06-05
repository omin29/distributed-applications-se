using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class ClassDTO : BaseDTO, IValidate
    {
        #region Properties
        [Required]
        [Range(1, 12)]
        public int Grade { get; set; }

        public string? Name { get; set; }

        [Required]
        [Range(20, 30)]
        public int Capacity { get; set; }

        [Required]
        [Range(2.0, 6.0)]
        public double AverageScore { get; set; }

        public string EducationalStage
        {
            get
            {
                if(Grade >=1 && Grade <= 4)
                {
                    return "Elementary";
                }
                else if(Grade >= 5 && Grade <= 8)
                {
                    return "Secondary";
                }
                else if(Grade >= 9 && Grade <= 12)
                {
                    return "High";
                }

                return "Unknown";
            }
        }

        [StringLength(50)]
        public string? Specialization { get; set; }

        [Required]
        public int FormTeacherId { get; set; }

        public TeacherDTO? FormTeacher { get; set; }
        #endregion

        #region Constructors
        public ClassDTO() { }

        public ClassDTO(SaveClassDTO saveClassDTO)
        {
            Id = saveClassDTO.Id;
            Grade = saveClassDTO.Grade;
            Capacity = saveClassDTO.Capacity;
            AverageScore = saveClassDTO.AverageScore;
            Specialization = saveClassDTO.Specialization;
            FormTeacherId = saveClassDTO.FormTeacherId;
        }
        #endregion

        public bool Validate()
        {
            string[] validSpecializations = new string[] { "Foreign languages", "Natural sciences", "Social sciences" };

            try
            {
                Validator.ValidateObject(this, new ValidationContext(this));
            }
            catch
            {
                return false;
            }

            return (Specialization == null || validSpecializations.Contains(Specialization));
        }
    }
}
