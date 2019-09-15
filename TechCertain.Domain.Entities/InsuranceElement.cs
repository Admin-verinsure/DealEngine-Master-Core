using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class InsuranceElement : EntityBase
    {
        IList<InsuranceElementLabel> insuranceElementLabels = new List<InsuranceElementLabel>();

        protected InsuranceElement() : base (null) { }

        public InsuranceElement(User createdBy, string name)
			: base (createdBy)
        {
            Name = name;
        }

        public virtual string Name { get; protected set; }

        public virtual string DataType { get; set; }

        public virtual InsuranceElementType Type { get; set; }

        public virtual int DisplayOrder { get; set; }

        public virtual InsuranceElement Parent {get; set;}

        public virtual IEnumerable<InsuranceElementSelection> Selection { get; set; }

        public virtual IEnumerable<InsuranceElementLabel> InsuranceElementLabels { get { return insuranceElementLabels; } }

        public virtual void AddLabel(InsuranceElementLabel label) 
        {
            if (label == null)
				throw new ArgumentNullException(nameof(label)); //TODO - Finish

            insuranceElementLabels.Add(label);
        }

       // public virtual Product 
    }
}
