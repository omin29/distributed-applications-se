using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User:BaseEntity
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string HashedPassword { get; set; }
    }
}
