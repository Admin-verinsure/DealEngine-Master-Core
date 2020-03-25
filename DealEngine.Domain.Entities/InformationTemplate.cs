using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class InformationTemplate : EntityBase, IAggregateRoot
    {
        protected InformationTemplate()
			: base (null)
        {
            Sections = new List<InformationSection>();
        }

        public InformationTemplate(User createdBy, string name, IList<InformationSection> sections = null)
			: base (createdBy)
        {
            Name = name;

            if (sections == null)
                Sections = new List<InformationSection>();
            else
                Sections = sections;
        }

        public virtual string Name { get; set; }

        public virtual IList<InformationSection> Sections {  get; set; }

		public virtual Product Product { get; set; }

        public virtual IList<InformationSection> AddSection(InformationSection section)
        {
            Sections.Add(section);

            return Sections;
        }
    }

    public class SubInformationTemplate : InformationTemplate
    {
        public virtual InformationTemplate BaseInformationTemplate { get; set; }
        public SubInformationTemplate() { }
    }

}
