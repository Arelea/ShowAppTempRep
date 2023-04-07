using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;
using AppNov14.Models.Manufacturing;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public class CreateCorrectionOutcomingOrderViewModel : BaseViewModel
    {
        public CreateCorrectionOutcomingOrderForm Form { get; set; }

        public List<SelectListItem> TypeList { get; set; }

        public List<SelectListItem> BatchList { get; set; }
    }

    public sealed class CreateCorrectionOutcomingOrderForm : BaseWarehouseParamsRequiredForm
    {
        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public int IndexId { get; set; }

        [Required]
        public int BatchId { get; set; }

        [Required]
        public DateTime DocDate { get; set; }

        public string Remarks { get; set; }
    }
}
