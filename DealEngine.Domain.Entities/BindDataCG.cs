using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Domain.Entities
{
    public class BindDataCG : EntityBase, IAggregateRoot
    {
        protected BindDataCG() : base(null) { }

        public BindDataCG(User createdBy)
            : base(createdBy)
        {
        }

        public BindDataCG(User user, ClientInformationSheet sheet)
            : base (user)
        {
            if(sheet!= null)
            {
                var Boat = sheet.Boats.FirstOrDefault();
                if(Boat != null)
                {
                    Year = Boat.YearOfManufacture.ToString();
                    Make = Boat.BoatMake;
                    Model = Boat.BoatModel;
                    Type = Boat.BoatType2;
                    Construction = Boat.BoatType1;
                    Location = Boat.BoatLandLocation.Location;
                    SumInsured = Boat.Sum.ToString();
                    Trailer = Boat.BoatTrailers.FirstOrDefault();
                    Hull = "";
                    TotalSumInsured = "To Be Calced";
                    Excess = Boat.BoatQuoteExcessOption.ToString();
                    RacingRisk = Boat.BoatUses.FirstOrDefault().BoatUseRace;
                    Premium = "To Be Calced";
                    Coy = "To Be Calced";
                    FENZ = "To Be Calced";
                    GST = "To Be Calced";
                    Total = "To Be Calced";
                    Brokerage = "To Be Calced";
                }
            }
        }

        public virtual string BindType {get;set;}
        public virtual DateTime AgreementDate { get; set; }
        public virtual string ClientName { get; set; }
        public virtual string Year { get; set; }
        public virtual string Make { get; set; }
        public virtual string Model { get; set; }
        public virtual string Type { get; set; }
        public virtual string Construction { get; set; }
        public virtual Location Location { get; set; }
        public virtual string SumInsured { get; set; }
        public virtual string Hull { get; set; }
        public virtual Vehicle Trailer { get; set; }
        public virtual string TotalSumInsured { get; set; }
        public virtual string Excess { get; set; }
        public virtual string RacingRisk { get; set; }
        public virtual string Premium { get; set; }
        public virtual string Coy { get; set; }
        public virtual string FENZ { get; set; }
        public virtual string GST { get; set; }
        public virtual string Total { get; set; }
        public virtual string Brokerage { get; set; }
    }
}

