using System;
using AppNov14.Web.ViewModels.Base;

namespace AppNov14.Web.ViewModels.ManufacturingTable
{
    public sealed class RestoreIndexDatesViewModel : BaseViewModel
    {
        public RestoreIndexDatesForm Form { get; set; }

        public string Name { get; set; }
    }

    public sealed class RestoreIndexDatesForm : BaseForm
    {
        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int Id { get; set; }
    }
}