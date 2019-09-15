using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class InformationSection : EntityBase, IAggregateRoot
    {
        private IList<InformationItem> _items;

		public virtual string Name { get; protected set; }

		public virtual string CustomView { get; set; }

		public virtual int Position { get; set; }

		public virtual InformationTemplate InformationTemplate { get; set; }

		public virtual IList<InformationItem> Items {
			get { return _items; }
			set { _items = value; }
		}

        protected InformationSection()
			: base (null)
        {
            _items = new List<InformationItem>();
        }

        public InformationSection(User createdBy, string name, IList<InformationItem> items = null)
			: base (createdBy)
        {
            Name = name;

            if (items == null)
                _items = new List<InformationItem>();
            else
                _items = items;
        }

        public virtual IList<InformationItem> AddItem(InformationItem item)
        {
            _items.Add(item);

            return _items;
        }


    }
}
