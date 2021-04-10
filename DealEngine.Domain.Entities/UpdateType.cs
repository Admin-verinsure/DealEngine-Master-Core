using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DealEngine.Domain.Entities
{
    [JsonObject]
    public class UpdateType : EntityBase, IAggregateRoot
    {
        public UpdateType() : base(null) { }

        //public UpdateType(User createdBy, string typeName, string typeValue, bool typeIsTc, bool typeIsBroker, bool typeIsInsurer, bool typeIsClient, bool programmeIsFanz, bool programmeIsFmc)
        //    : base(createdBy)
        public UpdateType(User createdBy, string typeName, string typeValue, bool typeIsTc, bool typeIsBroker, bool typeIsInsurer, bool typeIsClient)
          : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException(nameof(typeName));

            if (string.IsNullOrWhiteSpace(typeValue))
                throw new ArgumentNullException(nameof(typeValue));


            TypeName = typeName;
            TypeValue = typeValue;
            TypeIsTc = typeIsTc;
            TypeIsInsurer = typeIsInsurer;
            TypeIsClient = typeIsClient;
            TypeIsBroker = typeIsBroker;
            //ProgrammeIsFanz = programmeIsFanz;
            //ProgrammeIsFmc = programmeIsFmc;
           //programmes = new List<Programme>();



        }

        [JsonIgnore]
        
        public virtual string TypeName
        {
            get;
            set;
        }

        public virtual string TypeValue
        {
            get;
            set;
        }
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
        //public virtual bool ProgrammeIsFmc
        //{
        //    get;
        //    set;
        //}
        //public virtual bool ProgrammeIsFanz
        //{
        //    get;
        //    set;
        //}

        [JsonIgnore]
        public virtual IList<Programme> Programmes { get; set; }
    }
}
