using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace techcertain2019core.Models.ViewModels
{
    public class BusinessInterruptionViewModel
    {
        public Guid AnswerSheetId { get; set; }

        public Guid BusinessInterruptionId { get; set; }

        public int IndemnityPeriod { get; set; }

        public string FinancialYearEnd { get; set; }

        public int GrossProfit { get; set; }

        public int GrossRents { get; set; }

        public int AdditionalIncreaseInCostsOfWorking { get; set; }

        public int ClaimsPreparationCosts { get; set; }

        public int DualWages { get; set; }

        public int InitialPeriod { get; set; }

        public int Remainder { get; set; }

        public int AlternatePeriod { get; set; }

        public Guid BusinessInterruptionLocation { get; set; }

        public BusinessInterruption ToEntity(User creatingUser)
        {
            BusinessInterruption businessInterruption = new BusinessInterruption(creatingUser);
            UpdateEntity(businessInterruption);
            return businessInterruption;
        }

        public BusinessInterruption UpdateEntity(BusinessInterruption businessInterruption)
        {
            businessInterruption.IndemnityPeriod = IndemnityPeriod;
            if (!string.IsNullOrEmpty(FinancialYearEnd))
            {
                businessInterruption.FinancialYearEnd = DateTime.Parse(FinancialYearEnd, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ"));
            }
            else
            {
                businessInterruption.FinancialYearEnd = DateTime.MinValue;
            }
            businessInterruption.GrossProfit = GrossProfit;
            businessInterruption.GrossRents = GrossRents;
            businessInterruption.AdditionalIncreaseInCostsOfWorking = AdditionalIncreaseInCostsOfWorking;
            businessInterruption.ClaimsPreparationCosts = ClaimsPreparationCosts;
            businessInterruption.DualWages = DualWages;
            businessInterruption.InitialPeriod = InitialPeriod;
            businessInterruption.Remainder = Remainder;
            businessInterruption.AlternatePeriod = AlternatePeriod;

            return businessInterruption;
        }

        public static BusinessInterruptionViewModel FromEntity(BusinessInterruption businessInterruption)
        {
            BusinessInterruptionViewModel model = new BusinessInterruptionViewModel
            {
                BusinessInterruptionId = businessInterruption.Id,
                IndemnityPeriod = businessInterruption.IndemnityPeriod,
                FinancialYearEnd = (businessInterruption.FinancialYearEnd > DateTime.MinValue) ? businessInterruption.FinancialYearEnd.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                GrossProfit = businessInterruption.GrossProfit,
                GrossRents = businessInterruption.GrossRents,
                AdditionalIncreaseInCostsOfWorking = businessInterruption.AdditionalIncreaseInCostsOfWorking,
                ClaimsPreparationCosts = businessInterruption.ClaimsPreparationCosts,
                DualWages = businessInterruption.DualWages,
                InitialPeriod = businessInterruption.InitialPeriod,
                Remainder = businessInterruption.Remainder,
                AlternatePeriod = businessInterruption.AlternatePeriod,
            };
            if (businessInterruption.Location != null)
                model.BusinessInterruptionLocation = businessInterruption.Location.Id;

            return model;
        }
    }

}