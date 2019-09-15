using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechCertain.Domain.Entities;

namespace techcertain2015rebuildcore.Models.ViewModels
{
    public class InformationBuilderViewModel : BaseViewModel
    {
        public Guid? Id { get; set; }
        //public IEnumerable<InformationItem> SectionItems { get; set; }
        public IEnumerable<InformationTemplate> InformationTemplates { get; set; }
        
        public InformationTemplate InformationTemplate { get; set; }

        public IEnumerable<InformationSectionViewModel> Sections { get; set; }

        public string Name { get; set; }
    }
}