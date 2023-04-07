using AppNov14.Models.Warehouse;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Web.ViewModels.Laboratory
{
    public sealed class ViewIndexLabViewModel : BaseViewModel
    {
        public ViewIndexLabForm Form { get; set; }

        public List<IndexDataModel> Items { get; set; }

        public List<SelectListItem> TypeOfMaterialList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> NameOfTypeMaterialList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ProviderList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ManufacturerList { get; set; } = new List<SelectListItem>();
    }

    public sealed class ViewIndexLabForm : BaseWarehouseParamsRequiredForm
    {
    }
}