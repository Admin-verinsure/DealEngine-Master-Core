



using Microsoft.AspNetCore.Routing;
using System.Web.Mvc.Ajax;
using System.Web.Mvc;
using System;

namespace techcertain2019core.Helpers.CustomHtml.FixedHelpers
{
	public class FixedAjaxActionLink 
	{
		protected AjaxHelper _ajaxHelper;
		protected AjaxOptions _options;

		protected string _text;
		protected string _cssClass;
		protected string _action = "Index";
		protected string _controller = "Home";

		protected RouteValueDictionary _routeValues;

		public FixedAjaxActionLink (AjaxHelper helper, string text, string action, string controller, RouteValueDictionary routeValues, AjaxOptions options)
		{
            throw new Exception("Method needs to be re-written");
			//_ajaxHelper = helper;

			//_cssClass = "";

			//RouteData routeData = _ajaxHelper.ViewContext.RouteData;
			//_text = text;
			//_action = !string.IsNullOrEmpty(action) ? action : routeData.GetRequiredString("action");
			//_controller = !string.IsNullOrEmpty(controller) ? controller : routeData.GetRequiredString("controller");

			//_routeValues = routeValues;

			//_options = options;
		}

		public FixedAjaxActionLink Action(string action)
		{
			_action = action;
			return this;
		}

		public FixedAjaxActionLink Class(string cssClass)
		{
			_cssClass += cssClass;
			return this;
		}

		#region IHtmlString implementation

		public string ToHtmlString ()
		{
			TagBuilder tag = new TagBuilder ("a");
			tag.SetInnerText(_text);
			tag.AddCssClass (_cssClass);

			// render ajax attributes
			if (_options != null) {
				tag.Attributes.Add ("data-ajax", "true");
				if (!string.IsNullOrWhiteSpace (_options.HttpMethod))
					tag.Attributes.Add ("data-ajax-method", _options.HttpMethod);
				tag.Attributes.Add ("data-ajax-mode", _options.InsertionMode.ToString ());
				if (!string.IsNullOrWhiteSpace (_options.UpdateTargetId))
					tag.Attributes.Add ("data-ajax-update", "#" + _options.UpdateTargetId);
				// events
				if (!string.IsNullOrWhiteSpace (_options.OnBegin))
					tag.Attributes.Add ("data-ajax-begin", _options.OnBegin);
				if (!string.IsNullOrWhiteSpace (_options.OnComplete))
					tag.Attributes.Add ("data-ajax-complete", _options.OnComplete);
				if (!string.IsNullOrWhiteSpace (_options.OnFailure))
					tag.Attributes.Add ("data-ajax-failure", _options.OnFailure);
				if (!string.IsNullOrWhiteSpace (_options.OnSuccess))
					tag.Attributes.Add ("data-ajax-success", _options.OnSuccess);
			}

			string url = "/" + _controller + "/" + _action;

			if (_routeValues != null)
				foreach (var kvp in _routeValues)
					url = url + "/" + kvp.Value;
			
			tag.Attributes.Add ("href", url);

			return tag.ToString();
		}

		#endregion
	}
}

