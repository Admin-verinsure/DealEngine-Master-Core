using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DealEngine.WebUI.Helpers.CustomHtml.TableFor.Interfaces;

namespace DealEngine.WebUI.Helpers.CustomHtml.TableFor
{
	public class TableCommand<TModel, TProperty> : ITableCommand, ITableCommandInternal<TModel> where TModel : class
	{
		protected string _action;
		protected string _text;

		public KeyValuePair<string, string> DeleteAction { get; set; }

		public KeyValuePair<string, string> EditAction { get; set; }

		public KeyValuePair<string, string> ViewAction { get; set; }

		public IList<KeyValuePair<string, string>> CustomActions { get; set; }

		public IList<KeyValuePair<string, string>> Events { get; set; }

		public string Classes { get; set; }

		/// <summary>
		/// Compiled lambda expression to get the property value from a model object.
		/// </summary>
		public Func<TModel, TProperty> CompiledExpression { get; set; }

		public TableCommand (Expression<Func<TModel, TProperty>> expression)
		{
			CompiledExpression = expression.Compile ();
			CustomActions = new List<KeyValuePair<string, string>> ();
			Events = new List<KeyValuePair<string, string>> ();
		}

		public ITableCommand Custom (string action, string text)
		{
			CustomActions.Add (new KeyValuePair<string, string> (text, action));
			return this;
		}

		public ITableCommand Event (string jsEvent, string text)
		{
			Events.Add (new KeyValuePair<string, string> (text, jsEvent));
			return this;
		}

		public ITableCommand Delete (string action)
		{
			DeleteAction = new KeyValuePair<string, string>("Delte", action);
			return this;
		}

		public ITableCommand Edit (string action)
		{
			EditAction = new KeyValuePair<string, string> ("Edit", action);
			return this;
		}

		public ITableCommand View (string action)
		{
			ViewAction = new KeyValuePair<string, string> ("View", action);
			return this;
		}

		public ITableCommand Class (string classes)
		{
			Classes = classes;
			return this;
		}

		/// <summary>
		/// Get the property value from a model object.
		/// </summary>
		/// <param name="model">Model to get the property value from.</param>
		/// <returns>Property value from the model.</returns>
		public string Evaluate (TModel model)
		{
			var result = this.CompiledExpression (model);
			return result == null ? string.Empty : result.ToString ();
		}
	}
}

