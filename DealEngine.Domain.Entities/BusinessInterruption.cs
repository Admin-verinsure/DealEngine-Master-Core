using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class BusinessInterruption : EntityBase, IAggregateRoot
    {
        protected BusinessInterruption() : base(null) { }

        public BusinessInterruption(User createdBy)
            : base(createdBy)
        {
            
        }
        public virtual Location Location { get; set; }
        public virtual ClientInformationSheet ClientInformationSheet { get; set; }
        public virtual int IndemnityPeriod { get; set; }
        public virtual DateTime FinancialYearEnd { get; set; }
        public virtual int GrossProfit { get; set; }
        public virtual int GrossRents { get; set; }
        public virtual int AdditionalIncreaseInCostsOfWorking { get; set; }
        public virtual int ClaimsPreparationCosts { get; set; }
        public virtual int DualWages { get; set; }
        public virtual int InitialPeriod { get; set; }
        public virtual int Remainder { get; set; }
        public virtual int AlternatePeriod { get; set; }

        public virtual bool Removed { get; set; }
        public virtual BusinessInterruption CloneForNewSheet(ClientInformationSheet newSheet) 
        {
            BusinessInterruption newBusinessInterruption = new BusinessInterruption();
            newBusinessInterruption.AdditionalIncreaseInCostsOfWorking = AdditionalIncreaseInCostsOfWorking;
            newBusinessInterruption.AlternatePeriod = AlternatePeriod;
            newBusinessInterruption.ClaimsPreparationCosts = ClaimsPreparationCosts;
            newBusinessInterruption.ClientInformationSheet = newSheet;
            newBusinessInterruption.CreatedBy = newSheet.CreatedBy;
            newBusinessInterruption.DualWages = DualWages;
            newBusinessInterruption.FinancialYearEnd = FinancialYearEnd;
            newBusinessInterruption.GrossProfit = GrossProfit;
            newBusinessInterruption.GrossRents = GrossRents;
            newBusinessInterruption.IndemnityPeriod = IndemnityPeriod;
            newBusinessInterruption.InitialPeriod = InitialPeriod;
            newBusinessInterruption.Location = Location;
            newBusinessInterruption.Remainder = Remainder;
            return newBusinessInterruption;
        }
    }
}
