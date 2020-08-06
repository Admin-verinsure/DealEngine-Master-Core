using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DealEngine.WebUI.Models.Milestone
{
    public class MilestoneViewModel : BaseViewModel
    {
        public MilestoneViewModel(List<Domain.Entities.Programme> programmes, Domain.Entities.Milestone milestone)
        {
            Programmes = GetProgrammeList(programmes);
            ProgrammeProcesses = GetProgrammeProcesses();
            Activities = GetActivities();
            Milestone = milestone;
        }

        public Domain.Entities.Milestone Milestone { get; set; }
        public IList<SelectListItem> Activities { get; set; }
        public IList<SelectListItem> Programmes { get; set; }
        public IList<SelectListItem> ProgrammeProcesses { get; set; }
        private IList<SelectListItem> GetActivities()
        {
            var list = new List<SelectListItem>() {
                new SelectListItem
                {
                    Text = "-- Select --",
                    Value = "0"
                },
                new SelectListItem
                {
                    Text = "Agreement Status - Not Started",
                    Value = "Agreement Status - Not Started"
                },
                new SelectListItem
                {
                    Text = "Agreement Status - Started",
                    Value = "Agreement Status - Started"
                },
                new SelectListItem
                {
                    Text = "Agreement Status - Quoted",
                    Value = "Agreement Status - Quoted"
                },
                new SelectListItem
                {
                    Text = "Agreement Status – Referred",
                    Value = "Agreement Status – Referred"
                },
                new SelectListItem
                {
                    Text = "Agreement Status - Declined",
                    Value = "Agreement Status - Declined"
                },
                new SelectListItem
                {
                    Text = "Agreement Status - Completed",
                    Value = "Agreement Status - Completed"
                },
                new SelectListItem
                {
                    Text = "Agreement Status - Bound and Waiting Payment",
                    Value = "Agreement Status - Bound and Waiting Payment"
                },
                new SelectListItem
                {
                    Text = "Agreement Status - Bound and Waiting Invoice",
                    Value = "Agreement Status - Bound and Waiting Invoice"
                },
            };
            return list;
        }
        private IList<SelectListItem> GetProgrammeProcesses()
        {
            var list = new List<SelectListItem>() {
                    new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    },
                    new SelectListItem
                    {
                        Text = "Process New Agreement",
                        Value = "Process New Agreement"
                    },
                    new SelectListItem
                    {
                        Text = "Change Agreement",
                        Value = "Change Agreement"
                    },
                    new SelectListItem
                    {
                        Text = "Process Cancel Agreement",
                        Value = "Process Cancel Agreement"
                    },
                    new SelectListItem
                    {
                        Text = "Process Renewal Agreement",
                        Value = "Process Renewal Agreement"
                    }
                };
            return list;

        }
        private IList<SelectListItem> GetProgrammeList(List<Domain.Entities.Programme> programmes)
        {
            var list = new List<SelectListItem>();
            foreach (var programme in programmes)
            {
                list.Add(new SelectListItem()
                {
                    Text = programme.Name,
                    Value = programme.Id.ToString()
                });
            }
            return list;
        }
        public Advisory Advisory { get;set;}

    }

}
