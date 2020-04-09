using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class InformationBuilderViewModel : BaseViewModel
    {
        public Guid? Id { get; set; }
        //public IEnumerable<InformationItem> SectionItems { get; set; }
        public IEnumerable<InformationTemplate> InformationTemplates { get; set; }
        public InformationTemplate InformationTemplate { get; set; }
        public IEnumerable<InformationSectionViewModel> Sections { get; set; }
        public IEnumerable<Rule> Rules { get; set; }
        public string Name { get; set; }
    }
}