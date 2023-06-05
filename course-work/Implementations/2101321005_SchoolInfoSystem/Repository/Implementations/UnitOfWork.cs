using Data.Context;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementations
{
    public class UnitOfWork : IDisposable
    {
        private SchoolInfoContext context = new SchoolInfoContext();
        private GenericRepository<Teacher> teacherRepository;
        private GenericRepository<Class> classRepository;
        private GenericRepository<Student> studentRepository;
        private GenericRepository<TeacherClass> teacherClassRepository;
        private GenericRepository<User> userRepository;

        public GenericRepository<Teacher> TeacherRepository
        {
            get
            {
                if(this.teacherRepository == null)
                {
                    this.teacherRepository = new GenericRepository<Teacher>(context);
                }
                return teacherRepository;
            }
        }

        public GenericRepository<Class> ClassRepository
        {
            get
            {

                if (this.classRepository == null)
                {
                    this.classRepository = new GenericRepository<Class>(context);
                }
                return classRepository;
            }
        }

        public GenericRepository<Student> StudentRepository
        {
            get
            {

                if (this.studentRepository == null)
                {
                    this.studentRepository = new GenericRepository<Student>(context);
                }
                return studentRepository;
            }
        }

        public GenericRepository<TeacherClass> TeacherClassRepository
        {
            get
            {
                if(this.teacherClassRepository == null)
                {
                    this.teacherClassRepository = new GenericRepository<TeacherClass>(context);
                }
                return teacherClassRepository;
            }
        }

        public GenericRepository<User> UserRepository
        {
            get
            {
                if(this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(context);
                }
                return userRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
