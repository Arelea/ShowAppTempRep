using System;
using System.Web.WebPages.Html;
using System.Collections.Generic;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using AppNov14.Models.Warehouse;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class ViewIndexViewModel : BaseViewModel
    {
        public ViewIndexForm Form { get; set; }

        public List<IndexDisplayDataModel> Items { get; set; }

        public List<SelectListItem> TypeOfMaterialList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> NameOfTypeMaterialList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ProviderList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ManufacturerList { get; set; } = new List<SelectListItem>();
    }

    public sealed class ViewIndexForm : BaseWarehouseParamsForm
    {
        public string IndexName { get; set; }

        public bool ByWarehouse { get; set; } = true;
    }
}