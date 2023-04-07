using AppNov14.Web.ViewModels.Base;
using System;

namespace AppNov14.Web.ViewModels.Warehouse
{
    public sealed class WarehouseLabViewAllViewModel : WarehouseViewAllViewModel
    {
        public WarehouseLabViewAllForm Form { get; set; }
    }

    public sealed class WarehouseLabViewAllForm : BaseForm
    {
        public bool Mode { get; set; }
    }
}