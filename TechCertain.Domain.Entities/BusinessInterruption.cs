using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class BusinessInterruption : EntityBase, IAggregateRoot
    {
        protected BusinessInterruption() : base(null) { }

        public BusinessInterruption(User createdBy)
            : base(createdBy)
        {
            
        }

        public virtual Location Location
        {
            get;
            set;
        }

        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }

        public virtual int IndemnityPeriod
        {
            get;
            set;
        }

        public virtual DateTime FinancialYearEnd
        {
            get;
            set;
        }

        public virtual int GrossProfit
        {
            get;
            set;
        }

        public virtual int GrossRents
        {
            get;
            set;
        }

        public virtual int AdditionalIncreaseInCostsOfWorking
        {
            get;
            set;
        }

        public virtual int ClaimsPreparationCosts
        {
            get;
            set;
        }

        public virtual int DualWages
        {
            get;
            set;
        }

        public virtual int InitialPeriod
        {
            get;
            set;
        }

        public virtual int Remainder
        {
            get;
            set;
        }

        public virtual int AlternatePeriod
        {
            get;
            set;
        }

        public virtual bool Removed
        {
            get;
            set;
        }

    }
}
