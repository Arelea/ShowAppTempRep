using System.Web.WebPages.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using AppNov14.Models.Manufacturing;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class EditViewModel : BaseViewModel
    {
        public EditForm Form { get; set; }

        public List<SelectListItem> TypeList { get; set; }

        public List<SelectListItem> SubTypeList { get; set; }

        public List<SelectListItem> ProviderList { get; set; }

        public List<SelectListItem> ManufacturerList { get; set; }

        public List<SelectListItem> BatchList { get; set; }

        public List<SelectListItem> IndexList { get; set; }
    }

    public sealed class EditForm : BaseWarehouseParamsRequiredForm
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public int? BatchId { get; set; }

        [Required]
        public string ReplenishmentDocument { get; set; }

        [Required]
        public int? IndexId { get; set; }

        [Required]
        public string Index { get; set; }

        [Required]
        public DateTime DocDate { get; set; }

        public string Employee { get; set; }

        public string Remarks { get; set; }

        public int ActionType { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public void Fill(ManufacturingRecordDisplayModel model)
        {
            this.Id = model.Id;
            this.Type = model.Type;
            this.SubType = model.SubType;
            this.Provider = model.Provider;
            this.Manufacturer = model.Manufacturer;
            this.Quantity = model.Quantity; 
            this.DocDate = model.DocDate;
            this.Remarks = model.Remarks;
            this.IndexId = model.IndexId.Value;
            this.ActionType = model.ActionType;
            this.Employee = model.Employee;
            this.BatchId = model.BatchId;
            this.ReplenishmentDocument = model.ReplenishmentDocument;
            this.Index = model.Index;
            this.ExpirationDate = model.ExpirationDate;
            this.ManufacturingDate = model.ManufacturingDate;
        }
    }
}