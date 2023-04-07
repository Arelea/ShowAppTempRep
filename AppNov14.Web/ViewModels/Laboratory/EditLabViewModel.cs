using System.Web.WebPages.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using AppNov14.Models.ManufacturingTable;

namespace AppNov14.Web.ViewModels.Laboratory
{
    public class EditLabViewModel : BaseViewModel
    {
        public EditLabForm Form { get; set; }

        public List<SelectListItem> TypeOfMaterialList { get; set; }

        public List<SelectListItem> NameOfTypeMaterialList { get; set; }

        public List<SelectListItem> ProviderList { get; set; }

        public List<SelectListItem> ManufacturerList { get; set; }

        public List<SelectListItem> ConsignmentTypesList { get; set; }

        public List<SelectListItem> IndexList { get; set; }
    }

    public sealed class EditLabForm : BaseWarehouseParamsRequiredForm
    {
        public int Id { get; set; }

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

        public string Employee { get; set; }

        public string IpAddress { get; set; }

        public DateTime AutoDate { get; set; }

        public string Remarks { get; set; }

        public int OperationType { get; set; }

        public void Fill(ManufacturingTableWriteModel model)
        {
            this.Id = model.Id;
            this.Type = model.Type;
            this.SubType = model.SubType;
            this.Provider = model.Provider;
            this.Manufacturer = model.Manufacturer;
            this.Quantity = model.Quantity;
            this.Document = model.Document;
            this.DocumentNumber = model.DocumentNumber;
            this.DocDate = model.DocDate;
            this.Remarks = model.Remarks;
            this.Indexation = model.Indexation;
            this.OperationType = model.OperationType;
            this.Employee = model.Employee;
        }
    }
}