using System.Web.WebPages.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;

namespace AppNov14.Web.ViewModels.Laboratory
{
    public sealed class CreateOutcomingOrderLabViewModel : BaseViewModel
    {
        public CreateOutcomingOrderLabForm Form { get; set; }

        public List<SelectListItem> TypeOfMaterialList { get; set; }

        public List<SelectListItem> ConsignmentNamesList { get; set; }
    }

    public sealed class CreateOutcomingOrderLabForm : BaseWarehouseParamsRequiredForm
    {
        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public string Document { get; set; }

        [Required]
        public string DocumentNumber { get; set; }

        [Required]
        public string Indexation { get; set; }

        [Required]
        public DateTime DocDate { get; set; }

        public string Remarks { get; set; }
    }
}