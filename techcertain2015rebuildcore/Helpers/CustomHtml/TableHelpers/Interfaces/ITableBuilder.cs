using System;
using System.Collections.Generic;

namespace techcertain2019core.Helpers.CustomHtml.TableFor.Interfaces
{
	/// <summary>
	/// Properties and methods used by the consumer to configure the TableBuilder.
	/// </summary>
	public interface ITableBuilder<TModel> where TModel : class
	{
		TableBuilder<TModel> DataSource(IEnumerable<TModel> dataSource);
		TableBuilder<TModel> Columns(Action<ColumnBuilder<TModel>> columnBuilder);

		TableBuilder<TModel> SetId (string id);
		TableBuilder<TModel> Hover ();
		TableBuilder<TModel> Striped ();
	}
}

