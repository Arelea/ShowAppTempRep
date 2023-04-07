using System;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Error
{
    public sealed class ErrorViewModel : BaseViewModel
    {
        public string ExceptionPath { get; set; }

        public string ExceptionMessage { get; set; }
    }
}