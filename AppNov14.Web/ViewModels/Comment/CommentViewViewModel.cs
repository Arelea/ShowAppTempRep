using AppNov14.Models.Comment;
using AppNov14.Web.ViewModels.Base;
using System;

namespace AppNov14.Web.ViewModels.Comment
{
    public class CommentViewViewModel : BaseViewModel
    {
        public CommentModel Item { get; set; }

        public CommentViewForm Form { get; set; }
    }

    public sealed class CommentViewForm : BaseForm
    {
        public int Id { get; set; }

        public string Answer { get; set; }
    }
}
