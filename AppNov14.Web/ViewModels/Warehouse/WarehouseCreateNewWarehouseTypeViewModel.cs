using AppNov14.Web.ViewModels.Base;
using AppNov14.Web.ViewModels.Base.BaseOldManfAndLab;
using System;

namespace AppNov14.Web.ViewModels.Warehouse
{
    public sealed class WarehouseCreateNewWarehouseTypeViewModel : BaseViewModel
    {
        public WarehouseCreateNewWarehouseTypeForm Form { get; set; }
    }

    public sealed class WarehouseCreateNewWarehouseTypeForm : BaseWarehouseParamsRequiredForm
    {
    }
}