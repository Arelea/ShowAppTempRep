using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Helpers
{
    public static class CommentStatus
    {
        public static int Opened = 1;

        public static int Closed = 2;

        public static readonly List<SelectListItem> StatusesList = new List<SelectListItem>
        {
            new SelectListItem() { Text = "Открытые", Value = CommentStatus.Opened.ToString() },
            new SelectListItem() { Text = "Закрытые", Value =  CommentStatus.Closed.ToString() },
        };

        public static string GetStatus(int status)
        {
            if (status == CommentStatus.Opened)
            {
                return "Открыт";
            }
            else if (status == CommentStatus.Closed)
            {
                return "Закрыт";
            }
            else
            {
                return "";
            }
        }
    }
}