using System;
using System.ComponentModel.DataAnnotations;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class AddBatchTypeViewModel : BaseViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}