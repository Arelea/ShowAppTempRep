using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace AppNov14.Web.ViewModels.Warehouse
{
    public sealed class WarehouseCreateNewWarehouseFieldViewModel : BaseViewModel
    {
        public WarehouseCreateNewWarehouseFieldForm Form { get; set; }

        public List<SelectListItem> TypeOfMaterialList { get; set; }
    }

    public sealed class WarehouseCreateNewWarehouseFieldForm : BaseWarehouseParamsRequiredForm
    {
    }
}