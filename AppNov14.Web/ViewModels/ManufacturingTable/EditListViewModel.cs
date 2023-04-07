using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppNov14.Models.Manufacturing;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class EditListViewModel : BaseViewModel
    {
        public EditDeleteAllDataForm Form { get; set; }

        public List<ManufacturingRecordDisplayModel> Items { get; set; }
    }

    public sealed class EditDeleteAllDataForm : BaseForm
    {
        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateFinish { get; set; }
    }
}