using System.Web.WebPages.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using AppNov14.Models.Manufacturing;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class CreateOutcomingOrderViewModel : BaseViewModel
    {
        public CreateOutcomingOrderForm Form { get; set; }

        public List<SelectListItem> TypeList { get; set; }

        public List<SelectListItem> BatchLineList { get; set; }

        public List<SelectListItem> BatchTypeList { get; set; }

        public List<SelectListItem> BatchList { get; set; }

        public List<SelectListItem> SubTypeList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ProviderList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ManufacturerList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> IndexList { get; set; } = new List<SelectListItem>();
    }

    public sealed class CreateOutcomingOrderForm : BaseWarehouseParamsRequiredForm
    {
        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public int BatchTypeId { get; set; }

        [Required]
        public string BatchName { get; set; }

        public bool IsNewBatch { get; set; } = false;

        [Required]
        public int IndexId { get; set; }

        [Required]
        public int BatchLineId { get; set; }

        [Required]
        public int BatchId { get; set; }

        [Required]
        public DateTime DocDate { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public string Remarks { get; set; }

        public void Fill(ManufacturingRecordModel model)
        {
            if (model != null)
            {        
                this.Type = model.Type;
                this.SubType = model.SubType;
                this.Provider = model.Provider;
                this.Manufacturer = model.Manufacturer;
                this.DocDate = model.DocDate;
                this.Remarks = model.Remarks;
                this.IndexId = model.IndexId.Value;
            }
        }
    }  
}