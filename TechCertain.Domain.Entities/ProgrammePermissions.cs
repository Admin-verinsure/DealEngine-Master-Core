using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class ProgrammePermissions : ValueObject
    {
        public virtual ApplicationRole ViewInformationRole { get; set; }

        public virtual ApplicationRole EditUserRole { get; set; }

        public ProgrammePermissions() { }
    }
}

