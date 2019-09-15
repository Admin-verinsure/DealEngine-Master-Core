using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class SubTermType : EntityBase
    {
        protected SubTermType () : base (null) { }

		public SubTermType (User createdBy) : base (createdBy) { }

        public virtual string Name { get; set; }

    }
}
