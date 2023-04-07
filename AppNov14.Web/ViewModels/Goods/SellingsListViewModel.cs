using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;
using AppNov14.Models.Batch;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.Goods
{
    public sealed class SellingsListViewModel : BaseViewModel
    {
        public SellingsListForm Form { get; set; }

        public List<SelectListItem> BatchNameList { get; set; }

        public List<SelectListItem> CustomerList { get; set; }

        public List<BatchActionDisplayModel> Items { get; set; }
    }

    public sealed class SellingsListForm : BaseForm
    {
        public int? CustomerId { get; set; }

        public string Query { get; set; }

        public DateTime? SellDateTo { get; set; }

        public DateTime? SellDateFrom { get; set; }

        public int? BatchId { get; set; }
    }
}
