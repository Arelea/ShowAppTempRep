using AppNov14.Web.ViewModels.Base;
using System;

namespace AppNov14.Web.Models
{
    public class ErrorViewModel : BaseViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}