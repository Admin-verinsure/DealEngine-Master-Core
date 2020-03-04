using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class Cover : EntityBase
    {
        private string name;
        private string description;
        private string code;
        private bool enableRetroactiveDate;

        protected Cover() : base (null) { }

        public Cover(User createdBy, string name, string description, string code, bool enableRetroactiveDate)
			: base (createdBy)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Can not create cover without cover name.");

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException("Can not create cover without cover code.");

            this.name = name;
            this.description = description;
            this.code = code;
            this.enableRetroactiveDate = enableRetroactiveDate;
        }

        public virtual string Name
        {
            get { return name; }
        }

        public virtual string Description
        {
            get { return description; }
        }

        public virtual string Code
        {
            get { return code; }
        }

        public virtual bool EnableRetroactiveDate
        {
            get { return enableRetroactiveDate; }
        }
    }
}
