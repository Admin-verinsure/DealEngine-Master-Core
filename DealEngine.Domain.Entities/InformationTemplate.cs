using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class InformationTemplate : EntityBase, IAggregateRoot
    {
        protected InformationTemplate()
			: base (null)
        {
            _sections = new List<InformationSection>();
        }

        public InformationTemplate(User createdBy, string name, IList<InformationSection> sections = null)
			: base (createdBy)
        {
            Name = name;

            if (sections == null)
                _sections = new List<InformationSection>();
            else
                _sections = sections;
        }

        IList<InformationSection> _sections;

        public virtual string Name { get; protected set; }

        public virtual IList<InformationSection> Sections {  get { return _sections; } set { _sections = value; }   }

		public virtual Product Product { get; set; }

        public virtual IList<InformationSection> AddSection(InformationSection section)
        {
            _sections.Add(section);

            return _sections;
        }
    }

    public class SubInformationTemplate : InformationTemplate
    {
        public virtual InformationTemplate BaseInformationTemplate { get; set; }
        public SubInformationTemplate() { }
    }

}
