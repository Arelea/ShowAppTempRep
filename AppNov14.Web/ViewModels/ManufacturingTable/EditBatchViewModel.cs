using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public class EditBatchViewModel : BaseViewModel
    {
        public EditBatchForm Form { get; set; }

        public List<SelectListItem> BatchTypeList { get; set; }

        public List<SelectListItem> BatchStatusList { get; set; }

        public List<SelectListItem> BatchLineList { get; set; }
    }

    public sealed class EditBatchForm : BaseForm
    {
        public int Id { get; set; }

        [Required]
        public string BatchName { get; set; }

        [Required]
        public int? BatchTypeId { get; set; }

        [Required]
        public int BatchLineId { get; set; }

        [Required]
        public int BatchStatusId { get; set; }

        [Required]
        public DateTime? CreateDate { get; set; }

        public void Fill(BatchModel model)
        {
            this.Id = model.Id;
            this.BatchTypeId = model.TypeId;
            this.BatchName = model.Name;
            this.BatchStatusId = model.StatusId;
            this.BatchLineId = model.LineId;
            this.CreateDate = model.CreateDate;
        }
    }
}