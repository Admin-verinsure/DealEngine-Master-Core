
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TechCertain.WebUI.Helpers.CustomHtml.FixedHelpers
{
	public class FixedActionLink
    {
		protected HtmlHelper _htmlHelper;

		protected string _text;
		protected string _action;
		protected string _controller;

		FixedActionLink (HtmlHelper helper)
		{
			this._htmlHelper = helper;
			_action = "Index";
			_controller = "Home";
		}

		public FixedActionLink (HtmlHelper helper, string text, string action)
			: this (helper)
		{
			_text = text;
			Action (action);
		}

		public FixedActionLink (HtmlHelper helper, string text, string action, string controller)
			: this (helper, text, action)
		{
			Controller (controller);
		}

		public FixedActionLink Action(string action)
		{
			_action = action;
			return this;
		}

		public FixedActionLink Controller(string controller)
		{
			_controller = controller;
			return this;
		}

		#region IHtmlString implementation

		//public string ToHtmlString ()
		//{
		//	TagBuilder tag = new TagBuilder ("a");
		//	tag.SetInnerText(_text);

		//	string url = "/" + _controller + "/" + _action;
		//	tag.Attributes.Add ("href", url);

		//	return tag.ToString();
		//}

		#endregion
	}
}

