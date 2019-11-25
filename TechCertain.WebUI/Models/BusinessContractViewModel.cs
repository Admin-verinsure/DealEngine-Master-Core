using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models
{
    public class BusinessContractViewModel : BaseViewModel
    {
        public Guid AnswerSheetId { get; set; }

        public Guid BusinessContractId { get; set; }

        public string Year { get; set; }

        public string ContractTitle { get; set; }

        public string ConstructionValue { get; set; }

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
            businessContract.ContractType = ContractType;
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
                Fees = businessContract.Fees,
            };
            return model;
        }
    }

}

