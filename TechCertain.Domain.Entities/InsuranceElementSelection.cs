using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class InsuranceElementSelection : EntityBase
    {
        protected InsuranceElementSelection() : base (null) { }

		public InsuranceElementSelection (User createdBy) : base (createdBy) { }

        public virtual InsuranceElement InsuranceElement { get; set; }

        public virtual string Text { get; set; }

        public virtual string Value { get; set; }

        public virtual int DisplayOrder { get; set; }
    }
}