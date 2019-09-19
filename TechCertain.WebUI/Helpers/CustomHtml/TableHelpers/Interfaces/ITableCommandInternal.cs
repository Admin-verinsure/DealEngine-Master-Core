using System;
using System.Collections.Generic;

namespace TechCertain.WebUI.Helpers.CustomHtml.TableFor.Interfaces
{
	public interface ITableCommandInternal<TModel> where TModel : class
	{
		KeyValuePair<string, string> DeleteAction { get; set; }
		KeyValuePair<string, string> EditAction { get; set; }
		KeyValuePair<string, string> ViewAction { get; set; }
		IList<KeyValuePair<string, string>> CustomActions { get; set; }
		IList<KeyValuePair<string, string>> Events { get; set; }

		string Classes { get; set; }

		string Evaluate (TModel model);
	}
}

