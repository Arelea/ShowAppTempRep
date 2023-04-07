using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Helpers
{
    public static class OperationTypes
    {
        public static readonly List<SelectListItem> OperationTypeList = new List<SelectListItem>
        {
            new SelectListItem() { Text = "Все записи", Value = "4" },
            new SelectListItem() { Text = "Пополнение", Value = "1" },
            new SelectListItem() { Text = "Списание", Value = "2" },           
        };
    }
}