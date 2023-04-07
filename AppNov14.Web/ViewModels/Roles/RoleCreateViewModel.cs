using System;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Roles
{
    public sealed class RoleCreateViewModel : BaseViewModel
    {
        public RoleCreateForm Form { get; set; }
    }

    public sealed class RoleCreateForm : BaseForm
    {
        public string RoleName { get; set; }
    }
}