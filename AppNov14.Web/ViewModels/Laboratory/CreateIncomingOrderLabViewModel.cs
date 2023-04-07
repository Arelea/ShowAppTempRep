using System.Web.WebPages.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;

namespace AppNov14.Web.ViewModels.Laboratory
{
    public sealed class CreateIncomingOrderLabViewModel : BaseViewModel
    {
        public CreateIncomingOrderLabForm Form { get; set; }

        public List<SelectListItem> TypeOfMaterialList { get; set; }
    }

    public sealed class CreateIncomingOrderLabForm : BaseWarehouseParamsRequiredForm
    {
        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public string Indexation { get; set; }

        [Required]
        public DateTime DocDate { get; set; }

        public string Remarks { get; set; }
    }
}