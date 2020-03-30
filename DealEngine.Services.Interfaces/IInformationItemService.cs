using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DealEngine.Services.Interfaces
{
    public interface IInformationItemService
    {
        Task<InformationItem> CreateLabelItem(User createdBy, string name, string label, int width, string itemType);
        Task<InformationItem> CreateTextboxItem (User createdBy, string name, string label, int width, string itemType);
        Task<InformationItem> CreateTextAreaItem (User createdBy, string name, string label, int width, string itemType);
        Task<InformationItem> CreateJSButtonItem (User createdBy, string name, string label, int width, string itemType, string onclickValue);
        Task<InformationItem> CreateSubmitButtonItem (User createdBy, string name, string label, int width, string itemType);
        Task<InformationItem> CreateDropdownListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType);
        Task<InformationItem> CreateItemFromForm(IFormCollection form, User user, string title);
        Task<InformationItem> GetItemById(Guid guid);
        Task UpdateItem(InformationItem item);
    }

    
}
