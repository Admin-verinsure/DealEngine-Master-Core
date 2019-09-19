using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace techcertain2019core.Models.ViewModels
{
    public class LabelPersentageViewModel : BaseViewModel
    {
        public string Label { get; set; }

        public decimal Percentage { get; set; }
    }
}