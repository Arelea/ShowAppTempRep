using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Helpers
{
    public static class FactoryLineNames
    {
        public const string Bolshevik = "Bolshevik";

        public const string Xinda = "Xinda1";

        public const string Biersdorff = "Biersdorff";

        public const string Bandera = "BANDERA";

        public const string Maris = "MARIS";

        public const string SyntesLab = "Лаборатория синтеза";

        public const string Xinda2 = "Xinda2";

        public static readonly List<SelectListItem> FactoryLines = new List<SelectListItem>
        {
            new SelectListItem() { Text = FactoryLineNames.Bolshevik, Value = FactoryLineNames.Bolshevik },
            new SelectListItem() { Text = FactoryLineNames.Bandera, Value = FactoryLineNames.Bandera },
            new SelectListItem() { Text = FactoryLineNames.Xinda, Value = FactoryLineNames.Xinda },
            new SelectListItem() { Text = FactoryLineNames.Biersdorff, Value = FactoryLineNames.Biersdorff },
            new SelectListItem() { Text = FactoryLineNames.Maris, Value = FactoryLineNames.Maris },
            new SelectListItem() { Text = FactoryLineNames.SyntesLab, Value = FactoryLineNames.SyntesLab },
            new SelectListItem() { Text = FactoryLineNames.Xinda2, Value = FactoryLineNames.Xinda2 },
        };

        public static readonly List<SelectListItem> FactoryLinesWithUnselect = new List<SelectListItem>
        {
            new SelectListItem() { Text = "", Value = null },
            new SelectListItem() { Text = FactoryLineNames.Bolshevik, Value = FactoryLineNames.Bolshevik },
            new SelectListItem() { Text = FactoryLineNames.Bandera, Value = FactoryLineNames.Bandera },
            new SelectListItem() { Text = FactoryLineNames.Xinda, Value = FactoryLineNames.Xinda },
            new SelectListItem() { Text = FactoryLineNames.Biersdorff, Value = FactoryLineNames.Biersdorff },
            new SelectListItem() { Text = FactoryLineNames.Maris, Value = FactoryLineNames.Maris },
            new SelectListItem() { Text = FactoryLineNames.SyntesLab, Value = FactoryLineNames.SyntesLab },
            new SelectListItem() { Text = FactoryLineNames.Xinda2, Value = FactoryLineNames.Xinda2 },
        };
    }
}