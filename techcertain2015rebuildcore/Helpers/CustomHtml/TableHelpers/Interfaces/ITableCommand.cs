using System;
namespace techcertain2015rebuildcore.Helpers.CustomHtml.TableFor.Interfaces
{
	public interface ITableCommand
	{
		ITableCommand Custom (string action, string text);
		ITableCommand Event (string jsEvent, string text);
		ITableCommand Delete (string action);
		ITableCommand Edit (string action);
		ITableCommand View (string action);
		ITableCommand Class (string classes);
	}
}

