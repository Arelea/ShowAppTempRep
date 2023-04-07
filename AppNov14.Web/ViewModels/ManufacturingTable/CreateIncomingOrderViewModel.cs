using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class CreateIncomingOrderViewModel : BaseViewModel
    {
        public CreateIncomingOrderForm Form { get; set; }

        public List<SelectListItem> TypeList { get; set; }
    }

    public sealed class CreateIncomingOrderForm : BaseWarehouseParamsRequiredForm
    {
        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public string ReplenishmentDocument { get; set; }

        [Required]
        public string Index { get; set; }

        [Required]
        public DateTime DocDate { get; set; }

        public string Remarks { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }
}