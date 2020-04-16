using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class ClientAgreementEndorsementMappingOverride : IAutoMappingOverride<ClientAgreementEndorsement>
    {
        public void Override(AutoMapping<ClientAgreementEndorsement> mapping)
        {
            mapping.Map(x => x.Value).Length(4000);
        }
    }
}
