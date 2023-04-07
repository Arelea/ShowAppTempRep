using System;
using AppNov14.Models.Manufacturing;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class DeleteTableRowViewModel : BaseViewModel
    {
        public DeleteForm Form { get; set; }
    }

    public sealed class DeleteForm : BaseWarehouseParamsForm
    {
        public int Id { get; set; }

        public decimal Quantity { get; set; }

        public string ReplenishmentDocument { get; set; }

        public string Index { get; set; }

        public DateTime DocDate { get; set; }

        public DateTime InsertDate { get; set; }

        public string Employee { get; set; }

        public string IpAddress { get; set; }

        public string Remarks { get; set; }

        public int ActionType { get; set; }

        public string BatchName { get; set; }

        public string BatchLine { get; set; }

        public string BatchType { get; set; }

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
            this.ActionType = model.ActionType;
            this.Employee = model.Employee;
            this.ReplenishmentDocument = model.ReplenishmentDocument;
            this.Index = model.Index;
            this.BatchName = model.BatchName;
            this.BatchType = model.BatchType;
            this.BatchLine = model.BatchLine;
            this.InsertDate = model.InsertDate;
        }
    }
}