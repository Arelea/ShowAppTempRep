using System;
using System.ComponentModel.DataAnnotations;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Laboratory
{
    public class ImportExcelFileLabViewModel : BaseViewModel
    {
        public ImportExcelFileLabForm Form { get; set; }

        public int ResultsCount { get; set; }
    }

    public sealed class ImportExcelFileLabForm : BaseForm
    {
        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateFinish { get; set; }
    }
}