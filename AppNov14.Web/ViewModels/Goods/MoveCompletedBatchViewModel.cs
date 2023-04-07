using System;
using System.ComponentModel.DataAnnotations;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class MoveCompletedBatchViewModel : BaseViewModel
    {
        public BatchDisplayModel Item { get; set; }

        public MoveCompletedBatchForm Form { get; set; }
    }

    public sealed class MoveCompletedBatchForm : BaseForm
    {

        [DisplayFormat(DataFormatString = "{0,000}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal InitialQuantity { get; set; }

        [Required]
        public int InitialPackage { get; set; }

        [Required]
        public DateTime CompletionDate { get; set; }

        public string Remark { get; set; }

        [Required]
        public int Id { get; set; }
    }
}