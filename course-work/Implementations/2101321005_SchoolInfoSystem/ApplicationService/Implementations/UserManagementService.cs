using ApplicationService.DTOs;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class UserManagementService
    {
        public bool Register(UserDTO userDTO)
        {
            if (userDTO == null || Exists(userDTO))
            {
                return false;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            User user = new User()
            {
                Username = userDTO.Username,
                HashedPassword = hashedPassword
            };

            try
            {
                using(UnitOfWork unitOfWork = new UnitOfWork())
                {
                    user.CreatedOn = DateTime.Now;
                    unitOfWork.UserRepository.Insert(user);
                    unitOfWork.Save();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool Login(UserDTO userDTO)
        {
            bool isSuccessful = true;

            using(UnitOfWork unitOfWork = new UnitOfWork())
            {
                User? user = unitOfWork.UserRepository.Get(x => x.Username == userDTO.Username).FirstOrDefault();
                
                if (user == null ||
                !BCrypt.Net.BCrypt.Verify(userDTO.Password, user.HashedPassword))
                {
                    isSuccessful = false;
                }
            }

            return isSuccessful;
        }

        public bool Exists(UserDTO userDTO)
        {
            if(userDTO == null)
            {
                return false;
            }

            bool exists = true;

            using(UnitOfWork unitOfWork = new UnitOfWork())
            {
                User? user = unitOfWork.UserRepository.Get(x => x.Username == userDTO.Username).FirstOrDefault();

                if(user == null)
                {
                    exists = false;
                }
            }

            return exists;
        }
    }
}
