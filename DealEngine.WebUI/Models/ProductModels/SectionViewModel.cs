using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.ProductModels
{
    public class SectionViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ProgrammeName { get; set; }
        public IEnumerable<InformationItem> Items { get; set; }

        public string CustomView { get; set; }

        public int Position { get; set; }
    }


}

