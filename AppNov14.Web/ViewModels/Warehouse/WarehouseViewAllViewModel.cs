using AppNov14.Models.Warehouse;
using AppNov14.Web.ViewModels.Base;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Web.ViewModels.Warehouse
{
    public class WarehouseViewAllViewModel : BaseViewModel
    {
        public string Type { get; set; }

        public List<WarehouseModel> Items { get; set; }

        public List<string> DistinctTypes { get; set; }

        public List<SelectListItem> DistinctSelectTypes { get; set; }
    }
}