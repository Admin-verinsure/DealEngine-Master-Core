using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace techcertain2015rebuildcore.Models.ViewModels.ControlModels
{
	/// <summary>
	/// Defines a ViewModel for a jquery Grid (jqGrid).
	/// </summary>
	public class JqGridViewModel
	{
		List<JqGridRow> _rows;
		XDocument _document;
		XElement _root;

		public int Page { get; set; }

		public int TotalPages { get; set; }

		public int TotalRecords { get; set; }

		public JqGridViewModel ()
		{
			_rows = new List<JqGridRow> ();
			_document = new XDocument (new XElement ("rows"));
			_root = _document.Root;
			Page = 1;
			TotalPages = 1;
			TotalRecords = 0;
		}

		/// <summary>
		/// Adds a JqGridRow to this View Model.
		/// </summary>
		/// <param name="row">JqGridRow to add.</param>
		public void AddRow(JqGridRow row)
		{
			_rows.Add (row);
		}

		/// <summary>
		/// Formats the JqGridViewModel to an XDocument for rendering.
		/// </summary>
		/// <returns>The XDocument.</returns>
		public XDocument ToXml()
		{
			_root.Add(new XElement("page", Page));
			_root.Add(new XElement("total", TotalPages));
			_root.Add(new XElement("records", TotalRecords));

			foreach (JqGridRow row in _rows)
				_root.Add (row.ToXml ());

			return _document;
		}
	}

	/// <summary>
	/// Defines a row in a jqGrid.
	/// </summary>
	public class JqGridRow
	{
		XElement _row;

		public object RowId { get; private set; }

		public JqGridRow(object Id)
		{
			RowId = Id;
			_row = new XElement ("row");
			_row.SetAttributeValue ("id", RowId);
		}

		/// <summary>
		/// Adds the specified value to the JqGridRow.
		/// </summary>
		/// <param name="value">The value to be added.</param>
		public void AddValue(object value)
		{
			_row.Add (new XElement ("cell", value));
		}

		/// <summary>
		/// Adds the specified values to the JqGridRow.
		/// </summary>
		/// <param name="values">The values to be added.</param>
		public void AddValues(params object[] values)
		{
			for (int i = 0; i < values.Length; i++)
				AddValue (values [i]);
		}

		/// <summary>
		/// Formats the JqGridRow to an XElement for rendering.
		/// </summary>
		/// <returns>The XElement.</returns>
		public XElement ToXml()
		{
			return _row;
		}
	}
}

