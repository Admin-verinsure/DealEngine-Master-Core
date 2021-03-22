using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealEngine.WebUI.Models
{
    public class UpdateTypesViewModel : BaseViewModel
    {
        public string ValueType { get; set; }
        public string NameType { get; set; }
        //public bool userIs { get; set; }
        // public int Id { get; set; }
        public Guid Id { get; set; }


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
