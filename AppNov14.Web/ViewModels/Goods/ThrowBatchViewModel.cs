using System.ComponentModel.DataAnnotations;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public class ThrowBatchViewModel : BaseViewModel
    {
        public BatchExtendedDisplayModel Item { get; set; }

        public ThrowBatchForm Form { get; set; }
    }

    public sealed class ThrowBatchForm : BaseForm
    {

        [Required]
        public string Remark { get; set; }

        [Required]
        public int Id { get; set; }
    }
}