using DealEngine.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Domain.Entities
{
    public class RoleData : EntityBase, IAggregateRoot
    {

        protected RoleData() : this(null) 
        {
            DataRoles = new List<SharedDataRole>();
            AdditionalRoleInformation = new AdditionalRoleInformation(null);
        }

        public RoleData(ClientInformationSheet sheet, User user=null)
            :base(user)
        {
            DataRoles = new List<SharedDataRole>();
            AdditionalRoleInformation = new AdditionalRoleInformation(null);
            if (sheet != null)
            {
                DataRoles = CreateRoles(sheet);
            }
        }

        private IList<SharedDataRole> CreateRoles(ClientInformationSheet sheet)
        {
            if (sheet.RoleData != null)
            {
                foreach (var role in sheet.RoleData.DataRoles)
                {
                    DataRoles.Add(new SharedDataRole(null)
                    {
                        TemplateId = role.TemplateId,
                        Name = role.Name,
                        ProfessionalTotal = 0,
                        PrincipalTotal = 0,
                        OtherTotal = 0,
                        Selected = false                  
                    });
                }
            }
            if (DataRoles.Count != sheet.Programme.BaseProgramme.SharedDataRoleTemplates.Count)
            {
                foreach (var template in sheet.Programme.BaseProgramme.SharedDataRoleTemplates)
                {
                    var containsRole = DataRoles.Where(t => t.TemplateId == template.Id).ToList();
                    if (containsRole.Count == 0)
                    {
                        DataRoles.Add(new SharedDataRole(null)
                        {
                            TemplateId = template.Id,
                            Name = template.Name,
                            ProfessionalTotal = 0,
                            PrincipalTotal = 0,
                            OtherTotal = 0,
                            Selected = false
                        });
                    }
                }
            }

            return DataRoles;
        }

        public virtual IList<SharedDataRole> DataRoles { get; set; }
        public virtual AdditionalRoleInformation AdditionalRoleInformation { get; set; }
    }

    public class SharedDataRole : EntityBase, IAggregateRoot
    {
        protected SharedDataRole() : this(null) { }
        public SharedDataRole(User createdBy) : base(createdBy) { }
        public virtual Guid TemplateId { get; set; }
        public virtual bool Selected { get; set; }
        public virtual string Name { get; set; }
        public virtual int PrincipalTotal { get; set; }
        public virtual int OtherTotal { get; set; }
        public virtual int ProfessionalTotal { get; set; }
    }


    public class AdditionalRoleInformation : EntityBase, IAggregateRoot
    {
        protected AdditionalRoleInformation() : this(null) { }
        public AdditionalRoleInformation(User createdBy) : base(createdBy) { }
        public virtual string OtherDetails { get; set; }
    }
}

