using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class DropdownListOption : EntityBase
    {
        protected DropdownListOption() : base (null) { }

        public DropdownListOption(User createdBy, string text, string value)
			: base (createdBy)
        {
            Text = text;
            Value = value;
        }

        public virtual string Text { get; protected set; }

        public virtual string Value { get; protected set; }
    }
}
