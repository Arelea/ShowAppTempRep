using AppNov14.Models.Manufacturing;
using AppNov14.Web.ViewModels.Base;
using System.Collections.Generic;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class MyLastAddingViewModel : BaseViewModel
    {
        public List<ManufacturingRecordDisplayModel> Items { get; set; }
    }
}