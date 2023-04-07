using AppNov14.Web.ViewModels.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppNov14.Web.ViewModels.Account
{
    public class AccountRegisterViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string LoginApp { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string LName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}