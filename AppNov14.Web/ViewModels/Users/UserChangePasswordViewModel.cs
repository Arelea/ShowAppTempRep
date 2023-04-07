using System;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Users
{
    public class UserChangePasswordViewModel : BaseViewModel
    {
        public UserChangePasswordForm Form { get; set; }
    }

    public sealed class UserChangePasswordForm : BaseForm
    {
        public string Id { get; set; }

        public string LoginApp { get; set; }

        public string NewPassword { get; set; }
    }
}