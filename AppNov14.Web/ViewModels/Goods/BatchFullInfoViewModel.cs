using System;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class BatchFullInfoViewModel : BaseViewModel
    {
        public BatchFullDisplayModel Batch { get; set; }
    }
}