using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInformationItemService
    {
		// General Itemsl
		InformationItem CreateLabelItem (User createdBy, string name, string label, int width, string itemType);
		InformationItem CreateSectionBreakItem (User createdBy, string itemType);
		InformationItem CreateTextboxItem (User createdBy, string name, string label, int width, string itemType);
		InformationItem CreateTextAreaItem (User createdBy, string name, string label, int width, string itemType);

		// Button Itemss
		InformationItem CreateJSButtonItem (User createdBy, string name, string label, int width, string itemType, string onclickValue);
		InformationItem CreateSubmitButtonItem (User createdBy, string name, string label, int width, string itemType);

		// List Items
		InformationItem CreateDropdownListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType);
		InformationItem CreateMotorVehicleListItem (User createdBy, string name, string label, int width, string itemType);
		InformationItem CreateMultiselectListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType);
    }

    
}
