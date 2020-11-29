using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace DealEngine.WebUI.Models
{
	public class DocumentViewModel : BaseViewModel
	{
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
		public string Description { get; set; }
		public int DocumentType { get; set; }
		public string Content { get; set; }
        public string ProductId { get; set; }
		public bool RenderToPDF { get; set; }
		//public IList<Product> Products { get; set; }
		//public Product Product { get; set; }
		public DocumentViewModel ()
		{
		}
	}

	public class DocumentInfoViewModel : BaseViewModel
	{
		public Guid Id { get; set; }
		public string DisplayName { get; set; }
		public string Type { get; set; }
        public string ProductName { get; set; }
        public string Owner { get; set; }
		public string ProductId { get; set; }
	}
}

