using System.Collections.Generic;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class InformationItemService : IInformationItemService
    {      
        IMapperSession<DropdownListItem> _dropdownListItemRepository;
        IMapperSession<TextboxItem> _textboxItemRepository;
        IMapperSession<LabelItem> _labelItemRepository;
		IMapperSession<TextAreaItem> _textareaItemRepository;
		IMapperSession<JSButtonItem> _jsButtonRepository;
		IMapperSession<SubmitButtonItem> _submitButtonRepository;
		IMapperSession<SectionBreakItem> _sectionBreakItemRepository;
		IMapperSession<MotorVehicleListItem> _motorVehicleListItemRepository;


        public InformationItemService(            
            IMapperSession<DropdownListItem> dropdownListItemRepository,
            IMapperSession<TextboxItem> textboxItemRepository,
            IMapperSession<LabelItem> labelItemRepository,
			IMapperSession<TextAreaItem> textareaItemRepository,
			IMapperSession<JSButtonItem> jsButtonRepository,
			IMapperSession<SubmitButtonItem> submitButtonRepository,
			IMapperSession<SectionBreakItem> sectionBreakItemRepository,
			IMapperSession<MotorVehicleListItem> motorVehicleListItemRepository)
        {            
            _dropdownListItemRepository = dropdownListItemRepository;
            _textboxItemRepository = textboxItemRepository;
            _labelItemRepository = labelItemRepository;
			_textareaItemRepository = textareaItemRepository;
			_jsButtonRepository = jsButtonRepository;
			_submitButtonRepository = submitButtonRepository;
			_sectionBreakItemRepository = sectionBreakItemRepository;
			_motorVehicleListItemRepository = motorVehicleListItemRepository;
        }

        public InformationItem CreateDropdownListItem(User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType)
        {
            DropdownListItem infoItem = new DropdownListItem(createdBy, name, label, width, itemType, options, defaultText); //InformationItem.CreateDropdownListItem(createdBy, name, label, width, itemType, defaultText, options ) as DropdownListItem;
            _dropdownListItemRepository.AddAsync(infoItem);

            return infoItem;
        }

        public InformationItem CreateLabelItem(User createdBy, string name, string label, int width, string itemType)
        {
            InformationItem infoItem = new InformationItem(createdBy, name, label, width, itemType); //InformationItem.CreateLabelItem(createdBy, name, label, width, itemType);
            _labelItemRepository.AddAsync(infoItem as LabelItem);
            return infoItem;
        }

        public InformationItem CreateTextboxItem(User createdBy, string name, string label, int width, string itemType)
        {
            InformationItem infoItem = new InformationItem(createdBy, name, label, width, itemType); // InformationItem.CreateTextboxItem(createdBy, name, label, width, itemType);
            _textboxItemRepository.AddAsync(infoItem as TextboxItem);
            return infoItem;
        }

		public InformationItem CreateMultiselectListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType)
		{
            MultiselectListItem infoItem = new MultiselectListItem(createdBy, name, label, width, itemType, defaultText, options); // InformationItem.CreateMultiselectListItem (createdBy, name, label, width, itemType, defaultText, options) as MultiselectListItem;
			_dropdownListItemRepository.AddAsync(infoItem);
			return infoItem;
		}

		public InformationItem CreateTextAreaItem (User createdBy, string name, string label, int width, string itemType)
		{
            InformationItem infoItem = new InformationItem(createdBy, name, label, width, itemType); // InformationItem.CreateTextAreaItem (createdBy, name, label, width, itemType);
			_textareaItemRepository.AddAsync(infoItem as TextAreaItem);
			return infoItem;
		}

		public InformationItem CreateJSButtonItem (User createdBy, string name, string label, int width, string itemType, string onclickValue)
		{
			InformationItem infoItem = new InformationItem(createdBy, name, label, width, itemType);
			_jsButtonRepository.AddAsync(infoItem as JSButtonItem);
			return infoItem;
		}

		public InformationItem CreateSubmitButtonItem (User createdBy, string name, string label, int width, string itemType)
		{
			InformationItem infoItem = new InformationItem(createdBy, name, label, width, itemType);
			_submitButtonRepository.AddAsync(infoItem as SubmitButtonItem);
			return infoItem;
		}

		public InformationItem CreateSectionBreakItem (User createdBy, string itemType)
		{
			InformationItem infoItem = new InformationItem(createdBy, itemType, "", 0, "");
			_sectionBreakItemRepository.AddAsync(infoItem as SectionBreakItem);
			return infoItem;
		}

		public InformationItem CreateMotorVehicleListItem (User createdBy, string name, string label, int width, string itemType)
		{
			InformationItem infoItem = new InformationItem(createdBy, name, label, width, itemType);
			_motorVehicleListItemRepository.AddAsync(infoItem as MotorVehicleListItem);
			return infoItem;
		}
    }
}
