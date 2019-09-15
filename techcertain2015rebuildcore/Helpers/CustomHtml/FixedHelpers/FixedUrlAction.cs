using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Web;



namespace techcertain2019core.Helpers.CustomHtml.FixedHelpers
{
	public class FixedUrlAction 
	{
		protected UrlHelper _urlHelper;

		protected string _action = "Index";
		protected string _controller = "Home";

		protected RouteValueDictionary _routeValues;
		protected string _protocol;

		public FixedUrlAction (UrlHelper helper, string action, string controller, RouteValueDictionary routeValues, string protocol)
		{
            throw new Exception("Method needs to be re-written");
			//this._urlHelper = helper;
			//RouteData routeData = _urlHelper.RequestContext.RouteData;
			//_action = !string.IsNullOrEmpty(action) ? action : routeData.GetRequiredString("action");
			//_controller = !string.IsNullOrEmpty(controller) ? controller : routeData.GetRequiredString("controller");

			//_routeValues = routeValues;
			//_protocol = protocol;
		}

		#region IHtmlString implementation

		public string ToHtmlString ()
		{
			string url = "/" + _controller + "/" + _action;

			if (!string.IsNullOrEmpty (_protocol))
				url = _protocol + ":/" + url;

			if (_routeValues != null)
				foreach (var kvp in _routeValues)
					url = url + "/" + kvp.Value;

			return url;
		}

		#endregion
	}
}

