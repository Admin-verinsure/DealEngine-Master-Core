using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;
using DealEngine.Domain.Entities;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
	public class DocumentMapping : IAutoMappingOverride<Document>
	{
		readonly int kb = 1024;

		public void Override (AutoMapping<Document> mapping)
		{
			mapping.Map (x => x.Contents).CustomType ("BinaryBlob").Length (100 * kb);
		}
	}

	public class ImageMapping : IAutoMappingOverride<Image>
	{
		readonly int kb = 1024;

		public void Override (AutoMapping<Image> mapping)
		{
			mapping.Map (x => x.Contents).CustomType ("BinaryBlob").Length (500 * kb).LazyLoad();
		}
	}
}

