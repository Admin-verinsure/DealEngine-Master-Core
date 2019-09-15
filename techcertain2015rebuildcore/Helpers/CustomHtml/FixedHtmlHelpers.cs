using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using techcertain2015rebuildcore.Helpers.CustomHtml.FixedHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace techcertain2015rebuildcore.Helpers.CustomHtml
{
	/// <summary>
	/// Contains corrected helpers since the Mono ones currently don't work properly.
	/// </summary>
	public static class FixedHtmlHelpers
	{
		#region HtmlHelpers

		public static FixedActionLink FixedActionLink(this HtmlHelper helper, string text, string action)
		{
			return new FixedActionLink (helper, text, action);
		}

		public static FixedActionLink FixedActionLink(this HtmlHelper helper, string text, string action, string controller)
		{
			return new FixedActionLink (helper, text, action, controller);
		}

		#endregion

		#region UrlHelpers

		public static FixedUrlAction FixedAction(this UrlHelper helper, string action)
		{
			return new FixedUrlAction (helper, action, "", null, "");
		}

		public static FixedUrlAction FixedAction(this UrlHelper helper, string action, string controller)
		{
			return new FixedUrlAction (helper, action, controller, null, "");
		}

		public static FixedUrlAction FixedAction(this UrlHelper helper, string action, string controller, RouteValueDictionary routeValues)
		{
			return new FixedUrlAction (helper, action, controller, routeValues, "");
		}

		#endregion

		#region AjaxHelpers

		//public static FixedAjaxActionLink FixedActionLink(this AjaxHelper helper, string text, string action, AjaxOptions options)
		//{
		//	return new FixedAjaxActionLink (helper, text, action, "", null, options);
		//}

		//public static FixedAjaxActionLink FixedActionLink(this AjaxHelper helper, string text, string action, RouteValueDictionary routeValues, AjaxOptions options)
		//{
		//	return new FixedAjaxActionLink (helper, text, action, "", routeValues, options);
		//}

		#endregion
	}
}

