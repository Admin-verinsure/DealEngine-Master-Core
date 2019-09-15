using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using techcertain2015rebuildcore.Models.ViewModels;

namespace techcertain2015rebuildcore.Controllers
{
    public class InformationDemoController : BaseController
    {
		IMapper _mapper;
		IUnitOfWorkFactory _unitOfWorkFactory;
		IInformationTemplateService _informationTemplateService;

		public InformationDemoController (IUserService userService, IInformationTemplateService informationTemplateService, IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
			: base(userService)
		{
			_informationTemplateService = informationTemplateService;
			_unitOfWorkFactory = unitOfWorkFactory;
			_mapper = mapper;
		}

		[HttpGet]
        public ActionResult Create(string id)
        {
			InformationViewModel model = null;

			if  (id.StartsWith ("icib", StringComparison.CurrentCulture)) {
				if (id.EndsWith ("md", StringComparison.CurrentCulture))
					model = GetIcibMDCover ();

			}

			List<InformationSection> sections = new List<InformationSection> ();

			foreach (var section in model.Sections) {
				List<InformationItem> items = new List<InformationItem> ();

				foreach (var item in section.Items) {
					if (item != null) {
						string itemTypeName = Enum.GetName (typeof (ItemType), item.Type);

						switch (item.Type) {
						case ItemType.TEXTAREA:
						case ItemType.TEXTBOX:
							var textboxItem = new TextboxItem (CurrentUser, item.Name, item.Label, item.Width, itemTypeName);
							items.Add (textboxItem);
							item.Id = textboxItem.Id;
							break;
						case ItemType.DROPDOWNLIST:
							var options = _mapper.Map<IList<DropdownListOption>> (item.Options);
							var newDropdownList = new DropdownListItem (CurrentUser, item.Name, item.Label, item.Width, itemTypeName, item.DefaultText, options);
							items.Add (newDropdownList);
							item.Id = newDropdownList.Id;
							break;
						case ItemType.SECTIONBREAK:
							var terminatorItem = new SectionBreakItem (CurrentUser, itemTypeName);
							items.Add (terminatorItem);
							break;
						default:
							throw new Exception ("Unrecognized itemType: " + item.Type);
						}
					}
				}

				InformationSection informationSection = new InformationSection (CurrentUser, section.Name, items) {
					CustomView = section.CustomView
				};
				sections.Add (informationSection);
				section.Id = informationSection.Id;
			}

			using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork ()) {
				_informationTemplateService.CreateInformationTemplate (CurrentUser, sections [0].Name, sections);
				uow.Commit ();
			}

			return RedirectToAction ("Index", "Home");
        }

		[HttpGet]
		public InformationViewModel GetIcibMDCover ()
		{
			InformationViewModel model = new InformationViewModel ();
			model.Name = "ICIB Marquees - Optional Material Damages Cover";

			var itemsList = new List<InformationItemViewModel> {
				new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-dollar", Label = "What is the highest value of any one marquee at its fullest configuration, including any marque that you sub-hire in for the purposes of rehiring out?", Name = "marqueeHighestValue", Width = 6, Type = ItemType.TEXTBOX },
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Label = "Who normally erects your marquees?", Name = "marqueesErectedBy", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Value = "1", Options = new List<SelectListItem> {
						new SelectListItem { Text = "-- Select --", Value = "" },
						new SelectListItem { Text = "Our Employees", Value = "1" },
						new SelectListItem { Text = "Contractors", Value = "2" },
						new SelectListItem { Text = "The Hirer", Value = "3" },
						new SelectListItem { Text = "Varies", Value = "4" }
					}				
				},
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Control = "text", Label = "Please explain what factors are taken into account in deciding who carries out this work.", Name = "marqueesErectedByDetails", Width = 6, Type = ItemType.TEXTBOX },
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Label = "Who oversees or supervises marquee operations onsite?", Name = "marqueeOperationsOnsite", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Value = "1", Options = new List<SelectListItem> {
						new SelectListItem { Text = "-- Select --", Value = "" },
						new SelectListItem { Text = "Our Employees", Value = "1" },
						new SelectListItem { Text = "Contractors", Value = "2" },
						new SelectListItem { Text = "The Hirer", Value = "3" },
						new SelectListItem { Text = "Varies", Value = "4" }
					}
				},
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Control = "text", Label = "Please explain what factors are taken into account in deciding who supervises this work.", Name = "marqueesSupervisedByDetails", Width = 6, Type = ItemType.TEXTBOX },
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Label = "When you have comitted to a job, do you normally check weather forecasts to determine whether the conditions are suitable to erect a marquee?", Name = "marqueeWeatherConditions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Value = "1", Options = new List<SelectListItem> {
						new SelectListItem { Text = "-- Select --", Value = "" },
						new SelectListItem { Text = "Yes", Value = "1" },
						new SelectListItem { Text = "No", Value = "2" },
					}
				},
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Control = "text", Label = "Please explain why you don't check weather forecasts.", Name = "marqueeWeatherConditionsDetails", Width = 6, Type = ItemType.TEXTBOX },
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Label = "Do you at all times erect marquees according to at least the specifications laid down by the manufacturers?", Name = "marqueeErectedSpecifications", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Value = "1", Options = new List<SelectListItem> {
						new SelectListItem { Text = "-- Select --", Value = "" },
						new SelectListItem { Text = "Yes", Value = "1" },
						new SelectListItem { Text = "No", Value = "2" },
					}
				},
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Control = "text", Label = "Please explain why you don't always follow the manufacturers specifications.", Name = "marqueeErectedSpecificationsDetails", Width = 6, Type = ItemType.TEXTBOX },
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Label = "When erecting marquees larger than 100 square meters, do you apply for permit in accordance with The Building Act?", Name = "marqueeBuildingAct", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Value = "1", Options = new List<SelectListItem> {
						new SelectListItem { Text = "-- Select --", Value = "" },
						new SelectListItem { Text = "Yes", Value = "1" },
						new SelectListItem { Text = "No", Value = "2" },
					}
				},
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Control = "text", Label = "Please explain why you don't apply for a permit.", Name = "marqueeBuildingActDetails", Width = 6, Type = ItemType.TEXTBOX },
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Label = "When your employees or contractors are erecting the marquee and where pegs & poles are driven 1 meter or more into the ground, do you normally check with the relevant authorities for any underground utilities such as water pipes, power or telephone cable?", Name = "marqueeCheckPipesCables", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Value = "1", Options = new List<SelectListItem> {
						new SelectListItem { Text = "-- Select --", Value = "" },
						new SelectListItem { Text = "Yes", Value = "1" },
						new SelectListItem { Text = "No", Value = "2" },
					}
				},
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Control = "text", Label = "Please explain why you don't check for underground pipes and cables.", Name = "marqueeCheckPipesCablesDetails", Width = 6, Type = ItemType.TEXTBOX },
 				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Label = "Have you had any marquee hires that have resulted in extended weather exposure?", Name = "marqueeWeatherExposure", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Value = "1", Options = new List<SelectListItem> {
						new SelectListItem { Text = "-- Select --", Value = "" },
						new SelectListItem { Text = "Yes", Value = "1" },
						new SelectListItem { Text = "No", Value = "2" },
					}
				},
				new InformationItemViewModel { Type = ItemType.SECTIONBREAK },
				new InformationItemViewModel { Control = "text", Label = "Please explain the extent of the weather exposure.", Name = "marqueeWeatherExposureDetails", Width = 6, Type = ItemType.TEXTBOX },
			};

			// set sections
			model.Sections = new List<InformationSectionViewModel> {
				new InformationSectionViewModel { Items = itemsList, Name = "Marquees - Optional Material Damages Cover" }
			};

			return model;
		}

		[HttpGet]
		public InformationViewModel GetIcibLiabilityCover ()
		{
			InformationViewModel model = new InformationViewModel ();




			return model;
		}
    }
}