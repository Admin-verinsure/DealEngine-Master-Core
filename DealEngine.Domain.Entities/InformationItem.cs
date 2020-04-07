using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;


namespace DealEngine.Domain.Entities
{
	public class InformationItem : EntityBase, IAggregateRoot
	{
		protected InformationItem() : base(null) { }

		public InformationItem(User createdBy, string name, string label, string controlType, int width, string itemType)
			: base(createdBy)
		{
			Name = name;
			Label = label;
			Width = width;
			Type = itemType;

			DropdownList = new List<DropdownListItem>();
			ConditionalList = new List<InformationItem>();
		}


		public virtual string Name { get; protected set; }
		public virtual string ControlType { get; set; }

		public virtual string Label { get; protected set; }

		public virtual int Width { get; protected set; }

		public virtual string Type { get; set; }

		public virtual int ItemOrder { get; set; }

		public virtual bool NeedsReview { get; set; }

		public virtual bool NeedsMilestone { get; set; }

		public virtual bool ReferUnderwriting { get; set; }

		public virtual bool Required { get; set; }
		public virtual Rule Rule {get; set;}		
		public virtual IList<InformationItem> ConditionalList { get; set; }
        public virtual IList<DropdownListItem> DropdownList { get; set; }

		// Add locality later

	}

    public class DropdownListItem : InformationItem
	{
		public DropdownListItem ()
		{
			options = new List<DropdownListOption> ();
		}

		public DropdownListItem (User createdBy, string name, string label, string controlType, int width, string itemType, IList<DropdownListOption> options, string defaultText = "")
			: base (createdBy, name, label, controlType, width, itemType)
		{		

			if (options == null)
				this.options = new List<DropdownListOption> ();
			else
				this.options = options;
		}

		private IList<DropdownListOption> options;

		public virtual string DefaultText { get; set; }

		public virtual IList<DropdownListOption> Options {
			get { return options; }
			protected set { options = value; }
		}

		public virtual IList<DropdownListOption> AddItems (IList<DropdownListOption> items)
		{
			List<DropdownListOption> newList = options.ToList ();

			newList.AddRange (items);

			options = newList;

			return options;
		}

		public virtual IList<DropdownListOption> AddItem (DropdownListOption option)
		{
			options.Add (option);
			return options;
		}

		public virtual IList<DropdownListOption> RemoveItem (DropdownListOption option)
		{
			options.Remove (option);
			return options;
		}
	}

    public class TextboxItem : InformationItem
    {
        protected TextboxItem()
        {

        }

        public TextboxItem(User createdBy, string name, string label,string controlType, int width, string itemType) :
            base(createdBy, name, label, controlType, width, itemType)
        {

        }
    }

    public class LabelItem : InformationItem
    {
        protected LabelItem()
        {

        }

        public LabelItem(User createdBy, string name, string label, string id ,int width, string itemType) : 
            base(createdBy, name, label, id, width,  itemType)
        {

        }
    }

	public class TextAreaItem : InformationItem
	{
		protected TextAreaItem ()
		{

		}

		public TextAreaItem (User createdBy, string name, string label, string id, int width, string itemType) :
            base(createdBy, name, label, id, width, itemType)
        {

		}
	}

	public class MultiselectListItem : DropdownListItem
	{
		protected MultiselectListItem ()
		{

		}

		public MultiselectListItem (User createdBy, string name, string label, string id, int width, string itemType, string defaultText = "", IList<DropdownListOption> options = null)
			: base(createdBy, name, label, id,  width, itemType, options, defaultText )
		{

		}
	}

	public class JSButtonItem : InformationItem
	{
		public virtual string Value { get; set; }

		protected JSButtonItem ()
		{

		}

		public JSButtonItem (User createdBy, string name, string label,string id, int width, string itemType, string onclickValue)
			: base (createdBy, name, label,id, width, itemType)
		{
			Value = onclickValue;
		}
	}

	public class SubmitButtonItem : InformationItem
	{
		protected SubmitButtonItem ()
		{

		}

		public SubmitButtonItem (User createdBy, string name, string label,string id, int width, string itemType)
			: base (createdBy, name, label, id, width, itemType)
		{

		}
	}

	public class SectionBreakItem : InformationItem
	{
		protected SectionBreakItem ()
		{

		}

		public SectionBreakItem (User createdBy, string itemType)
			: base (createdBy, "", "","",0, itemType)
		{

		}
	}

	public class MotorVehicleListItem : InformationItem
	{
		public virtual IList<Vehicle> Vehicles {
			get;
			protected set;
		}

		protected MotorVehicleListItem ()
		{
			Vehicles = new List<Vehicle> ();
		}

		public MotorVehicleListItem (User createdBy, string name, string label, string id , int width, string itemType)
			: base(createdBy, name, label, id, width, itemType)
		{

		}

		public virtual void Add (Vehicle vehicle)
		{
			Vehicles.Add (vehicle);
		}

		public virtual void Remove (Vehicle vehicle)
		{
			Vehicles.Remove (vehicle);
		}
	}

}
