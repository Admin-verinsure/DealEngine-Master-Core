using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using DealEngine.Domain.Entities;
using DealEngine.WebUI.Controllers;


namespace DealEngine.WebUI.Models
{
    public static class MapperConfig
    {
        public static IMapper ConfigureMaps()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            return mapperConfiguration.CreateMapper();
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // place all automapper maps here

            // Admin
            CreateMap<PrivateServer, PrivateServerViewModel>().ReverseMap();
            CreateMap<PaymentGateway, PaymentGatewayViewModel>().ReverseMap();
            CreateMap<Merchant, MerchantViewModel>().ReverseMap();

            // Home
            //CreateMap<UserTask, TaskItem>().ReverseMap();

            // Information
            CreateMap<AdditionalActivityViewModel, RevenueByActivityViewModel>();
            CreateMap<InformationTemplate, InformationViewModel>();
            CreateMap<InformationSection, InformationSectionViewModel>();
            CreateMap<InformationItem, InformationItemViewModel>()
                .Include<TextboxItem, InformationItemViewModel>()
                .Include<LabelItem, InformationItemViewModel>()
                .Include<DropdownListItem, InformationItemViewModel>()
                .Include<TextAreaItem, InformationItemViewModel>()
                .Include<MultiselectListItem, InformationItemViewModel>()
                .Include<JSButtonItem, InformationItemViewModel>()
                .Include<SubmitButtonItem, InformationItemViewModel>()
                .Include<SectionBreakItem, InformationItemViewModel>()
                .Include<MotorVehicleListItem, InformationItemViewModel>();
                //.Include<AdditionalInformation, RevenueByActivityViewModel>();
                //.Include<InformationItemConditional, InformationItemViewModel>();

            CreateMap<SelectListItem, DropdownListOption>().ReverseMap();
            //CreateMap<DropdownListOption, SelectListItem> ();

            CreateMap<TextboxItem, InformationItemViewModel>();
            CreateMap<DropdownListItem, InformationItemViewModel>();
            CreateMap<LabelItem, InformationItemViewModel>();
            CreateMap<TextAreaItem, InformationItemViewModel>();
            CreateMap<MultiselectListItem, InformationItemViewModel>();
            CreateMap<JSButtonItem, InformationItemViewModel>();
            CreateMap<SubmitButtonItem, InformationItemViewModel>();
            CreateMap<SectionBreakItem, InformationItemViewModel>();
            CreateMap<MotorVehicleListItem, InformationItemViewModel>();

            CreateMap<Location, LocationViewModel>();
            CreateMap<Vehicle, VehicleViewModel>();
            CreateMap<OrganisationalUnit, OrganisationalUnitViewModel>();
            CreateMap<Organisation, OrganisationViewModel>();
            CreateMap<User, UserDetailsVM>();
            CreateMap<BusinessActivity, BusinessActivityViewModel>();

            // Policy
            CreateMap<RiskCategory, InsuranceRiskCategory>().ReverseMap();
            CreateMap<Old_PolicyDocumentTemplate, PolicyDocumentViewModel>();

            //subsystem            
            CreateMap<InformationTemplate, SubInformationTemplate>()
                .ForMember(dest => dest.BaseInformationTemplate, map => map.Ignore())
                .ForMember(dest => dest.Sections, map => map.Ignore())
                .ForMember(dest => dest.Name, map => map.MapFrom(t => t.Name + " Sub"))
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<ClientInformationSheet, SubClientInformationSheet>()
                .ForMember(dest => dest.BaseClientInformationSheet, map => map.Ignore())
                .ForMember(dest => dest.Id, map => map.Ignore())
                .ForMember(dest => dest.SubClientInformationSheets, map => map.Ignore())
                .ForMember(dest => dest.Answers, map => map.Ignore())
                .ForMember(dest => dest.ClaimNotifications, map => map.Ignore())
                .ForMember(dest => dest.ReferenceId, map => map.Ignore())
                .ForMember(dest => dest.PreviousInformationSheet, map => map.Ignore())
                .ForMember(dest => dest.NextInformationSheet, map => map.Ignore())
                .ForMember(dest => dest.BusinessContracts, map => map.Ignore())
                .ForMember(dest => dest.Organisation, map => map.Ignore())
                .ForMember(dest => dest.ClaimNotifications, map => map.Ignore());                
            CreateMap<ClientProgramme, SubClientProgramme>()
                .ForMember(dest => dest.BaseClientProgramme, map => map.Ignore())                             
                .ForMember(dest => dest.EGlobalClientStatus, map => map.Ignore())
                .ForMember(dest => dest.SubClientProgrammes, map => map.Ignore())
                .ForMember(dest => dest.ChangeReason, map => map.Ignore())
                .ForMember(dest => dest.Products, map => map.Ignore())
                .ForMember(dest => dest.Products, map => map.Ignore())
                .ForMember(dest => dest.Payment, map => map.Ignore())
                .ForMember(dest => dest.InformationSheet, map => map.Ignore())
                .ForMember(dest => dest.ClientAgreementEGlobalResponses, map => map.Ignore())
                .ForMember(dest => dest.ClientAgreementEGlobalSubmissions, map => map.Ignore())
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<InformationSection, InformationSection>()                
                .ForMember(dest => dest.Id, map => map.Ignore());

            //clonesystem
            CreateMap<ClientInformationSheet, ClientInformationSheet>()
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<ClientProgramme, ClientProgramme>()
                .ForMember(dest => dest.Id, map => map.Ignore());
        }
    }

}
