using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Domain.Services.Factories
{
    public class InformationItemFactory : IEntityFactory
    {
        public InformationItem CreateDropdownListItem(User createdBy, string name, string label, int width, string itemType, string defaultText = "", IList<DropdownListOption> options = null)
        {
            return new DropdownListItem(createdBy, name, label, width, itemType, options, defaultText);
        }

        public InformationItem CreateTextboxItem(User createdBy, string name, string label, int width, string itemType)
        {
            return new TextboxItem(createdBy, name, label, width, itemType);
        }

        public InformationItem CreateLabelItem(User createdBy, string name, string label, int width, string itemType)
        {
            return new LabelItem(createdBy, name, label, width, itemType);
        }

		public InformationItem CreateTextAreaItem (User createdBy, string name, string label, int width, string itemType)
		{
			return new TextAreaItem (createdBy, name, label, width, itemType);
		}

		public InformationItem CreateMultiselectListItem (User createdBy, string name, string label, int width, string itemType, string defaultText = "", IList<DropdownListOption> options = null)
		{
			return new MultiselectListItem (createdBy, name, label, width, itemType, defaultText, options);
		}

		public InformationItem CreateJSButtonItem (User createdBy, string name, string label, int width, string itemType, string onclickValue)
		{
			return new JSButtonItem (createdBy, name, label, width, itemType, onclickValue);
		}

		public InformationItem CreateSubmitButtonItem (User createdBy, string name, string label, int width, string itemType)
		{
			return new SubmitButtonItem (createdBy, name, label, width, itemType);
		}

		public InformationItem CreateSectionBreakItem (User createdBy, string itemType)
		{
			return new SectionBreakItem (createdBy, itemType);
		}

		public InformationItem CreateMotorVehicleListItem (User createdBy, string name, string label, int width, string itemType)
		{
			return new MotorVehicleListItem (createdBy, name, label, width, itemType);
		}
    }
}
