using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class InsuranceAttribute : EntityBase, IAggregateRoot
    {
        public virtual string InsuranceAttributeName { get; protected set; }

        public virtual IList<Organisation> IAOrganisations { get; set; }

        protected InsuranceAttribute() : base(null) { }

        public InsuranceAttribute(User createdBy, string insuranceAttributeName)
            : base(createdBy)
        {
            InsuranceAttributeName = insuranceAttributeName;
            IAOrganisations = new List<Organisation>();
        }

    }
}
