//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TechCertain.Domain.Entities;
//using techcertain2015rebuildcore.Models.ViewModels;

//namespace techcertain2015rebuildcore
//{
//    public static class MapperConfig
//    {
//        public static IMapper ConfigureMaps()
//        {
//            var mapperConfiguration = new MapperConfiguration(cfg =>
//            {
//                cfg.AddProfile(new MappingProfile());
//            });
//            return mapperConfiguration.CreateMapper();
//        }
//    }

//    public class MappingProfile : Profile
//    {
//        public MappingProfile()
//        {
//            // place all automapper maps here

//            // Admin
//            CreateMap<PrivateServer, PrivateServerViewModel>().ReverseMap();
//            CreateMap<PaymentGateway, PaymentGatewayViewModel>().ReverseMap();
//            CreateMap<Merchant, MerchantViewModel>().ReverseMap();

//            // Home
//            //CreateMap<UserTask, TaskItem>().ReverseMap();

//            // InformationBuilder
//            CreateMap<InformationBuilder, InformationBuilderViewModel>().ReverseMap();

//            // Information
//            CreateMap<InformationTemplate, InformationViewModel>();
//            CreateMap<InformationSection, InformationSectionViewModel>();
//            CreateMap<InformationItem, InformationItemViewModel>()
//                .Include<TextboxItem, InformationItemViewModel>()
//                .Include<LabelItem, InformationItemViewModel>()
//                .Include<DropdownListItem, InformationItemViewModel>()
//                .Include<TextAreaItem, InformationItemViewModel>()
//                .Include<MultiselectListItem, InformationItemViewModel>()
//                .Include<JSButtonItem, InformationItemViewModel>()
//                .Include<SubmitButtonItem, InformationItemViewModel>()
//                .Include<SectionBreakItem, InformationItemViewModel>()
//                .Include<MotorVehicleListItem, InformationItemViewModel>();
//            CreateMap<SelectListItem, DropdownListOption>().ReverseMap();
//            //CreateMap<DropdownListOption, SelectListItem> ();

//            CreateMap<InformationItemConditional, InformationItemConditionalViewModel>().ReverseMap();

//            CreateMap<TextboxItem, InformationItemViewModel>();
//            CreateMap<DropdownListItem, InformationItemViewModel>();
//            CreateMap<LabelItem, InformationItemViewModel>();
//            CreateMap<TextAreaItem, InformationItemViewModel>();
//            CreateMap<MultiselectListItem, InformationItemViewModel>();
//            CreateMap<JSButtonItem, InformationItemViewModel>();
//            CreateMap<SubmitButtonItem, InformationItemViewModel>();
//            CreateMap<SectionBreakItem, InformationItemViewModel>();
//            CreateMap<MotorVehicleListItem, InformationItemViewModel>();

//            CreateMap<Location, LocationViewModel>();
//            CreateMap<Vehicle, VehicleViewModel>();
//            CreateMap<OrganisationalUnit, OrganisationalUnitViewModel>();
//            CreateMap<Organisation, OrganisationViewModel>();
//            CreateMap<User, UserDetailsVM>();

//            CreateMap<RevenueByActivity, RevenueByActivityViewModel>();
//            CreateMap<RevenueByCountry, RevenueByCountryViewModel>();
//            CreateMap<BusinessActivity, BusinessActivityViewModel>();

//            // Policy
//            CreateMap<RiskCategory, InsuranceRiskCategory>().ReverseMap();
//            CreateMap<Old_PolicyDocumentTemplate, PolicyDocumentViewModel>();
//        }
//    }

//}
