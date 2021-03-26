using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHibernate.AspNetCore.Identity;

namespace DealEngine.WebUI.Models
{
    public class UpdateTypesViewModel : BaseViewModel
    {
        public UpdateTypesViewModel() { }
        //public UpdateTypesViewModel(UpdateType UpdateTypes) {
        //    if(UpdateTypes != null)
        //    {
               
        //    }
        //}

        public string ValueType { get; set; }
        public string NameType { get; set; }
        public object DateDeleted { get; internal set; }
        //public bool userIs { get; set; }
        // public int Id { get; set; }
        public Guid Id { get; set; }
        //public List<EditTermsViewModel> CLTerms { get; internal set; }
        public List<UpdateTypesViewModel> UpdateTypes { get; set; }
        public List<UpdateTypesViewModel> updateType { get; set; }


        public virtual bool TypeIsTc
        {
            get;
            set;
        }
        public virtual bool TypeIsBroker
        {
            get;
            set;
        }
        public virtual bool TypeIsInsurer
        {
            get;
            set;
        }
        public virtual bool TypeIsClient
        {
            get;
            set;
        }
    }
}
