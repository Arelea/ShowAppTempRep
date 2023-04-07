using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AppNov14.Web.Models.Roles
{
    public class ChangeUserRoleModel
    {
        public string UserId { get; set; }

        public string UserLogin { get; set; }

        public List<IdentityRole> AllRoles { get; set; }

        public IList<string> UserRoles { get; set; }

        public ChangeUserRoleModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}