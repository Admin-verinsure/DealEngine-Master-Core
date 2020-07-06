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
                    Value = "Boat"
                });
            }
            if (companyName == "AIB")
            {
                SearchOptions.Add(new SelectListItem
                {
                    Text = "Advisor Name",
                    Value = "Advisory"
                });
            }
            SearchOptions.Add(new SelectListItem
            {
                Text = "Reference Number",
                Value = "Reference"
            });
            SearchOptions.Add(new SelectListItem
            {
                Text = "Insured First Name",
                Value = "Name"
            });
            SearchOptions.Add(new SelectListItem
            {
                Text = "Insured Last Name",
                Value = "Name"
            });

            return SearchOptions;
        }

        public IList<SelectListItem> SearchOptions;
    }
}

