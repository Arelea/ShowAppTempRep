using AppNov14.Web.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;

namespace AppNov14.Web.ViewModels.Comment
{
    public sealed class CommentCreateViewModel : BaseViewModel
    {
        public CommentCreateForm Form { get; set; }

        public List<SelectListItem> CommentTypes { get; set; }
    }

    public class CommentCreateForm : BaseForm
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int Type { get; set; }
    }
}