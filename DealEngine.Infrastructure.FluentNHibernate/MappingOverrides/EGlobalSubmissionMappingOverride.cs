﻿using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class EGlobalSubmissionMappingOverride : IAutoMappingOverride<EGlobalSubmission>
    {
        public void Override(AutoMapping<EGlobalSubmission> mapping)
        {
            mapping.Map(x => x.SubmissionRequestXML).Length(100000);
        }
    }
}
