using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DealEngine.WebUI.Models
{
    public class SearchViewModel
    {
        public SearchViewModel(string CompanyName)
        {
            SearchOptions = GetSearchOptions(CompanyName);
        }

        private IList<SelectListItem> GetSearchOptions(string companyName)
        {
            SearchOptions = new List<SelectListItem>();
            if(companyName == "Marsh")
            {
                SearchOptions.Add(new SelectListItem
                {
                    Text = "Boat Name",
                    Value = "1"
                });
            }
            if (companyName == "AIB")
            {
                SearchOptions.Add(new SelectListItem
                {
                    Text = "Advisory Name",
                    Value = "1"
                });
            }
            return SearchOptions;
        }

        public IList<SelectListItem> SearchOptions;
    }
}

