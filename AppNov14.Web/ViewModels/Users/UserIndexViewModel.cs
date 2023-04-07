using System;
using System.Collections.Generic;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Users
{
    public class UserIndexViewModel : BaseViewModel
    {
        public List<AppNov14.Web.Models.Users.Users> Users { get; set; }
    }
}