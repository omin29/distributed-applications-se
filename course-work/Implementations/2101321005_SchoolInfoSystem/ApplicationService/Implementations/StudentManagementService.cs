using ApplicationService.DTOs;
using Data.Entities;
using Repository.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class StudentManagementService
    {
        public List<StudentDTO> Get(int? page = null, int? itemsPerPage = null, string? firstName = null, string? lastName = null)
        {
            List<StudentDTO> studentsDto = new List<StudentDTO>();

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Expression<Func<Student, bool>> filter =
                        (x => (!string.IsNullOrEmpty(firstName) ? x.FirstName == firstName : true) &&
                              (!string.IsNullOrEmpty(lastName) ? x.LastName == lastName : true));
                IEnumerable<Student> students = unitOfWork.StudentRepository.Get(filter);

                if (page != null && itemsPerPage != null && page > 0 && itemsPerPage > 0)
                {
                    if (page <= GetPageCount(itemsPerPage.Value, firstName, lastName))
                    {
                        students = students.Skip((page.Value - 1) * itemsPerPage.Value).Take(itemsPerPage.Value).ToList();
                    }
                    else
                    {
                        //No result
                        return new List<StudentDTO>();
                    }
                }

                foreach (var item in students)
                {
                    Class @class = unitOfWork.ClassRepository.GetByID(item.ClassId);

                    studentsDto.Add(new StudentDTO
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        BirthDate = item.BirthDate,
                        ExcusedAbsences = item.ExcusedAbsences,
                        UnexcusedAbsences = item.UnexcusedAbsences,
                        IsActive = item.IsActive,
                        ClassId = item.ClassId,
                        Class = new ClassDTO
                        {
                            Id = @class.Id,
                            Grade = ClassManagementService.GetGrade(@class.Name),
                            Name = @class.Name,
                            Capacity = @class.Capacity,
                            AverageScore = @class.AverageScore,
                            Specialization = @class.Specialization,
                            FormTeacherId = @class.FormTeacherId,
                            FormTeacher = null
                        }
                    });
                }
            }

            return studentsDto;
        }

        public StudentDTO? GetById(int id)
        {
            StudentDTO? studentDTO = null;

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Student student = unitOfWork.StudentRepository.GetByID(id);

                if (student != null)
                {
                    Class @class = unitOfWork.ClassRepository.GetByID(student.ClassId);
                    studentDTO = new StudentDTO
                    {
                        Id = student.Id,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        BirthDate = student.BirthDate,
                        ExcusedAbsences = student.ExcusedAbsences,
                        UnexcusedAbsences = student.UnexcusedAbsences,
                        IsActive = student.IsActive,
                        ClassId = student.ClassId,
                        Class = new ClassDTO
                        {
                            Id = @class.Id,
                            Name = @class.Name,
                            Grade = ClassManagementService.GetGrade(@class.Name),
                            Capacity = @class.Capacity,
                            AverageScore = @class.AverageScore,
                            Specialization = @class.Specialization,
                            FormTeacherId = @class.FormTeacherId,
                            FormTeacher = null
                        }
                    };
                }
            }

            return studentDTO;
        }

        public bool Save(StudentDTO studentDTO)
        {
            Student student = new Student
            {
                Id = studentDTO.Id,
                FirstName = studentDTO.FirstName,
                LastName = studentDTO.LastName,
                BirthDate = studentDTO.BirthDate,
                ExcusedAbsences = studentDTO.ExcusedAbsences,
                UnexcusedAbsences = studentDTO.UnexcusedAbsences,
                IsActive = studentDTO.IsActive,
                ClassId = studentDTO.ClassId,
                Class = null!
            };

            try
            {
                //Separating to other unit of work block to avoid tracking exception
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    Class @class = unitOfWork.ClassRepository.GetByID(studentDTO.ClassId);
                    //Not allowing saving of a student when the class is already at full capacity
                    if (unitOfWork.StudentRepository.Get(x => x.ClassId == @class.Id).Count() >= @class.Capacity)
                    {
                        return false;
                    }
                }

                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    if (studentDTO.Id == 0)
                    {
                        student.CreatedOn = DateTime.Now;
                        unitOfWork.StudentRepository.Insert(student);
                    }
                    else
                    {
                        student.UpdatedOn = DateTime.Now;
                        unitOfWork.StudentRepository.Update(student);
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
                    Student student = unitOfWork.StudentRepository.GetByID(id);
                    unitOfWork.StudentRepository.Delete(student);
                    unitOfWork.Save();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetPageCount(int itemsPerPage, string? firstName = null, string? lastName = null)
        {
            if (itemsPerPage <= 0)
            {
                return 1;
            }

            int pageCount = 0;

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Expression<Func<Student, bool>> filter =
                        (x => (!string.IsNullOrEmpty(firstName) ? x.FirstName == firstName : true) &&
                              (!string.IsNullOrEmpty(lastName) ? x.LastName == lastName : true));
                pageCount = (int)Math.Ceiling((double)unitOfWork.StudentRepository.Get(filter).Count() / itemsPerPage);
            }

            return pageCount;
        }
    }
}
