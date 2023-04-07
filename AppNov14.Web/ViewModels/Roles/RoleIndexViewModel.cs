using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Roles
{
    public sealed class RoleIndexViewModel : BaseViewModel
    {
        public List<IdentityRole> Roles { get; set; }
    }
}