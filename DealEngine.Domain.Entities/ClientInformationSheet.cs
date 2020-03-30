﻿using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace DealEngine.Domain.Entities
{
    public class ClientInformationSheet : EntityBase, IAggregateRoot
    {
        public virtual Organisation Owner { get; set; }        
        public virtual ClientProgramme Programme { get; set; }
		[Obsolete ("No longer required with the new Programme implementation")]        
        public virtual ClientAgreement ClientAgreement { get; set; }       
        public virtual IList<ClientInformationAnswer> Answers { get; protected set; }
		public virtual IList<Vehicle> Vehicles { get; protected set; }
        public virtual IList<Building> Buildings { get; protected set; }
        public virtual IList<BusinessInterruption> BusinessInterruptions { get; protected set; }
        public virtual IList<MaterialDamage> MaterialDamages { get; protected set; }        
        public virtual IList<ClaimNotification> ClaimNotifications { get; protected set; }
        public virtual IList<Location> Locations { get; protected set; }
        public virtual IList<WaterLocation> WaterLocations { get; protected set; }
        public virtual IList<Boat> Boats { get; protected set; }
        public virtual IList<BoatUse> BoatUses { get; protected set; }
        public virtual IList<Organisation> Organisation { get; set; }
		public virtual IList<SharedDataRole> SharedDataRoles { get; set; }        
        public virtual IList<SubClientInformationSheet> SubClientInformationSheets { get; set; }
        public virtual RevenueByActivity RevenueData { get; set; }
        //Not Started; Started; Submitted; Bound and pending payment; Bound and invoice pending; Bound and invoiced; Bound; Not Taken Up        
        public virtual string Status { get; set; }        
        public virtual string ReferenceId { get; set; }        
        public virtual ClientInformationSheet PreviousInformationSheet { get; protected set; }        
        public virtual ClientInformationSheet NextInformationSheet { get; protected set; }
		public virtual bool IsRenewawl { get; set; }
        public virtual bool IsChange { get; set; }
        public virtual string SheetReference { get; set; }
        public virtual DateTime SubmitDate { get; set; }
		public virtual User SubmittedBy { get; set; }
        public virtual DateTime UnlockDate { get; set; }
        public virtual User UnlockedBy { get; set; }        
        public virtual IList<AuditLog> ClientInformationSheetAuditLogs { get; protected set; }        
        public virtual IList<BusinessContract> BusinessContracts { get; protected set; }
        protected ClientInformationSheet () : this (null) { }

		protected ClientInformationSheet (User createdBy)
			: base (createdBy)
		{
            SubClientInformationSheets = new List<SubClientInformationSheet>();
            Organisation = new List<Organisation>();
            SharedDataRoles = new List<SharedDataRole>();
            Answers = new List<ClientInformationAnswer> ();
			Vehicles = new List<Vehicle> ();
			Locations = new List<Location> ();
            WaterLocations = new List<WaterLocation>();
            Buildings = new List<Building>();
            Boats = new List<Boat>();
            BoatUses = new List<BoatUse>();
            ClaimNotifications = new List<ClaimNotification>();
            ClientInformationSheetAuditLogs = new List<AuditLog>();
            BusinessContracts = new List<BusinessContract>();
            Status = "Not Started";
		}

		public ClientInformationSheet (User createdBy, Organisation createdFor, InformationTemplate informationTemplate)
			: this (createdBy)
		{
			//OwnerId = user.GetPersonalOrganisation ().Id;
			Owner = createdFor;	// For now
			//InformationTemplate = informationTemplate;
		}

        public ClientInformationSheet(User createdBy, Organisation createdFor, InformationTemplate informationTemplate, string referenceId)
            : this(createdBy)
        {
            //OwnerId = user.GetPersonalOrganisation ().Id;
            Owner = createdFor; // For now
                                //InformationTemplate = informationTemplate;
            ReferenceId = referenceId;
        }

        public ClientInformationSheet (User createdBy, ClientInformationSheet originalSheet, bool renewal)
			: this (createdBy)
		{
			Owner = originalSheet.Owner;
			//InformationTemplate = originalSheet.InformationTemplate;
			PreviousInformationSheet = originalSheet;
			IsRenewawl = renewal;
		}       

        public virtual void AddAnswer(string itemName, string value)
		{
			// no value entered for that question? we won't bother saving it
			if (string.IsNullOrWhiteSpace (value))
				return;

			ClientInformationAnswer answer = Answers.FirstOrDefault (a => a.ItemName == itemName);
			if (answer != null)
				answer.Value = value;
			else
				Answers.Add (new ClientInformationAnswer (CreatedBy, itemName, value));
		}

		public virtual void AddVehicle (Vehicle vehicle)
		{
			Vehicles.Add (vehicle);
		}

        public virtual void AddBoat(Boat boat)
        {
            Boats.Add(boat);
        }

        public virtual void AddClaim(ClaimNotification claim)
        {
            ClaimNotifications.Add(claim);
        }

        public virtual void AddBuilding(Building building)
        {
            Buildings.Add(building);
        }

        public virtual void AddWaterLocation(WaterLocation waterLocation)
        {
            WaterLocations.Add(waterLocation);
        }

        public virtual void AddLocation (Location location)
		{
			Locations.Add (location);
		}

        public virtual void AddBoatUse(BoatUse boatUse)
        {
            BoatUses.Add(boatUse);
        }

        public virtual void AddClientInformationSheetAuditLog(AuditLog clientInformationSheetAuditLog)
        {
            ClientInformationSheetAuditLogs.Add(clientInformationSheetAuditLog);
        }
        public virtual void AddBusinessContract(BusinessContract businessContract)
        {
            BusinessContracts.Add(businessContract);
        }
        public virtual ClientInformationSheet CloneForUpdate (User cloningUser, IMapper mapper)
		{
			ClientInformationSheet newSheet = new ClientInformationSheet (cloningUser, Owner, null);
            newSheet = mapper.Map<ClientInformationSheet>(this);
            newSheet.PreviousInformationSheet = this;
			newSheet.Product = Product;
			NextInformationSheet = newSheet;
            


   //         foreach (ClientInformationAnswer answer in Answers)
			//	newSheet.Answers.Add (answer.CloneForNewSheet (newSheet));

			//foreach (Location location in Locations)
			//	newSheet.AddLocation (location.CloneForNewSheet (newSheet));

   //         foreach (Building building in Buildings.Where(bui => !bui.Removed && bui.DateDeleted == null))
   //             newSheet.AddBuilding(building.CloneForNewSheet(newSheet));

   //         foreach (BoatUse boatUse in BoatUses.Where(bu => !bu.Removed && bu.DateDeleted == null))
   //             newSheet.AddBoatUse(boatUse.CloneForNewSheet(newSheet));

   //         foreach (Vehicle vehicle in Vehicles.Where(v => !v.Removed && v.DateDeleted == null))
			//	newSheet.AddVehicle (vehicle.CloneForNewSheet (newSheet));

   //         foreach (Boat boat in Boats.Where(b => !b.Removed && b.DateDeleted == null))
   //             newSheet.AddBoat(boat.CloneForNewSheet(newSheet));

   //         foreach (ClaimNotification claim in ClaimNotifications.Where(cl => !cl.Removed && cl.DateDeleted == null))
   //             newSheet.AddClaim(claim.CloneForNewSheet(newSheet));

            return newSheet;
		}

		public virtual ClientInformationSheet CloneForRenewal (User renewingUser, IMapper mapper)
		{
			ClientInformationSheet sheet = CloneForUpdate(renewingUser, mapper);
			sheet.IsRenewawl = true;
			return sheet;
		}

		public virtual Product Product { get; set; }
	}

    public class SubClientInformationSheet : ClientInformationSheet
    {
        public virtual ClientInformationSheet BaseClientInformationSheet { get; set; }
        public SubClientInformationSheet() { }
    }
}

