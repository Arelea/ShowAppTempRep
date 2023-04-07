using System;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Users
{
    public sealed class UserEditViewModel : BaseViewModel
    {
        public UserEditForm Form { get; set; }
    }

    public sealed class UserEditForm : BaseForm
    {
        public string Id { get; set; }

        public string LoginApp { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}