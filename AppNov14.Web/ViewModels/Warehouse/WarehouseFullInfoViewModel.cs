using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;
using AppNov14.Models.Warehouse;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;

namespace AppNov14.Web.ViewModels.Warehouse
{
    public sealed class WarehouseFullInfoViewModel : BaseViewModel
    {
        public WarehouseFullInfoForm Form { get; set; }

        public List<IndexDisplayDataModel> Items { get; set; }

        public List<SelectListItem> TypeOfMaterialList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> NameOfTypeMaterialList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ProviderList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ManufacturerList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ExpiredModeList { get; set; } = new List<SelectListItem>();
    }

    public sealed class WarehouseFullInfoForm : BaseWarehouseParamsForm
    {
        public int? Id { get; set; }

        public string IndexName { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateFinish { get; set; }

        public bool ShowEmpty { get; set; }

        public int? ExpiredMode { get; set; }
    }
}