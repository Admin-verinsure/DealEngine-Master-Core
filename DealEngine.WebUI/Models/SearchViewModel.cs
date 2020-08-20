using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DealEngine.WebUI.Models
{
    public class SearchViewModel
    {

        public SearchViewModel(List<Domain.Entities.Programme> programmes)
        {
            SearchOptions = GetSearchOptions();
            Programmes = programmes;
        }

        private IList<SelectListItem> GetSearchOptions()
        {
            SearchOptions = new List<SelectListItem>();
            //if(companyName == "Marsh")
            //{
            //    SearchOptions.Add(new SelectListItem
            //    {
            //        Text = "Boat Name",
            //        Value = "Boat"
            //    });
            //}
            SearchOptions.Add(new SelectListItem
            {
                Text = "Advisor Name",
                Value = "Advisor"
            });            
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
        public List<Domain.Entities.Programme> Programmes;
        private IList<System.Security.Claims.Claim> claims;
    }
}

