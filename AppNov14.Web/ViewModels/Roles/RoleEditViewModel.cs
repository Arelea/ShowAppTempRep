using AppNov14.Web.Models.Roles;
using System;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Roles
{
    public class RoleEditViewModel : BaseViewModel
    {
        public ChangeUserRoleModel ChangeRoleModel { get; set; }
    }
}