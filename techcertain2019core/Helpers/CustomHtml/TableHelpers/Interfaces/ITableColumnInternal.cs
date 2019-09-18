using System;

namespace techcertain2019core.Helpers.CustomHtml.TableFor.Interfaces
{
	/// <summary>
	/// Properties and methods used within the TableBuilder class.
	/// </summary>
	public interface ITableColumnInternal<TModel> where TModel : class
	{
		string ColumnName { get; set; }
		string ColumnTitle { get; set; }
		string ColumnIcon { get; set; }
		string ColumnType { get; set; }
		bool ColumnVisible { get; set; }
		string Evaluate(TModel model);
	}
}

