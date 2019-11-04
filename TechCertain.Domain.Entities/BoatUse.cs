using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class BoatUse : EntityBase, IAggregateRoot
    {
        protected BoatUse() : base(null) { }

        public BoatUse(User createdBy, string boatUseCategory)
            : base(createdBy)
        {
            BoatUseCategory = boatUseCategory;
        }
        public virtual IList<Boat> Boat
        {
            get;
            set;
        }
        public virtual BoatUse OriginalBoatUse
        {
            get;
            protected set;
        }

        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }

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
        public virtual string test1
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
            if (ClientInformationSheet == newSheet)
                throw new Exception("Cannot clone boat use for original information");

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
            newBoatUse.OriginalBoatUse = this;
            return newBoatUse;
        }
    }
}

