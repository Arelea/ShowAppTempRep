using System;
using System.Collections.Generic;
using AppNov14.Models.Warehouse;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class IndexInfoViewModel : BaseViewModel
    {
        public IndexDisplayDataModel Item { get; set; }

        public Dictionary<int, string> LinkedBatches { get; set; }
    }
}