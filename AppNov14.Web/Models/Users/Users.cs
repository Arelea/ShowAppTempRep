using Microsoft.AspNetCore.Identity;
using System;

namespace AppNov14.Web.Models.Users
{
    public class Users : IdentityUser
    {
        public string FirstName { get; set; }

        public string FamilyName { get; set; }
    }
}