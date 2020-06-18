using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using DealEngine.Domain.Entities;
using DealEngine.WebUI.Controllers;
using DealEngine.WebUI.Models.Programme;

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

            //
            CreateMap<Organisation, Organisation>()
                .ForMember(dest => dest.InsuranceAttributes, map => map.Ignore())
                .ForMember(dest => dest.OrganisationalUnits, map => map.Ignore())                
                .ForMember(dest => dest.Id, map => map.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Condition(source => source.Type != "Private"))
                .ForMember(dest => dest.OrganisationType, map => map.Ignore());

            CreateMap<User, User>()
                .ForMember(dest => dest.Id, map => map.Ignore())
                .ForMember(dest => dest.Organisations, map => map.Ignore())
                .ForMember(dest => dest.Email, map => map.Ignore())
                .ForMember(dest => dest.Branches, map => map.Ignore())
                .ForMember(dest => dest.Departments, map => map.Ignore())
                .ForMember(dest => dest.UISIssueNotifyProgrammes, map => map.Ignore())
                .ForMember(dest => dest.UISSubmissionNotifyProgrammes, map => map.Ignore())
                .ForMember(dest => dest.AgreementReferNotifyProgrammes, map => map.Ignore())
                .ForMember(dest => dest.AgreementIssueNotifyProgrammes, map => map.Ignore())
                .ForMember(dest => dest.AgreementBoundNotifyProgrammes, map => map.Ignore())
                .ForMember(dest => dest.PaymentConfigNotifyProgrammes, map => map.Ignore())
                .ForMember(dest => dest.InvoiceConfigNotifyProgrammes, map => map.Ignore());

            // Admin
            CreateMap<PrivateServer, PrivateServerViewModel>().ReverseMap();
            CreateMap<PaymentGateway, PaymentGatewayViewModel>().ReverseMap();
            CreateMap<Merchant, MerchantViewModel>().ReverseMap();

            // Information
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

            CreateMap<RevenueData, RevenueDataViewModel>();
            CreateMap<RoleData, RoleDataViewModel>();

            CreateMap<AdditionalRoleInformation, AdditionalRoleInformationViewModel>();
            CreateMap<AdditionalActivityInformation, AdditionalActivityViewModel>()
                .IncludeAllDerived();
                

                //.ForMember(dest => dest.HasInspectionReportOptions, map => map.Ignore())
                //.ForMember(dest => dest.HasIssuedCertificatesOptions, map => map.Ignore())
                //.ForMember(dest => dest.HasObservationServicesOptions, map => map.Ignore())
                //.ForMember(dest => dest.HasRecommendedCladdingOptions, map => map.Ignore())
                //.ForMember(dest => dest.HasStateSchoolOptions, map => map.Ignore());

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
                .ForMember(dest => dest.RevenueData, map => map.Ignore())
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
                .ForMember(dest => dest.Payment, map => map.Ignore())
                .ForMember(dest => dest.InformationSheet, map => map.Ignore())
                .ForMember(dest => dest.ClientAgreementEGlobalResponses, map => map.Ignore())
                .ForMember(dest => dest.ClientAgreementEGlobalSubmissions, map => map.Ignore())
                .ForMember(dest => dest.Agreements, map => map.Ignore())
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
