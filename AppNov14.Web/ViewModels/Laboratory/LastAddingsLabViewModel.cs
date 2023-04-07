using System;
using System.Collections.Generic;
using AppNov14.Models.ManufacturingTable;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Laboratory
{
    public class LastAddingsLabViewModel : BaseViewModel
    {
        public List<ManufacturingTableWriteModel> Items { get; set; }
    }
}