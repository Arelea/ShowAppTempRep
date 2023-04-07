using AppNov14.Helpers;
using AppNov14.Models.Manufacturing;
using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class SearchViewModel : BaseViewModel
    {
        public SearchForm Form { get; set; }

        public List<SelectListItem> TypesList { get; set; }

        public List<SelectListItem> SubTypesList { get; set; }

        public List<SelectListItem> ProviderList { get; set; }

        public List<SelectListItem> ManufacturerList { get; set; }

        public List<SelectListItem> BatchTypeList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> BatchList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> BatchLineList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ReplenishmentDocumentList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ShowModeList { get; set; }

        public List<ManufacturingRecordDisplayModel> Items { get; set; }
    }

    public sealed class SearchForm : BaseWarehouseParamsForm
    {
        public int? Id { get; set; }

        public int? BatchId { get; set; }

        public int? BatchTypeId { get; set; }

        public string Index { get; set; } = null;

        public int? BatchLineId { get; set; }

        public int? ReplenishmentDocumentId { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateFinish { get; set; }

        public int ShowMode { get; set; } = RawActionTypes.All;
    }
}