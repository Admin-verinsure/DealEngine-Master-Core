using System;
using System.Linq.Expressions;
using techcertain2015rebuildcore.Helpers.CustomHtml.TableFor.Interfaces;

namespace techcertain2015rebuildcore.Helpers.CustomHtml.TableFor
{
	/// <summary>
	/// Create instances of TableColumns.
	/// </summary>
	/// <typeparam name="TModel">Type of model to render in the table.</typeparam>
	public class ColumnBuilder<TModel> where TModel : class
	{
		public TableBuilder<TModel> TableBuilder { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tableBuilder">Instance of a TableBuilder.</param>
		public ColumnBuilder(TableBuilder<TModel> tableBuilder)
		{
			TableBuilder = tableBuilder;
		}

		/// <summary>
		/// Add lambda expressions to the TableBuilder.
		/// </summary>
		/// <typeparam name="TProperty">Class property that is rendered in the column.</typeparam>
		/// <param name="expression">Lambda expression identifying a property to be rendered.</param>
		/// <returns>An instance of TableColumn.</returns>
		public ITableColumn Expression<TProperty>(Expression<Func<TModel, TProperty>> expression)
		{
			return TableBuilder.AddColumn(expression);
		}

		public ITableCommand Commands<TProperty> (Expression<Func<TModel, TProperty>> expression)
		{
			return TableBuilder.SetCommands (expression);
		}
	}

}

