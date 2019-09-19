using System;

namespace TechCertain.WebUI.Helpers.CustomHtml.TableFor.Interfaces
{
	/// <summary>
	/// Properties and methods used by the consumer to configure the TableColumn.
	/// </summary>
	public interface ITableColumn
	{
		ITableColumn Title(string title);
		ITableColumn Icon(string icon);
		ITableColumn InputType (string type);
		ITableColumn Visible(bool visible);
	}
}

