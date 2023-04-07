using System;
using System.Collections.Generic;
using AppNov14.Models.Manufacturing;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class EveryoneLastAddingsViewModel : BaseViewModel
    {
        public List<ManufacturingRecordDisplayModel> Items { get; set; }
    }
}