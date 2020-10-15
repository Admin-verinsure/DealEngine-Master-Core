using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class BoatUse : EntityBase, IAggregateRoot
    {
        protected BoatUse() : base(null) { }

        public BoatUse(User createdBy, string boatUseCategory)
            : base(createdBy)
        {
            BoatUseCategory = boatUseCategory;
        }
        //public virtual void PopulateEntity()
        //{

        //    // Populating the Entity
        //    BoatUseCategory = boatUse.BoatUseCategory
        //    BoatUseLiveOnBoard
        //    BoatUseRace
        //    BoatUseRaceCategory
        //    BoatUseRaceUseSpinnakers
        //    BoatUseLiveNotes
        //    BoatUseRaceNotes
        //    BoatUseAdditionalNotes

        //    boatUse.BoatUseCategory = BoatUseCategory;
        //    boatUse.BoatUseLiveOnBoard = ;
        //    boatUse.BoatUseRace = BoatUseRace;
        //    boatUse.BoatUseRaceCategory = BoatUseRaceCategory;
        //    boatUse.BoatUseRaceUseSpinnakers = BoatUseRaceUseSpinnakers;
        //    boatUse.BoatUseLiveNotes = BoatUseLiveNotes;
        //    boatUse.BoatUseRaceNotes = BoatUseRaceNotes;

        //    boatUse.BoatUseAdditionalNotes = BoatUseAdditionalNotes;
        //    if (!string.IsNullOrEmpty(BoatUseEffectiveDate))
        //    {
        //        boatUse.BoatUseEffectiveDate = DateTime.Parse(BoatUseEffectiveDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ"));
        //    }
        //    else
        //    {
        //        boatUse.BoatUseEffectiveDate = DateTime.MinValue;
        //    }
        //    if (!string.IsNullOrEmpty(BoatUseCeaseDate))
        //    {
        //        boatUse.BoatUseCeaseDate = DateTime.Parse(BoatUseCeaseDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ"));
        //    }
        //    else
        //    {
        //        boatUse.BoatUseCeaseDate = DateTime.MinValue;
        //    }
        //    boatUse.BoatUseCeaseReason = BoatUseCeaseReason;

        //}

        //public virtual BoatUse OriginalBoatUse
        //{
        //    get;
        //    protected set;
        //}

        public virtual string BoatUseCategory
        {
            get;
            set;
        }

        public virtual string BoatUseLiveOnBoard
        {
            get;
            set;
        }

        public virtual string BoatUseRace
        {
            get;
            set;
        }

        public virtual string BoatUseRaceCategory
        {
            get;
            set;
        }

        public virtual string BoatUseRaceUseSpinnakers
        {
            get;
            set;
        }

        public virtual string BoatUseLiveNotes
        {
            get;
            set;
        }

        public virtual string BoatUseRaceNotes
        {
            get;
            set;
        }

        public virtual string BoatUseAdditionalNotes
        {
            get;
            set;
        }

        public virtual bool Removed
        {
            get;
            set;
        }

        public virtual DateTime BoatUseEffectiveDate
        {
            get;
            set;
        }

        public virtual DateTime BoatUseCeaseDate
        {
            get;
            set;
        }

        public virtual int BoatUseCeaseReason
        {
            get;
            set;
        }

        public virtual DateTime BoatUseInceptionDate
        {
            get;
            set;
        }

        public virtual DateTime BoatUseExpireDate
        {
            get;
            set;
        }

        public virtual BoatUse CloneForNewSheet(ClientInformationSheet newSheet)
        {
            BoatUse newBoatUse = new BoatUse(newSheet.CreatedBy, BoatUseCategory);
            newBoatUse.BoatUseLiveOnBoard = BoatUseLiveOnBoard;
            newBoatUse.BoatUseRace = BoatUseRace;
            newBoatUse.BoatUseRaceCategory = BoatUseRaceCategory;
            newBoatUse.BoatUseRaceUseSpinnakers = BoatUseRaceUseSpinnakers;
            newBoatUse.BoatUseLiveNotes = BoatUseLiveNotes;
            newBoatUse.BoatUseRaceNotes = BoatUseRaceNotes;
            if (BoatUseEffectiveDate > DateTime.MinValue)
                newBoatUse.BoatUseEffectiveDate = BoatUseEffectiveDate;
            if (BoatUseCeaseDate > DateTime.MinValue)
                newBoatUse.BoatUseCeaseDate = BoatUseCeaseDate;
            newBoatUse.BoatUseCeaseReason = BoatUseCeaseReason;
            if (BoatUseInceptionDate > DateTime.MinValue)
                newBoatUse.BoatUseInceptionDate = BoatUseInceptionDate;
            if (BoatUseExpireDate > DateTime.MinValue)
                newBoatUse.BoatUseExpireDate = BoatUseExpireDate;
            //newBoatUse.OriginalBoatUse = this;
            return newBoatUse;
        }
    }
}

