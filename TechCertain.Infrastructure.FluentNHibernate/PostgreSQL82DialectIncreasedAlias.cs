using System;
using NHibernate.Dialect;

namespace TechCertain.Infrastructure.FluentNHibernate
{
	public class PostgreSQL82DialectIncreasedAlias : PostgreSQL82Dialect
	{
		public override int MaxAliasLength {
			get {
				return 64;
			}
		}
	}
}

