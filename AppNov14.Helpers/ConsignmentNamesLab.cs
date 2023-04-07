using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Helpers
{
    public static class ConsignmentNamesLab
    {
        public const string K6 = "K6";

        public const string K7 = "K7";

        public const string K8 = "K8";

        public static readonly List<SelectListItem> ConsignmentNamesListLab = new List<SelectListItem>
        {
            new SelectListItem() { Text = ConsignmentNamesLab.K6, Value = ConsignmentNamesLab.K6 },
            new SelectListItem() { Text = ConsignmentNamesLab.K7, Value = ConsignmentNamesLab.K7 },
            new SelectListItem() { Text = ConsignmentNamesLab.K8, Value = ConsignmentNamesLab.K8 }
        };

        public static readonly List<SelectListItem> ConsignmentNamesListLabWithUnselect = new List<SelectListItem>
        {
            new SelectListItem() { Text = "", Value = null },
            new SelectListItem() { Text = ConsignmentNamesLab.K6, Value = ConsignmentNamesLab.K6 },
            new SelectListItem() { Text = ConsignmentNamesLab.K7, Value = ConsignmentNamesLab.K7 },
            new SelectListItem() { Text = ConsignmentNamesLab.K8, Value = ConsignmentNamesLab.K8 }
        };
    }
}