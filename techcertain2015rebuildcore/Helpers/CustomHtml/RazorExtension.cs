using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using techcertain2015rebuildcore.Helpers.CustomHtml.TableFor;
using techcertain2015rebuildcore.Helpers.CustomHtml.TableFor.Interfaces;

//using TwitterBootstrapMVC;
//using TwitterBootstrapMVC.Controls;

namespace techcertain2015rebuildcore.Helpers.CustomHtml
{
	public static class RazorExtension
	{
//		public static TableBuilder GetTable(this HtmlHelper self)
//		{
//			return new TableBuilder (self);
//		}

		//public static GridViewBuilder GetGridView(this HtmlHelper self)
		//{
		//	return new GridViewBuilder (self);
		//}
		/// <summary>
		/// Return an instance of a TableBuilder.
		/// </summary>
		/// <typeparam name="TModel">Type of model to render in the table.</typeparam>
		/// <returns>Instance of a TableBuilder.</returns>
		public static ITableBuilder<TModel> TableFor<TModel>(this HtmlHelper helper) where TModel : class
		{
			return new TableBuilder<TModel>(helper);
		}

		//[Obsolete]
		//public static ITableBuilder<TModel> SelectTableFor<TModel>(this HtmlHelper helper) where TModel : class
		//{
		//	return new SelectTableBuilder<TModel>(helper);
		//}

		//public static ITableBuilder<TModel> InputTableFor<TModel> (this HtmlHelper helper) where TModel : class
		//{
		//	return new InputTableBuilder<TModel> (helper);
		//}

		static void Sample()
		{
			

			// don't need the 'hh' in a razor view - only here since this is a sample code block
			// create must be called last to create the control
//			using (var table = GetTable (hh).Caption("This is a sample table").Advanced().Create ()) {
//				using (var header = table.TableHeader ().Sortable().Create ()) {
//					header.AddHeader ("header1");
//					header.AddHeaderRow ("header2", "header3");
//				}
//				table.TableRow ("data1", "data2", "data3", "data4").Color(FlatyColor.Lime).Create();
//				using (var row = table.TableRow ().Create()) {
//					row.AddCell ("test");
//					row.AddCell ("blah").ColSpan("2");
//				}
//				table.TableRow ("arg1", "arg2", "arg3", "arg4").Color(FlatyColor.Red).Create();
//			}
//
//			using (var grid = GetGridView (hh).Caption ("GridView Test").Create ()) {
//				grid.DataSource (null);
//			}

			//using (var table = new TwitterBootstrapMVC.BootstrapMethods.V3.Bootstrap<TechCertain.Web.UI.ViewModels.ProposalViewAllViewModel>(hh).Begin (
//			using (var table = TwitterBootstrap3.BootstrapHtmlExtension.Bootstrap<TechCertain.Web.UI.ViewModels.ProposalViewAllViewModel>().Begin(new TwitterBootstrapMVC.Table ())) {
//				using (var head = table.BeginHeader ()) {
//					using (var row = head.BeginHeaderRow ()) {
//						for (int i = 0; i < 5; i++)
//							row.Cell ("");
//					}
//				}
//				using (var body = table.BeginBody ()) {
//					
//				}
//			}

		}
	}
}

