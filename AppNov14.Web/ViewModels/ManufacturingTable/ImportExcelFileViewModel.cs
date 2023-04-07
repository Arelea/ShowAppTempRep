using System;
using System.ComponentModel.DataAnnotations;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class ImportExcelFileViewModel : BaseViewModel
    {
        public ImportExcelFileForm Form { get; set; }

        public int ResultsCount { get; set; }
    }

    public sealed class ImportExcelFileForm : BaseForm
    {
        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateFinish { get; set; }
    }
}