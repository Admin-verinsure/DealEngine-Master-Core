using System;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.Xml.Linq;
using TechCertain.WebUI.Helpers.CustomHtml.TableFor.Interfaces;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TechCertain.WebUI.Helpers.CustomHtml.TableFor
{
	/// <summary>
	/// Build a table based on an enumerable list of model objects.
	/// </summary>
	/// <typeparam name="TModel">Type of model to render in the table.</typeparam>
	public class TableBuilder<TModel> : ITableBuilder<TModel> where TModel : class
	{
		protected HtmlHelper HtmlHelper { get; set; }
		protected IEnumerable<TModel> Data { get; set; }

		protected string Id { get; set; }

		protected bool IsHover { get; set; }
		protected bool IsStriped { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		private TableBuilder()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal TableBuilder(HtmlHelper helper)
		{
			this.HtmlHelper = helper;

			this.TableColumns = new List<ITableColumnInternal<TModel>>();
		}

		/// <summary>
		/// Set the enumerable list of model objects.
		/// </summary>
		/// <param name="dataSource">Enumerable list of model objects.</param>
		/// <returns>Reference to the TableBuilder object.</returns>
		public TableBuilder<TModel> DataSource(IEnumerable<TModel> dataSource)
		{
			this.Data = dataSource;
			return this;
		}

		/// <summary>
		/// List of table columns to be rendered in the table.
		/// </summary>
		internal IList<ITableColumnInternal<TModel>> TableColumns { get; set; }

		/// <summary>
		/// Add an lambda expression as a TableColumn.
		/// </summary>
		/// <typeparam name="TProperty">Model class property to be added as a column.</typeparam>
		/// <param name="expression">Lambda expression identifying a property to be rendered.</param>
		/// <returns>An instance of TableColumn.</returns>
		internal ITableColumn AddColumn<TProperty>(Expression<Func<TModel, TProperty>> expression)
		{
            throw new Exception("Method needs to be re-written");
			//TableColumn<TModel, TProperty> column = new TableColumn<TModel, TProperty>(expression);
			//this.TableColumns.Add(column);
			//return column;
		}

		internal ITableCommandInternal<TModel> TableCommands { get; set; }

		internal ITableCommand SetCommands<TProperty> (Expression<Func<TModel, TProperty>> expression)
		{
			TableCommand<TModel, TProperty> command = new TableCommand<TModel, TProperty> (expression);
			TableCommands = command;
			return command;
		}

		/// <summary>
		/// Create an instance of the ColumnBuilder to add columns to the table.
		/// </summary>
		/// <param name="columnBuilder">Delegate to create an instance of ColumnBuilder.</param>
		/// <returns>An instance of TableBuilder.</returns>
		public TableBuilder<TModel> Columns(Action<ColumnBuilder<TModel>> columnBuilder)
		{
			ColumnBuilder<TModel> builder = new ColumnBuilder<TModel>(this);
			columnBuilder(builder);
			return this;
		}

		/// <summary>
		/// Renders the TableBuilder to output stream.
		/// </summary>
		public virtual void Render()
		{
			HtmlHelper.ViewContext.Writer.Write (ToHtmlString());
		}

		public TableBuilder<TModel> SetId (string id)
		{
			Id = id;
			return this;
		}

		public TableBuilder<TModel> Hover ()
		{
			IsHover = true;
			return this;
		}

		public TableBuilder<TModel> Striped ()
		{
			IsStriped = true;
			return this;
		}

		public string ToHtmlString()
		{
			XDocument html = new XDocument();
			XElement table = new XElement ("table");
			string _class = string.Join (" ", "table", IsHover ? "table-hover" : "", IsStriped ? "table-striped" : "");
			if (!string.IsNullOrWhiteSpace (Id)) table.SetAttributeValue ("id", Id);
			table.SetAttributeValue ("class", _class);
			html.Add(table);

			table.Add (CreateHeaderRow());
			table.Add (CreateBodyElement ());

			return html.ToString();
		}

		public override string ToString ()
		{
			return ToHtmlString();
		}

		protected virtual XElement CreateHeaderRow()
		{
			XElement thead = new XElement("thead");
			XElement tr = new XElement ("tr");
			thead.Add(tr);

			foreach (ITableColumnInternal<TModel> tc in TableColumns)
			{
				XElement th = new XElement ("th");
				if (tc.ColumnVisible == true) {
					if (!string.IsNullOrEmpty (tc.ColumnIcon)) {
						XElement i = new XElement ("i");
						i.Value = "";
						//i.IsEmpty = false;
						i.SetAttributeValue ("class", string.Format ("fa fa-fw fa-{0}", tc.ColumnIcon));
						th.Add (i);
					}
					th.Value += tc.ColumnTitle;
				}
				tr.Add (th);
			}
			if (TableCommands != null)
				tr.Add(new XElement ("th"));
			return thead;
		}

		protected virtual XElement CreateBodyElement()
		{
			XElement tbody = new XElement ("tbody");

			if (this.Data != null)
			{
				foreach (TModel model in this.Data)
				{
					XElement tr = new XElement ("tr");
					tbody.Add (tr);

					foreach (ITableColumnInternal<TModel> tc in this.TableColumns)
					{
						tr.Add (CreateDataCell (tc, model));
					}
					if (TableCommands != null)
					{
						string value = TableCommands.Evaluate (model);
						// add links here
						XElement td = new XElement ("td");
						tr.Add (td);
						CreateCommandCell (td, TableCommands, TableCommands.ViewAction, value);
						CreateCommandCell (td, TableCommands, TableCommands.EditAction, value);
						CreateCommandCell (td, TableCommands, TableCommands.DeleteAction, value);
						foreach (KeyValuePair<string, string> kvp in TableCommands.CustomActions)
							CreateCommandCell (td, TableCommands, kvp, value);
						foreach (KeyValuePair<string, string> kvp in TableCommands.Events)
							CreateEventsCell (td, TableCommands, kvp, value);
					}
				}
			}


			return tbody;
		}

		protected virtual XElement CreateDataCell (ITableColumnInternal<TModel> column, TModel model)
		{
			XElement td = new XElement ("td");
			string value = column.Evaluate (model);
			td.Value = value;
			return td;
		}

		protected virtual void CreateCommandCell (XElement cell, ITableCommandInternal<TModel> command, KeyValuePair<string, string> action, string value)
		{
            throw new Exception("Methods needs to be re-written");
			//if (!string.IsNullOrWhiteSpace(action.Key)) {
			//	string controller = (HtmlHelper.ViewContext.RequestContext.RouteData.Values ["controller"] ?? string.Empty).ToString ();
			//	XElement a = new XElement ("a");
			//	cell.Add (a);
			//	a.Add (new XAttribute ("href", string.Join ("/", "", controller, action.Value, value)), action.Key);
			//	if (!string.IsNullOrWhiteSpace (command.Classes))
			//		a.Add (new XAttribute ("class", command.Classes));
			//}
		}

		protected virtual void CreateEventsCell (XElement cell, ITableCommandInternal<TModel> command, KeyValuePair<string, string> action, string value)
		{
			if (!string.IsNullOrWhiteSpace (action.Key)) {
				XElement button = new XElement ("button");
				cell.Add (button);
				button.Add (new XAttribute ("type", "button"));
				if (!string.IsNullOrWhiteSpace (command.Classes))
					button.Add (new XAttribute ("class", command.Classes));
				string [] parts = action.Value.Split ('=');
				if (parts.Length > 1) {
					string formattedCmd = parts [1].Replace ("$id$", value);
					button.Add (new XAttribute (parts [0], formattedCmd), action.Key);
				}
			}
		}
	}
}

