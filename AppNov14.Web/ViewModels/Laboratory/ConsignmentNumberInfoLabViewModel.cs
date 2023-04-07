using System.Web.WebPages.Html;
using System;
using System.Collections.Generic;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Models.ManufacturingTable;

namespace AppNov14.Web.ViewModels.Laboratory
{
    public class ConsignmentNumberInfoLabViewModel : BaseViewModel
    {
        public string Number { get; set; }

        public List<ManufacturingTableWriteModel> Items { get; set; }

        public List<SelectListItem> ConsignmentList { get; set; }
    }
}