using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Xml;
using System.Xml.Linq;
using techcertain2015rebuildcore.Helpers.CustomHtml.TableFor.Interfaces;

namespace techcertain2015rebuildcore.Helpers.CustomHtml.TableFor
{
	[Obsolete]
	public class SelectTableBuilder<TModel> : TableBuilder<TModel> where TModel : class
	{
		internal SelectTableBuilder (HtmlHelper helper) : base (helper)
		{

		}

		protected override XElement CreateDataCell (ITableColumnInternal<TModel> column, TModel model)
		{
			XElement td = new XElement ("td");

			string value = column.Evaluate (model);
			bool result = false;
			if (Boolean.TryParse (value, out result))
			{
				XElement i = new XElement ("i");
				i.Value = "";
				//i.InnerText = "";
				//i.IsEmpty = false;
				if (result == true)
					i.SetAttributeValue ("class", "fa fa-check fa-4x txt-color-green");
				else
					i.SetAttributeValue ("class", "fa fa-times fa-4x txt-color-red");
				td.Add (i);

				XElement div = new XElement ("div");

				XElement label = new XElement ("label");
				div.Add (label);

				XElement input = new XElement ("input");
				label.Add (input);
				input.SetAttributeValue ("class", "checkbox style-0");
				input.SetAttributeValue ("type", "checkbox");
				input.SetAttributeValue ("data-val", "true");
				input.SetAttributeValue ("value", "true");
				input.SetAttributeValue ("name", this.HtmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName (column.ColumnName));
				//input.SetAttributeValue ("id", this.HtmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId (column.ColumnName));

				XElement span = new XElement ("span");
				span.Value = "Select";
				label.Add (span);

				td.Add(div);
			}
			else
				td.Value = value;
			
			return td;
		}
	}
}

