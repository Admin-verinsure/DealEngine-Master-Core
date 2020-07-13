﻿using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;
using Newtonsoft.Json;

namespace DealEngine.Domain.Entities
{
    public class InsuranceAttribute : EntityBase, IAggregateRoot
    {
        public virtual string Name { get; protected set; }
        public virtual string InsuranceAttributeName { get; protected set; }
        public virtual IList<AuditHistory> AuditHistory { get; set; }
        //[JsonIgnore]
        //public virtual IList<Organisation> IAOrganisations { get; set; }
        protected InsuranceAttribute() : base(null) { }

        public InsuranceAttribute(User createdBy, string name)
            : base(createdBy)
        {
            Name = name;
            InsuranceAttributeName = name;
            //IAOrganisations = new List<Organisation>();
            AuditHistory = new List<AuditHistory>();
        }

        public virtual void SetHistory(ClientInformationSheet sheet)
        {
            AuditHistory audit = new AuditHistory();
            audit.PreviousSheet = sheet;
            audit.DateDeleted = DateTime.Now;
            AuditHistory.Add(audit);
        }
    }
}
