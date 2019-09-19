using System;
using System.Collections.Generic;


namespace techcertain2019core.Models.ViewModels
{
	public class DashboardViewModel : BaseViewModel
	{
		public bool DisplayProducts { get; set; }

		public bool DisplayDeals { get; set; }

		public string DisplayRole { get; set; }

		public IList<ProductItemV2> ProductItems { get; set; }

		public IList<ProductItem> DealItems { get; set; }

		public IList<ProgrammeItem> ProgrammeItems { get; set; }

		public IList<TaskItem> CriticalTaskItems { get; set; }

		public IList<TaskItem> ImportantTaskItems { get; set; }

		public IList<TaskItem> CompletedTaskItems { get; set; }
	}

	public class ButtonItem
	{
		public string Text { get; set; }

		public string RedirectLink { get; set; }

		public string Classes { get; set; }
	}

	public class ProductItem
	{
        public string Name { get; set; }

		public string Description { get; set; }

		public IList<string> Languages { get; set; }

		public IList<ButtonItem> Buttons { get; set; }

		public string RedirectLink { get; set; }

		public string Status { get; set; }

		public bool HasProduct { get; set; }
	}

	public class ProductItemV2
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string SheetId { get; set; }

		public IList<string> Languages { get; set; }

		public IList<KeyValuePair<string, Guid>> SheetHistory { get; set; }

		public bool HasSheetHistory {
			get { return SheetHistory != null && SheetHistory.Count > 1; }
		}

		public string RedirectLink { get; set; }

		public string Status { get; set; }
	}

	public class DealItem
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Status { get; set; }

        public string ReferenceId { get; set; }

        public string Id { get; set; }

		public string LocalDateCreated { get; set; }

		public string LocalDateSubmitted { get; set; }

		public string GetStatusDisplay ()
		{
			List<string> statusDisplay = new List<string> ();

			if (!string.IsNullOrWhiteSpace (LocalDateCreated))
				statusDisplay.Add ("Created on " + LocalDateCreated);
			if (!string.IsNullOrWhiteSpace (LocalDateSubmitted))
				statusDisplay.Add ("Submitted on " + LocalDateSubmitted);

			return string.Join(", ", statusDisplay);
		}
	}

	public class ProgrammeItem : BaseViewModel
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string ProgrammeId { get; set; }

		public IList<string> Languages { get; set; }

		public string DisplayRole { get; set; }

		public IList<DealItem> Deals { get; set; }

        public string CurrentUserIsBroker { get; set; }

        public string CurrentUserIsInsurer { get; set; }

        public string CurrentUserIsTC { get; set; }
    }

	public class TaskItem
	{
        public virtual Guid Id { get; set; }
        public virtual string ClientName { get; set; }

		public virtual string Description { get; set; }

		public virtual string Details { get; set; }

		public virtual string TaskUrl { get; set; }

		public virtual int Priority { get; set; }

		public virtual string DueDate { get; set; }

		public virtual bool Completed { get; set; }
	}
}
    