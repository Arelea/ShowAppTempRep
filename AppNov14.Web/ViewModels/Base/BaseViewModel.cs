using System;

namespace AppNov14.Web.ViewModels.Base
{
    public class BaseViewModel
    {
        public string PageName { get; set; }

        public string BackUrl { get; set; }

        public string BackUrlName { get; set; }

        public bool IsBackButtonNeeded { get; set; } = true;

        public bool IsLabMode { get; set; } = false;
    }
}