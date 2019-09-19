using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels.Product
{

    public class TerritoryViewModel : BaseViewModel
    {
        public TerritoryBuilderVM Builder { get; set; }

        public TerritoryAttachVM TerritoryAttach { get; set; }



    };

    public class TerritoryBuilderVM
    {
        public string Location { get; set; }

        public string[] SelectedInclorExcl { get; set; }

        //public RiskEntityViewModel [] Risks { get; set; }
        public IList<SelectListItem> BaseExclIncl { get; set; }

        public Boolean Ispublic { get; set; }

        public int Zoneorder { get; set; }


    }

    public class TerritoryAttachVM
    {

        public string[] SelectedProgramme { get; set; }

        //public RiskEntityViewModel [] Risks { get; set; }
        public IList<SelectListItem> BaseProgList { get; set; }

      

    }



}

