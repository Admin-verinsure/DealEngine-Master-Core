using System;

namespace DealEngine.WebUI.Models
{
    public class InformationSectionBuilderViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public String TextBox { get; set; }
        public String IsChange { get; set; }

        public String DropDown { get; set; }

        public String Yes { get; set; }

       
    }

}