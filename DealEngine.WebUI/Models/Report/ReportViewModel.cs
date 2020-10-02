using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DealEngine.WebUI.Models.Report
{
    public class ReportViewModel : BaseViewModel
    {
        public ReportViewModel(List<Domain.Entities.Programme> programmes)
        {
            //if (programmes.Any())
            //{
            //    var clients = programmes.FirstOrDefault().ClientProgrammes.Where(c => c.DateDeleted == null).ToList();
            //    foreach(var client in clients)
            //    {
            //        SelectedClients.Add(
            //            new SelectListItem()
            //            {
            //                Text = client.Owner.Name,
            //                Value = client.Id.ToString()
            //            });
            //    }
            //}
        }

        public IList<SelectListItem> SelectedClients { get; set; }

    }
}
