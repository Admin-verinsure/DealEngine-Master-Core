using System.Collections.Generic;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Domain.Services.Factories;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class InformationItemService : IInformationItemService
    {
        InformationItemFactory _informationItemFactory;

        IMapperSession<DropdownListItem> _dropdownListItemRepository;
        IMapperSession<TextboxItem> _textboxItemRepository;
        IMapperSession<LabelItem> _labelItemRepository;
		IMapperSession<TextAreaItem> _textareaItemRepository;
		IMapperSession<JSButtonItem> _jsButtonRepository;
		IMapperSession<SubmitButtonItem> _submitButtonRepository;
		IMapperSession<SectionBreakItem> _sectionBreakItemRepository;
		IMapperSession<MotorVehicleListItem> _motorVehicleListItemRepository;


        public InformationItemService(
            InformationItemFactory informationItemFactory,
            IMapperSession<DropdownListItem> dropdownListItemRepository,
            IMapperSession<TextboxItem> textboxItemRepository,
            IMapperSession<LabelItem> labelItemRepository,
			IMapperSession<TextAreaItem> textareaItemRepository,
			IMapperSession<JSButtonItem> jsButtonRepository,
			IMapperSession<SubmitButtonItem> submitButtonRepository,
			IMapperSession<SectionBreakItem> sectionBreakItemRepository,
			IMapperSession<MotorVehicleListItem> motorVehicleListItemRepository)
        {
            _informationItemFactory = informationItemFactory;
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
            DropdownListItem infoItem = _informationItemFactory.CreateDropdownListItem(createdBy, name, label, width, itemType, defaultText, options ) as DropdownListItem;
            
            _dropdownListItemRepository.Add(infoItem);

            //infoItem.AddItems(options);

            //_dropdownListItemRepository.Add(infoItem);

            return infoItem;
        }

        public InformationItem CreateLabelItem(User createdBy, string name, string label, int width, string itemType)
        {
            InformationItem infoItem = _informationItemFactory.CreateLabelItem(createdBy, name, label, width, itemType);
            _labelItemRepository.Add(infoItem as LabelItem);
            return infoItem;
        }

        public InformationItem CreateTextboxItem(User createdBy, string name, string label, int width, string itemType)
        {
            InformationItem infoItem = _informationItemFactory.CreateTextboxItem(createdBy, name, label, width, itemType);
            _textboxItemRepository.Add(infoItem as TextboxItem);
            return infoItem;
        }

		public InformationItem CreateMultiselectListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType)
		{
			MultiselectListItem infoItem = _informationItemFactory.CreateMultiselectListItem (createdBy, name, label, width, itemType, defaultText, options) as MultiselectListItem;
			_dropdownListItemRepository.Add (infoItem);
			return infoItem;
		}

		public InformationItem CreateTextAreaItem (User createdBy, string name, string label, int width, string itemType)
		{
			InformationItem infoItem = _informationItemFactory.CreateTextAreaItem (createdBy, name, label, width, itemType);
			_textareaItemRepository.Add (infoItem as TextAreaItem);
			return infoItem;
		}

		public InformationItem CreateJSButtonItem (User createdBy, string name, string label, int width, string itemType, string onclickValue)
		{
			InformationItem infoItem = _informationItemFactory.CreateJSButtonItem (createdBy, name, label, width, itemType, onclickValue);
			_jsButtonRepository.Add (infoItem as JSButtonItem);
			return infoItem;
		}

		public InformationItem CreateSubmitButtonItem (User createdBy, string name, string label, int width, string itemType)
		{
			InformationItem infoItem = _informationItemFactory.CreateSubmitButtonItem (createdBy, name, label, width, itemType);
			_submitButtonRepository.Add (infoItem as SubmitButtonItem);
			return infoItem;
		}

		public InformationItem CreateSectionBreakItem (User createdBy, string itemType)
		{
			InformationItem infoItem = _informationItemFactory.CreateSectionBreakItem (createdBy, itemType);
			_sectionBreakItemRepository.Add (infoItem as SectionBreakItem);
			return infoItem;
		}

		public InformationItem CreateMotorVehicleListItem (User createdBy, string name, string label, int width, string itemType)
		{
			InformationItem infoItem = _informationItemFactory.CreateMotorVehicleListItem (createdBy, name, label, width, itemType);
			_motorVehicleListItemRepository.Add (infoItem as MotorVehicleListItem);
			return infoItem;
		}
    }
}
