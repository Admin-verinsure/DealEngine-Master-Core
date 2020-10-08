using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;
using Newtonsoft.Json;

namespace DealEngine.Domain.Entities
{
    [JsonObject]
    public class InformationSection : EntityBase, IAggregateRoot
    {

		public virtual string Name { get; set; }

		public virtual string CustomView { get; set; }

		public virtual int Position { get; set; }
        [JsonIgnore]
		public virtual InformationTemplate InformationTemplate { get; set; }

		public virtual IList<InformationItem> Items { get; set; }

        protected InformationSection()
			: base (null)
        {
            Items = new List<InformationItem>();
        }

        public InformationSection(User createdBy, string name, IList<InformationItem> items = null)
			: base (createdBy)
        {
            Name = name;

            if (items == null)
                Items = new List<InformationItem>();
            else
                Items = items;
        }

        public virtual IList<InformationItem> AddItem(InformationItem item)
        {
            Items.Add(item);

            return Items;
        }


    }
}
