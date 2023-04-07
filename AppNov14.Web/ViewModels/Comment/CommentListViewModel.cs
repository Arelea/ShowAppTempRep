using AppNov14.Models.Comment;
using AppNov14.Web.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Web.ViewModels.Comment
{
    public class CommentListViewModel : BaseViewModel
    {
        public List<CommentModel> Items { get; set; }

        public List<SelectListItem> Statuses { get; set; }

        public string Status { get; set; }
    }
}