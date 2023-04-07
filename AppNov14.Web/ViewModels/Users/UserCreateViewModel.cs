using System;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Users
{
    public class UserCreateViewModel : BaseViewModel
    {
        public UserCreateForm Form { get; set; }
    }

    public sealed class UserCreateForm : BaseForm
    {
        public string LoginApp { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }
    }
}