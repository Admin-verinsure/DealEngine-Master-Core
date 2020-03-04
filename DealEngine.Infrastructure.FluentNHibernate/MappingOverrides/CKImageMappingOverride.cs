using DealEngine.Domain.Entities;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class CKImageMappingOverride : IAutoMappingOverride<CKImage> {

        public void Override(AutoMapping<CKImage> mapping)
        {
            //mapping.IgnoreProperty(x => x.Image);
        }
    }
}
