using ApplicationService.DTOs;
using Data.Entities;
using Repository.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class TeacherManagementService
    {
        public List<TeacherDTO> Get(int? page = null, int? itemsPerPage = null, string? firstName = null, string? lastName = null,
            bool onlyWithoutTheirOwnClass = false)
        {
            List<TeacherDTO> teachersDto = new List<TeacherDTO>();

            using(UnitOfWork unitOfWork = new UnitOfWork())
            {
                IEnumerable<Teacher> teachers = new List<Teacher>();

                if (onlyWithoutTheirOwnClass)
                {                    
                    List<int> idsOfTeachersWithTheirOwnClass = unitOfWork.ClassRepository.Get()
                        .Select(x => x.FormTeacherId).ToList();
                    //Filtering teachers which are not a form teacher of a class
                    teachers = unitOfWork.TeacherRepository.Get(x => idsOfTeachersWithTheirOwnClass.Contains(x.Id) == false);
                }
                else
                {
                    Expression<Func<Teacher, bool>> filter =
                        (x => (!string.IsNullOrEmpty(firstName)? x.FirstName == firstName : true) &&
                              (!string.IsNullOrEmpty(lastName)? x.LastName == lastName:true));
                    teachers = unitOfWork.TeacherRepository.Get(filter);
                }

                if(page != null && itemsPerPage != null && page > 0 && itemsPerPage > 0)
                {
                    if (page <= GetPageCount(itemsPerPage.Value, firstName, lastName))
                    {
                        teachers = teachers.Skip((page.Value - 1) * itemsPerPage.Value).Take(itemsPerPage.Value).ToList();
                    }
                    else
                    {
                        //No result
                        return teachersDto;
                    }
                }

                foreach (var item in teachers)
                {
                    teachersDto.Add(new TeacherDTO
                    { 
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        BirthDate = item.BirthDate,
                        Phone = item.Phone,
                        YearsExperience = item.YearsExperience,
                        IsOnLeave = item.IsOnLeave
                    });
                }
            }

            return teachersDto;
        }

        public TeacherDTO? GetById(int id)
        {
            TeacherDTO? teacherDTO = null;

            using(UnitOfWork unitOfWork = new UnitOfWork())
            {
                Teacher teacher = unitOfWork.TeacherRepository.GetByID(id);

                if (teacher != null)
                {
                    teacherDTO = new TeacherDTO
                    {
                        FirstName = teacher.FirstName,
                        LastName = teacher.LastName,
                        BirthDate = teacher.BirthDate,
                        Phone = teacher.Phone,
                        YearsExperience = teacher.YearsExperience,
                        IsOnLeave = teacher.IsOnLeave,
                        Id = id
                    };
                }
            }

            return teacherDTO;
        }

        public bool Save(TeacherDTO teacherDTO)
        {
            Teacher teacher = new Teacher
            {
                Id = teacherDTO.Id,
                FirstName = teacherDTO.FirstName,
                LastName = teacherDTO.LastName,
                BirthDate = teacherDTO.BirthDate,
                Phone = teacherDTO.Phone,
                YearsExperience = teacherDTO.YearsExperience,
                IsOnLeave = teacherDTO.IsOnLeave,
                TheirClass = null!,
                TeachersClasses = null!
            };

            try
            {
                using(UnitOfWork unitOfWork = new UnitOfWork())
                {
                    if(teacherDTO.Id == 0)
                    {
                        teacher.CreatedOn = DateTime.Now;
                        unitOfWork.TeacherRepository.Insert(teacher);
                    }
                    else
                    {
                        teacher.UpdatedOn = DateTime.Now;
                        unitOfWork.TeacherRepository.Update(teacher);
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
                using(UnitOfWork unitOfWork = new UnitOfWork())
                {
                    Teacher teacher = unitOfWork.TeacherRepository.GetByID(id);
                    unitOfWork.TeacherRepository.Delete(teacher);
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
            if(itemsPerPage <= 0)
            {
                return 1;
            }

            int pageCount = 0;

            using(UnitOfWork unitOfWork = new UnitOfWork())
            {
                Expression<Func<Teacher, bool>> filter =
                        (x => (!string.IsNullOrEmpty(firstName) ? x.FirstName == firstName : true) &&
                              (!string.IsNullOrEmpty(lastName) ? x.LastName == lastName : true));
                pageCount = (int)Math.Ceiling((double)unitOfWork.TeacherRepository.Get(filter).Count() / itemsPerPage);
            }

            return pageCount;
        }
    }
}
