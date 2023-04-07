using System.Web.WebPages.Html;
using System;
using System.Collections.Generic;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using AppNov14.Models.ManufacturingTable;

namespace AppNov14.Web.ViewModels.Search
{
    public sealed class CommonSearchViewModel : BaseViewModel
    {
        public CommonSearchForm Form { get; set; }

        public List<SelectListItem> TypesList { get; set; }

        public List<SelectListItem> SubTypesList { get; set; }

        public List<SelectListItem> ProviderList { get; set; }

        public List<SelectListItem> ManufacturerList { get; set; }

        public List<SelectListItem> ConsignmentTypesList { get; set; }

        public List<SelectListItem> ConsignmentNumbersList{ get; set; }

        public List<SelectListItem> FactoryLinesList { get; set; }

        public List<SelectListItem> ShowModeList { get; set; }

        public List<ManufacturingTableWriteModel> Items { get; set; }
    }

    public sealed class CommonSearchForm : BaseWarehouseParamsForm
    {
        public int? Id { get; set; }

        public string Document { get; set; }

        public string DocumentNumber { get; set; }

        public string Index { get; set; } = null;

        public string Line { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateFinish { get; set; }

        public string ShowMode { get; set; }
    }
}