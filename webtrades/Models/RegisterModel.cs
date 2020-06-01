using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webtrades.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Длина логина должна быть от 5 до 20 символов")]
        [Remote(action: "CheckLogin", controller: "LoginScreen", ErrorMessage = "Login уже используется")]
        public string Login { get; set; }//Логин при регистрации

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Длина пароля должна быть от 7 до 20 символов")]
        public string Password { get; set; }//Пароль при регистрации

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }//Подтверждение пароля при регистрации
    }
}
