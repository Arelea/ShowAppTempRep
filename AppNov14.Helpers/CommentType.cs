using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Helpers
{
    public static class CommentType
    {
        public static int Manufacturing = 1;

        public static int Laboratory = 2;

        public static int Goods = 3;

        public static int Other = 4;

        public static readonly List<SelectListItem> TypesList = new List<SelectListItem>
        {
            new SelectListItem() { Text = "Производственная база", Value = CommentType.Manufacturing.ToString() },
            new SelectListItem() { Text = "Лабораторная база", Value = CommentType.Laboratory.ToString() },
            new SelectListItem() { Text = "Отгрузочная база", Value =  CommentType.Goods.ToString() },
            new SelectListItem() { Text = "Прочие", Value =  CommentType.Other.ToString() },
        };

        public static string GetType(int type)
        {
            if (type == CommentType.Manufacturing)
            {
                return "Производство";
            }
            else if (type == CommentType.Laboratory)
            {
                return "Лаборатория";
            }
            else if (type == CommentType.Goods)
            {
                return "Отгрузки";
            }
            else if (type == CommentType.Other)
            {
                return "Прочие";
            }
            else
            {
                return "";
            }
        }
    }
}