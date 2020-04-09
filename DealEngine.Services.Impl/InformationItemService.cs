using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DealEngine.Services.Impl
{
    public class InformationItemService : IInformationItemService
    {
        //      IMapperSession<DropdownListItem> _dropdownListItemRepository;
        //      IMapperSession<TextboxItem> _textboxItemRepository;
        //      IMapperSession<LabelItem> _labelItemRepository;
        //IMapperSession<TextAreaItem> _textareaItemRepository;
        //IMapperSession<JSButtonItem> _jsButtonRepository;
        //IMapperSession<SubmitButtonItem> _submitButtonRepository;
        //IMapperSession<SectionBreakItem> _sectionBreakItemRepository;
        IMapperSession<InformationItem> _informationItemRepository;


        public InformationItemService(
           IMapperSession<InformationItem> informationItemRepository)
        {
            _informationItemRepository = informationItemRepository;
        }

        public async Task<InformationItem> CreateDropdownListItem(User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType)
        {
            DropdownListItem infoItem = new DropdownListItem(createdBy, name, label, "", width, itemType, options, defaultText); //InformationItem.CreateDropdownListItem(createdBy, name, label, width, itemType, defaultText, options ) as DropdownListItem;
            await _informationItemRepository.AddAsync(infoItem);

            return infoItem;
        }

        public async Task<InformationItem> CreateLabelItem(User createdBy, string name, string label, int width, string itemType)
        {
            InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType); //InformationItem.CreateLabelItem(createdBy, name, label, width, itemType);
            await _informationItemRepository.AddAsync(infoItem as LabelItem);
            return infoItem;
        }

        public async Task<InformationItem> CreateTextboxItem(User createdBy, string name, string label, int width, string itemType)
        {
            var infoItem = new TextboxItem(createdBy, name, label, "", width, itemType); // InformationItem.CreateTextboxItem(createdBy, name, label, width, itemType);         
            await _informationItemRepository.AddAsync(infoItem);
            return (InformationItem)infoItem;
        }

		public async Task<InformationItem> CreateMultiselectListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType)
		{
            MultiselectListItem infoItem = new MultiselectListItem(createdBy, name, label, "", width, itemType, defaultText, options); // InformationItem.CreateMultiselectListItem (createdBy, name, label, width, itemType, defaultText, options) as MultiselectListItem;
            await _informationItemRepository.AddAsync(infoItem);
			return infoItem;
		}

		public async Task<InformationItem> CreateTextAreaItem (User createdBy, string name, string label, int width, string itemType)
		{
            InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType); // InformationItem.CreateTextAreaItem (createdBy, name, label, width, itemType);
            await _informationItemRepository.AddAsync(infoItem as TextAreaItem);
			return infoItem;
		}

		public async Task<InformationItem> CreateJSButtonItem (User createdBy, string name, string label, int width, string itemType, string onclickValue)
		{
			InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType);
            await _informationItemRepository.AddAsync(infoItem as JSButtonItem);
			return infoItem;
		}

		public async Task<InformationItem> CreateSubmitButtonItem (User createdBy, string name, string label, int width, string itemType)
		{
			InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType);
            await _informationItemRepository.AddAsync(infoItem as SubmitButtonItem);
			return infoItem;
		}

        public async Task<InformationItem> CreateItem(string title, User user, string questiontype, string question, List<string> options)
        {
            InformationItem item;
            List<DropdownListOption> ddOptions = new List<DropdownListOption>();
            string randomName = System.IO.Path.GetRandomFileName().Replace(".", "");
            switch (questiontype)
            {
                case "textTemplate":
                    item = await CreateTextboxItem(user, randomName, question, 10, "TEXTBOX");
                    break;
                case "yesNoTemplate":
                    ddOptions.Add(new DropdownListOption(user, "-- Select --", ""));
                    ddOptions.Add(new DropdownListOption(user, "Yes", "0"));
                    ddOptions.Add(new DropdownListOption(user, "No", "1"));
                    item = await CreateDropdownListItem(user, randomName, question, title, ddOptions, 10, "DROPDOWNLIST");
                    break;
                case "dropdownTemplate":
                    ddOptions.Add(new DropdownListOption(user, "-- Select --", ""));
                    for (int j = 0; j < options.Count; j++)
                    {
                        if (!string.IsNullOrWhiteSpace(options.ElementAt(j)))
                        {
                            ddOptions.Add(new DropdownListOption(user, options.ElementAt(j), j.ToString()));
                        }
                    }
                    item = await CreateDropdownListItem(user, randomName, question, title, ddOptions, 10, "DROPDOWNLIST");
                    break;
                default:
                    throw new Exception("Unable to map element (" + questiontype + ")");
            }
            return item;
        }

        public async Task<InformationItem> CreateItemFromForm(IFormCollection form, User user, string title)
        {
            InformationItem item;            
            var questiontype = form["questiontype"];
            var question = form["question"];
            var options = form["dropdownvalue"].ToList();
            item = await CreateItem(title, user, questiontype, question, options);
            try
            {
                var on = form["questionreferunderwriting"];
                item.ReferUnderwriting = true;
            }
            catch (Exception ex)
            {
                item.ReferUnderwriting = false;
            }
            try
            {
                var on = form["Required"];
                item.Required = true;
            }
            catch (Exception ex)
            {
                item.Required = false;
            }
            try
            {
                var on = form["conditional"];
                questiontype = form["conditionalquestiontype"];
                question = form["conditionalquestion"];
                options = form["conditionaldropdownvalue"].ToList();
                var conditionalquestion = await CreateItem(title, user, questiontype, question, options);
                item.ConditionalList.Add(conditionalquestion);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return item;
        }

        public async Task<InformationItem> GetItemById(Guid guid)
        {
            InformationItem informationItem;
            return await _informationItemRepository.GetByIdAsync(guid);
        }

        public async Task UpdateItem(InformationItem item)
        {
            await _informationItemRepository.UpdateAsync(item);
        }

        //public async Task<InformationItem> CreateSectionBreakItem(User createdBy, string itemType)
        //{
        //    //InformationItem infoItem = _informationItemFactory.CreateSectionBreakItem(createdBy, itemType);
        //    //await _sectionBreakItemRepository.AddAsync(infoItem as SectionBreakItem);
        //    return "";
        //}
    }
}
