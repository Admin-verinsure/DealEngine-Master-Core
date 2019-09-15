using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using techcertain2015rebuildcore.Helpers.CustomHtml.TableFor.Interfaces;

namespace techcertain2015rebuildcore.Helpers.CustomHtml.TableFor
{
	public class InputTableBuilder<TModel> : TableBuilder<TModel> where TModel : class
	{
		internal InputTableBuilder (HtmlHelper helper) : base (helper)
		{
		}

		protected override XElement CreateDataCell (ITableColumnInternal<TModel> column, TModel model)
		{
			XElement td = new XElement ("td");

			string value = column.Evaluate (model);
			//if (value == bool.FalseString || value == bool.TrueString)
			//	value = bool.TrueString;

			string prefix = HtmlHelper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
			int index = this.Data.ToList ().IndexOf (model);

			if (!string.IsNullOrWhiteSpace (column.ColumnType)) {
				XElement div = new XElement ("div",
					new XElement ("label",
						new XElement ("input",
							new XAttribute ("class", "checkbox style-0"),
							new XAttribute ("type", column.ColumnType),
							new XAttribute ("name", string.Format ("{0}[{1}].{2}", prefix, index, column.ColumnName)),
							new XAttribute ("id", string.Format ("{0}_{1}__{2}", prefix, index, column.ColumnName)),
				            new XAttribute ("value", value)
							),
						new XElement ("span", "Select")	// TODO - only show this when user requests it
						)
					);
				// set additional attributes on the input
				XElement input = div.Element ("label").Element ("input");
				// radiobuttons and checkboxes
				if (column.ColumnType == "radio" || column.ColumnType == "checkbox") {
					input.Attribute ("value").Value = bool.TrueString;
					if (value == bool.TrueString)
						input.Add (new XAttribute ("checked", ""));
				}



				td.Add (div);
			}
			else
				td.Add (new XElement ("label",
							new XAttribute ("name", string.Format ("{0}[{1}].{2}", prefix, index, column.ColumnName)),
							new XAttribute ("id", string.Format ("{0}_{1}__{2}", prefix, index, column.ColumnName)),
				            value));

			return td;
		}
	}
}

