using System;
using System.Collections.Generic;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Roles
{
    public sealed class RoleUserListViewModel : BaseViewModel
    {
        public List<AppNov14.Web.Models.Users.Users> UserList { get; set; }
    }
}
