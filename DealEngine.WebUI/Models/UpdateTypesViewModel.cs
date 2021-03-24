using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class UpdateTypesViewModel : BaseViewModel
    {
        public string ValueType { get; set; }
        public string NameType { get; set; }
        public object DateDeleted { get; internal set; }
        //public bool userIs { get; set; }
        // public int Id { get; set; }
        public Guid Id { get; set; }
        //public List<EditTermsViewModel> CLTerms { get; internal set; }
        public List<UpdateTypesViewModel> UpdateTypes { get; set; }


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

        //public static UpdateTypesViewModel FromEntity(UpdateTypesViewModel updateType)
        //{
        //    UpdateTypesViewModel model = new UpdateTypesViewModel
        //    {
        //        Id = updateType.Id,
        //        NameType = updateType.NameType,
        //        ValueType = updateType.ValueType,
        //        TypeIsBroker = updateType.TypeIsBroker,
        //        TypeIsClient = updateType.TypeIsClient,
        //        TypeIsInsurer = updateType.TypeIsInsurer,
        //        TypeIsTc = updateType.TypeIsTc
        //    };
        //    return model;
        //}

        //internal static UpdateTypesViewModel FromEntity(UpdateType pg)
        //{
        //    //throw new NotImplementedException();
        //    UpdateTypesViewModel model = new UpdateTypesViewModel
        //    {
        //        Id = pg.Id,
        //        NameType = pg.TypeName,
        //        ValueType = pg.TypeValue,
        //        TypeIsBroker = pg.TypeIsBroker,
        //        TypeIsClient = pg.TypeIsClient,
        //        TypeIsInsurer = pg.TypeIsInsurer,
        //        TypeIsTc = pg.TypeIsTc
        //    };
        //    return model;
        //}
    }
}
