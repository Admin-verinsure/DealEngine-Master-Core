using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class InsuranceElementLabel : EntityBase
    {
        protected InsuranceElementLabel() : base (null) { }

        public InsuranceElementLabel(User createdBy, string culture, string value)
			: base (createdBy)
        {
            Culture = culture;
            Value = value;
        }
            
        public virtual string Culture { get; set; }
        
        public virtual string Value { get; set; }
                
    }
}
