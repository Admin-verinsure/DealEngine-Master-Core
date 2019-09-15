using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class InsuranceElementType : ValueObject
    {
        protected InsuranceElementType()
        {
        }

        public InsuranceElementType(string type)
        {
            this.Type = type;
        }

        public virtual string Type { get; private set; }
    }
}
