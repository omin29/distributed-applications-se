using ApplicationService.DTOs;
using Data.Entities;
using Repository.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class TeacherClassManagementService
    {
        public List<TeacherClassDTO> Get()
        {
            List<TeacherClassDTO> teacherClassDTOs = new List<TeacherClassDTO>();

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.TeacherClassRepository.Get())
                {
                    Class @class = unitOfWork.ClassRepository.GetByID(item.ClassId);
                    Teacher teacher = unitOfWork.TeacherRepository.GetByID(item.TeacherId);

                    teacherClassDTOs.Add(new TeacherClassDTO
                    {
                        Id = item.Id,
                        ClassId = item.ClassId,
                        TeacherId = item.TeacherId,
                        Class = new ClassDTO
                        {
                            Name = @class.Name,
                            Grade = ClassManagementService.GetGrade(@class.Name),
                            Id = @class.Id,
                            Capacity = @class.Capacity,
                            AverageScore = @class.AverageScore,
                            Specialization = @class.Specialization,
                            FormTeacherId = @class.FormTeacherId,
                            FormTeacher = null
                        },
                        Teacher = new TeacherDTO
                        {
                            Id = teacher.Id,
                            FirstName = teacher.FirstName,
                            LastName = teacher.LastName,
                            BirthDate = teacher.BirthDate,
                            Phone = teacher.Phone,
                            YearsExperience = teacher.YearsExperience,
                            IsOnLeave = teacher.IsOnLeave
                        }
                    });
                }
            }

            return teacherClassDTOs;
        }

        public TeacherClassDTO? GetById(int id)
        {
            TeacherClassDTO? teacherClassDTO = null;

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                TeacherClass teacherClass = unitOfWork.TeacherClassRepository.GetByID(id);

                if (teacherClass != null)
                {
                    Class @class = unitOfWork.ClassRepository.GetByID(teacherClass.ClassId);
                    Teacher teacher = unitOfWork.TeacherRepository.GetByID(teacherClass.TeacherId);

                    teacherClassDTO = new TeacherClassDTO
                    {
                        Id = teacherClass.Id,
                        ClassId = teacherClass.ClassId,
                        TeacherId = teacherClass.TeacherId,
                        Class = new ClassDTO
                        {
                            Name = @class.Name,
                            Grade = ClassManagementService.GetGrade(@class.Name),
                            Id = @class.Id,
                            Capacity = @class.Capacity,
                            AverageScore = @class.AverageScore,
                            Specialization = @class.Specialization,
                            FormTeacherId = @class.FormTeacherId,
                            FormTeacher = null
                        },
                        Teacher = new TeacherDTO
                        {
                            Id = teacher.Id,
                            FirstName = teacher.FirstName,
                            LastName = teacher.LastName,
                            BirthDate = teacher.BirthDate,
                            Phone = teacher.Phone,
                            YearsExperience = teacher.YearsExperience,
                            IsOnLeave = teacher.IsOnLeave
                        }
                    };
                }
            }

            return teacherClassDTO;
        }

        public List<TeacherClassDTO> GetByTeacherId(int teacherId, bool reverse = false)
        {
            List<TeacherClassDTO> teacherClassDTOs = new List<TeacherClassDTO>();

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Teacher teacher = unitOfWork.TeacherRepository.GetByID(teacherId);

                if (reverse)
                {
                    List<int> taughtClassIds = unitOfWork.TeacherClassRepository.Get(x => x.TeacherId == teacherId)
                        .Select(x => x.ClassId).ToList();

                    IEnumerable<Class> notTaughtClassesByTeacher = unitOfWork.ClassRepository
                        .Get(x=>taughtClassIds.Contains(x.Id) == false);

                    foreach (var notTaughtClass in notTaughtClassesByTeacher)
                    {
                        teacherClassDTOs.Add(new TeacherClassDTO
                        {
                            Id = 0,
                            ClassId = notTaughtClass.Id,
                            TeacherId = teacher.Id,
                            Class = new ClassDTO
                            {
                                Name = notTaughtClass.Name,
                                Grade = ClassManagementService.GetGrade(notTaughtClass.Name),
                                Id = notTaughtClass.Id,
                                Capacity = notTaughtClass.Capacity,
                                AverageScore = notTaughtClass.AverageScore,
                                Specialization = notTaughtClass.Specialization,
                                FormTeacherId = notTaughtClass.FormTeacherId,
                                FormTeacher = null
                            },
                            Teacher = new TeacherDTO
                            {
                                Id = teacher.Id,
                                FirstName = teacher.FirstName,
                                LastName = teacher.LastName,
                                BirthDate = teacher.BirthDate,
                                Phone = teacher.Phone,
                                YearsExperience = teacher.YearsExperience,
                                IsOnLeave = teacher.IsOnLeave
                            }
                        });
                    }
                }
                else
                {
                    foreach (var teacherClass in unitOfWork.TeacherClassRepository.Get(x => x.TeacherId == teacherId))
                    {
                        Class @class = unitOfWork.ClassRepository.GetByID(teacherClass.ClassId);

                        teacherClassDTOs.Add(new TeacherClassDTO
                        {
                            Id = teacherClass.Id,
                            ClassId = teacherClass.ClassId,
                            TeacherId = teacherClass.TeacherId,
                            Class = new ClassDTO
                            {
                                Name = @class.Name,
                                Grade = ClassManagementService.GetGrade(@class.Name),
                                Id = @class.Id,
                                Capacity = @class.Capacity,
                                AverageScore = @class.AverageScore,
                                Specialization = @class.Specialization,
                                FormTeacherId = @class.FormTeacherId,
                                FormTeacher = null
                            },
                            Teacher = new TeacherDTO
                            {
                                Id = teacher.Id,
                                FirstName = teacher.FirstName,
                                LastName = teacher.LastName,
                                BirthDate = teacher.BirthDate,
                                Phone = teacher.Phone,
                                YearsExperience = teacher.YearsExperience,
                                IsOnLeave = teacher.IsOnLeave
                            }
                        });
                    }
                }               
            }

            return teacherClassDTOs;
        }

        public bool Save(TeacherClassDTO teacherClassDTO)
        {
            TeacherClass teacherClass = new TeacherClass
            {
                Id = teacherClassDTO.Id,
                ClassId = teacherClassDTO.ClassId,
                TeacherId = teacherClassDTO.TeacherId,
                Class = null!,
                Teacher = null!
            };

            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    /*Avoiding creation of duplicate pairs of teacher and class because only one pair
                      is enough to indicate that a certain teacher teaches in a certain class.*/
                    if(teacherClass.Id == 0 && unitOfWork.TeacherClassRepository
                        .Get(x=>x.ClassId == teacherClass.ClassId && x.TeacherId == teacherClass.TeacherId).Any())
                    {
                        return false;
                    }

                    if(teacherClassDTO.Id == 0)
                    {
                        teacherClass.CreatedOn = DateTime.Now;
                        unitOfWork.TeacherClassRepository.Insert(teacherClass);
                    }
                    else
                    {
                        teacherClass.UpdatedOn = DateTime.Now;
                        unitOfWork.TeacherClassRepository.Update(teacherClass);
                    }

                    unitOfWork.Save();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    TeacherClass teacherClass = unitOfWork.TeacherClassRepository.GetByID(id);
                    unitOfWork.TeacherClassRepository.Delete(teacherClass);
                    unitOfWork.Save();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
