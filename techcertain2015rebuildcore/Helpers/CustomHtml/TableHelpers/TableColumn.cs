using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using techcertain2019core.Helpers.CustomHtml.TableFor.Interfaces;

namespace techcertain2019core.Helpers.CustomHtml.TableFor
{
	/// <summary>
	/// Represents a column in a table.
	/// </summary>
	/// <typeparam name="TModel">Class that is rendered in a table.</typeparam>
	/// <typeparam name="TProperty">Class property that is rendered in the column.</typeparam>
	public class TableColumn<TModel, TProperty> : ITableColumn, ITableColumnInternal<TModel> where TModel : class
	{
		public string ColumnName { get; set; }

		/// <summary>
		/// Column title to display in the table.
		/// </summary>
		public string ColumnTitle { get; set; }

		/// <summary>
		/// Column icon to display in the table.
		/// </summary>
		public string ColumnIcon { get; set; }

		public string ColumnType { get; set; }

		public bool ColumnVisible { get; set; }

		/// <summary>
		/// Compiled lambda expression to get the property value from a model object.
		/// </summary>
		public Func<TModel, TProperty> CompiledExpression { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="expression">Lambda expression identifying a property to be rendered.</param>
		public TableColumn(Expression<Func<TModel, TProperty>> expression)
		{
			string propertyName = (expression.Body as MemberExpression).Member.Name;
			this.ColumnName = System.Web.Mvc.ExpressionHelper.GetExpressionText (expression);
			this.ColumnTitle = Regex.Replace(propertyName, "([a-z])([A-Z])", "$1 $2");
			this.ColumnVisible = true;
			this.CompiledExpression = expression.Compile();
		}

		/// <summary>
		/// Set the title for the column.
		/// </summary>
		/// <param name="title">Title for the column.</param>
		/// <returns>Instance of a TableColumn.</returns>
		public ITableColumn Title(string title)
		{
			this.ColumnTitle = title;
			return this;
		}

		/// <summary>
		/// Set the icon for the column.
		/// </summary>
		/// <param name="icon">Icon for the column.</param>
		/// <returns>Instance of a TableColumn.</returns>
		public ITableColumn Icon(string icon)
		{
			this.ColumnIcon = icon;
			return this;
		}

		public ITableColumn InputType(string type)
		{
			this.ColumnType = type;
			return this;
		}

		public ITableColumn Visible(bool visible)
		{
			this.ColumnVisible = visible;
			return this;
		}

		/// <summary>
		/// Get the property value from a model object.
		/// </summary>
		/// <param name="model">Model to get the property value from.</param>
		/// <returns>Property value from the model.</returns>
		public string Evaluate(TModel model)
		{
			var result = this.CompiledExpression(model);
			return result == null ? string.Empty : result.ToString();
		}
	}
}

