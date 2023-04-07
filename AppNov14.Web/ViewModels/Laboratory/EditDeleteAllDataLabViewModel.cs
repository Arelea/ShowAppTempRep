using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Laboratory
{
    public class EditDeleteAllDataLabViewModel : BaseViewModel
    {
        public EditDeleteAllDataLabForm Form { get; set; }

        public List<ManufacturingTableWriteModel> Items { get; set; }
    }

    public sealed class EditDeleteAllDataLabForm : BaseForm
    {
        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateFinish { get; set; }
    }
}