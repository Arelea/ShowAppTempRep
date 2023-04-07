using AppNov14.Web.ViewModels.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppNov14.Web.ViewModels.Account
{
    public sealed class AccountLoginViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string LoginApp { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
