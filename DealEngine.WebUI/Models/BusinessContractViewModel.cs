using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class BusinessContractViewModel : BaseViewModel
    {
        public Guid AnswerSheetId { get; set; }

        public Guid BusinessContractId { get; set; }

        public string Year { get; set; }

        public string ContractTitle { get; set; }
        public string ProjectDescription { get; set; }
        public string MajorResponsibilities { get; set; }
        public string ProjectDuration { get; set; }

        public Boolean ProjectDirector { get; set; }
        public Boolean ProjectManager { get; set; }
        public Boolean ProjectCoordinator { get; set; }
        public Boolean ProjectEngineer { get; set; }
        public string ConstructionValue { get; set; }
        public string Country { get; set; }
        public string Fees { get; set; }

        public string ContractType { get; set; }

        public BusinessContract ToEntity(User creatingUser)
        {
            BusinessContract businessContract = new BusinessContract(creatingUser);
            UpdateEntity(businessContract);
            return businessContract;
        }

        public BusinessContract UpdateEntity(BusinessContract businessContract)
        {
            businessContract.Year = Year;
            businessContract.ContractTitle = ContractTitle;
            businessContract.ConstructionValue = ConstructionValue;
            businessContract.Fees = Fees;
            businessContract.ProjectDescription = ProjectDescription;
            businessContract.MajorResponsibilities = MajorResponsibilities;
            businessContract.ProjectDirector = ProjectDirector;
            businessContract.ProjectManager = ProjectManager;
            businessContract.ProjectCoordinator = ProjectCoordinator;
            businessContract.ProjectEngineer = ProjectEngineer;
            businessContract.ProjectDuration = ProjectDuration;
            businessContract.Country = Country;
            return businessContract;
        }

        public static BusinessContractViewModel FromEntity(BusinessContract businessContract)
        {
            BusinessContractViewModel model = new BusinessContractViewModel
            {
                BusinessContractId = businessContract.Id,
                Year = businessContract.Year,
                ConstructionValue = businessContract.ConstructionValue,
                ContractTitle = businessContract.ContractTitle,
                ContractType = businessContract.ContractType,
                ProjectDescription = businessContract.ProjectDescription,
                MajorResponsibilities = businessContract.MajorResponsibilities,
                ProjectManager = businessContract.ProjectManager,
                ProjectCoordinator = businessContract.ProjectCoordinator,
                ProjectEngineer = businessContract.ProjectEngineer,
                ProjectDuration = businessContract.ProjectDuration,
                ProjectDirector = businessContract.ProjectDirector,
                Country = businessContract.Country,
                Fees = businessContract.Fees,
            };
            return model;
        }
    }

}

