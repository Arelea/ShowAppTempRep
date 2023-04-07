using System;
using System.ComponentModel.DataAnnotations;

namespace AppNov14.Web.ViewModels.Base.BaseOldManfAndLab
{
    public class BaseWarehouseParamsViewModel : BaseViewModel
    {

    }

    public class BaseWarehouseParamsForm : BaseForm
    {
        public virtual string Type { get; set; }

        public virtual string SubType { get; set; }

        public virtual string Provider { get; set; }

        public virtual string Manufacturer { get; set; }
    }

    public class BaseWarehouseParamsRequiredForm : BaseForm
    {
        [Required]
        public virtual string Type { get; set; }

        [Required]
        public virtual string SubType { get; set; }

        [Required]
        public virtual string Provider { get; set; }

        [Required]
        public virtual string Manufacturer { get; set; }
    }
}