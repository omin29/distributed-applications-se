using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context
{
    public class SchoolInfoContext : DbContext
    {
        public const string ConnectionString = "Server=localhost;Database=SchoolInfo;Trusted_Connection=True;TrustServerCertificate=True";
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<TeacherClass> TeachersClasses { get; set;}
        public DbSet<User> Users { get; set; }

        public SchoolInfoContext()
        {
            Teachers = Set<Teacher>();
            Classes = Set<Class>();
            Students = Set<Student>();
            TeachersClasses = Set<TeacherClass>();
            Users = Set<User>();
        }

        public SchoolInfoContext(DbContextOptions<SchoolInfoContext> options):base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(ConnectionString);
        }

    }
}
