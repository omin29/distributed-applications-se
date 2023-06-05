using ApplicationService.DTOs;
using Data.Entities;
using Repository.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class ClassManagementService
    {
        public static string? CreateClassName(ClassDTO classDTO)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                if (classDTO.Grade >= 1 && classDTO.Grade <= 12)
                {
                    //Gets the count of the other classes which have the same grade as current one
                    int gradeCount = unitOfWork.ClassRepository
                        .Get(x => x.Name.StartsWith(classDTO.Grade.ToString()) &&
                        x.Name.Length - 1 == classDTO.Grade.ToString().Length && /*Avoids false positive when X grade is 10-12 and class grade is 1*/
                        x.Id != classDTO.Id).Count();

                    if (gradeCount < 6)
                    {
                        string className = classDTO.Grade.ToString() + (char)('A' + gradeCount);
                        return className;
                    }
                }

                return null;
            }
        }

        public static int GetGrade(string className)
        {
            int grade = 0;
            string? gradeString = string.Join("", className.Where(x => char.IsDigit(x)).ToArray());
            int.TryParse(gradeString, out grade);

            return grade;
        }

        public List<ClassDTO> Get(int? page = null, int? itemsPerPage = null, string? className = null)
        {
            List<ClassDTO> classesDto = new List<ClassDTO>();

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Expression<Func<Class, bool>> filter =
                        (x => (!string.IsNullOrEmpty(className) ? x.Name == className : true));
                IEnumerable<Class> classes = unitOfWork.ClassRepository.Get(filter);

                if (page != null && itemsPerPage != null && page > 0 && itemsPerPage > 0)
                {
                    if (page <= GetPageCount(itemsPerPage.Value, className))
                    {
                        classes = classes.Skip((page.Value - 1) * itemsPerPage.Value).Take(itemsPerPage.Value).ToList();
                    }
                    else
                    {
                        //No result
                        return new List<ClassDTO>();
                    }
                }

                foreach (var item in classes)
                {
                    Teacher formTeacher = unitOfWork.TeacherRepository.GetByID(item.FormTeacherId);

                    classesDto.Add(new ClassDTO
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Grade = GetGrade(item.Name),
                        Capacity = item.Capacity,
                        AverageScore = item.AverageScore,
                        Specialization = item.Specialization,
                        FormTeacherId = formTeacher.Id,
                        FormTeacher = new TeacherDTO
                        {
                            FirstName = formTeacher.FirstName,
                            LastName = formTeacher.LastName,
                            BirthDate = formTeacher.BirthDate,
                            Phone = formTeacher.Phone,
                            YearsExperience = formTeacher.YearsExperience,
                            IsOnLeave = formTeacher.IsOnLeave,
                            Id = formTeacher.Id
                        }
                    });
                }
            }

            return classesDto;
        }

        public ClassDTO? GetById(int id)
        {
            ClassDTO? classDTO = null;

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Class @class = unitOfWork.ClassRepository.GetByID(id);
                if (@class != null)
                {
                    Teacher formTeacher = unitOfWork.TeacherRepository.GetByID(@class.FormTeacherId);

                    classDTO = new ClassDTO
                    {
                        Id = @class.Id,
                        Name = @class.Name,
                        Grade = GetGrade(@class.Name),
                        Capacity = @class.Capacity,
                        AverageScore = @class.AverageScore,
                        Specialization = @class.Specialization,
                        FormTeacherId = formTeacher.Id,
                        FormTeacher = new TeacherDTO
                        {
                            FirstName = formTeacher.FirstName,
                            LastName = formTeacher.LastName,
                            BirthDate = formTeacher.BirthDate,
                            Phone = formTeacher.Phone,
                            YearsExperience = formTeacher.YearsExperience,
                            IsOnLeave = formTeacher.IsOnLeave,
                            Id = formTeacher.Id
                        }
                    };
                }
            }
            return classDTO;
        }

        public bool Save(ClassDTO classDTO)
        {
            string? className = CreateClassName(classDTO);

            if(className == null)
            {
                return false;
            }

            Class @class = new Class
            {
                Id = classDTO.Id,
                Name = className,
                Capacity = classDTO.Capacity,
                AverageScore = classDTO.AverageScore,
                Specialization = classDTO.Specialization,
                EducationalStage = classDTO.EducationalStage,
                Students = null!,
                FormTeacherId = classDTO.FormTeacherId,
                FormTeacher = null!,
                TeachersClasses = null!
            };

            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    if (classDTO.Id == 0)
                    {
                        @class.CreatedOn = DateTime.Now;
                        unitOfWork.ClassRepository.Insert(@class);
                    }
                    else
                    {
                        @class.UpdatedOn = DateTime.Now;
                        unitOfWork.ClassRepository.Update(@class);
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
                    Class @class = unitOfWork.ClassRepository.GetByID(id);
                    unitOfWork.ClassRepository.Delete(@class);
                    unitOfWork.Save();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetPageCount(int itemsPerPage, string? className = null)
        {
            if (itemsPerPage <= 0)
            {
                return 1;
            }

            int pageCount = 0;

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Expression<Func<Class, bool>> filter =
                        (x => (!string.IsNullOrEmpty(className) ? x.Name == className : true));
                pageCount = (int)Math.Ceiling((double)unitOfWork.ClassRepository.Get(filter).Count() / itemsPerPage);
            }

            return pageCount;
        }
    }
}
