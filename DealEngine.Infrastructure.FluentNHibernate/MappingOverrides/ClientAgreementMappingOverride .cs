using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class ClientAgreementMappingOverride : IAutoMappingOverride<ClientAgreement>
    {
        public void Override(AutoMapping<ClientAgreement> mapping)
        {
            mapping.Map(x => x.issuetobrokercomment).Length(10000);
        }
    }
}
