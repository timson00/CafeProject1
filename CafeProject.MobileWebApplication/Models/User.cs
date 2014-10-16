using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using System.ComponentModel.DataAnnotations;

namespace CafeProject.MobileWebApplication.Models
{
    public class User
    {
        public Guid ID { get; set; }
        [Required]
        [StringLength (255, MinimumLength = 2, ErrorMessage="Длина строки должна быть от 2 до 255 символов")]
        public string Surname { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 2 до 255 символов")]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Длина строки должна быть от 5 до 255 символов")]
        public string Login { get; set; }
        public string ConfirmEmail { get; set; }
        public string Gender { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен состоять минимум из 6 элементов")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}