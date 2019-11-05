using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInformationItemService
    {
        // General Itemsl
        Task<InformationItem> CreateLabelItem(User createdBy, string name, string label, int width, string itemType);
        //Task<InformationItem> CreateSectionBreakItem (User createdBy, string itemType);
        Task<InformationItem> CreateTextboxItem (User createdBy, string name, string label, int width, string itemType);
        Task<InformationItem> CreateTextAreaItem (User createdBy, string name, string label, int width, string itemType);

        // Button Itemss
        Task<InformationItem> CreateJSButtonItem (User createdBy, string name, string label, int width, string itemType, string onclickValue);
        Task<InformationItem> CreateSubmitButtonItem (User createdBy, string name, string label, int width, string itemType);

        // List Items
        Task<InformationItem> CreateDropdownListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType);
        Task<InformationItem> CreateMotorVehicleListItem (User createdBy, string name, string label, int width, string itemType);
        Task<InformationItem> CreateMultiselectListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType);
    }

    
}
