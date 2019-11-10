using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<InformationItem> CreateDropdownListItem(User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType)
        {
            DropdownListItem infoItem = new DropdownListItem(createdBy, name, label,"", width, itemType, options, defaultText); //InformationItem.CreateDropdownListItem(createdBy, name, label, width, itemType, defaultText, options ) as DropdownListItem;
            await _dropdownListItemRepository.AddAsync(infoItem);

            return infoItem;
        }

        public async Task<InformationItem> CreateLabelItem(User createdBy, string name, string label, int width, string itemType)
        {
            InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType); //InformationItem.CreateLabelItem(createdBy, name, label, width, itemType);
            await _labelItemRepository.AddAsync(infoItem as LabelItem);
            return infoItem;
        }

        public async Task<InformationItem> CreateTextboxItem(User createdBy, string name, string label, int width, string itemType)
        {
            InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType); // InformationItem.CreateTextboxItem(createdBy, name, label, width, itemType);
            await _textboxItemRepository.AddAsync(infoItem as TextboxItem);
            return infoItem;
        }

		public async Task<InformationItem> CreateMultiselectListItem (User createdBy, string name, string label, string defaultText, IList<DropdownListOption> options, int width, string itemType)
		{
            MultiselectListItem infoItem = new MultiselectListItem(createdBy, name, label, "", width, itemType, defaultText, options); // InformationItem.CreateMultiselectListItem (createdBy, name, label, width, itemType, defaultText, options) as MultiselectListItem;
            await _dropdownListItemRepository.AddAsync(infoItem);
			return infoItem;
		}

		public async Task<InformationItem> CreateTextAreaItem (User createdBy, string name, string label, int width, string itemType)
		{
            InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType); // InformationItem.CreateTextAreaItem (createdBy, name, label, width, itemType);
            await _textareaItemRepository.AddAsync(infoItem as TextAreaItem);
			return infoItem;
		}

		public async Task<InformationItem> CreateJSButtonItem (User createdBy, string name, string label, int width, string itemType, string onclickValue)
		{
			InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType);
            await _jsButtonRepository.AddAsync(infoItem as JSButtonItem);
			return infoItem;
		}

		public async Task<InformationItem> CreateSubmitButtonItem (User createdBy, string name, string label, int width, string itemType)
		{
			InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType);
            await _submitButtonRepository.AddAsync(infoItem as SubmitButtonItem);
			return infoItem;
		}

        //public async Task<InformationItem> CreateSectionBreakItem(User createdBy, string itemType)
        //{
        //    //InformationItem infoItem = _informationItemFactory.CreateSectionBreakItem(createdBy, itemType);
        //    //await _sectionBreakItemRepository.AddAsync(infoItem as SectionBreakItem);
        //    return "";
        //}

        public async Task<InformationItem> CreateMotorVehicleListItem (User createdBy, string name, string label, int width, string itemType)
		{
			InformationItem infoItem = new InformationItem(createdBy, name, label, "", width, itemType);
            await _motorVehicleListItemRepository.AddAsync(infoItem as MotorVehicleListItem);
			return infoItem;
		}
    }
}
