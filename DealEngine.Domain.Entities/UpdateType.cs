//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DealEngine.Domain.Entities.Abstracts;
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
        //protected UpdateType() : this(null) { }

        protected UpdateType() : base(null) { }

        public UpdateType(User createdBy, string typeName, string typeValue, bool typeIsTc, bool typeIsBroker, bool typeIsInsurer, bool typeIsClient)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException(nameof(typeName));

            if (string.IsNullOrWhiteSpace(typeValue))
                throw new ArgumentNullException(nameof(typeValue));

            //if (bool.(typeIsBroker))
            //    throw new ArgumentNullException(nameof(typeIsBroker));

            //if (string.IsNullOrWhiteSpace(typeIsClient))
            //    throw new ArgumentNullException(nameof(typeIsClient));

            //if (string.IsNullOrWhiteSpace(typeIsInsurer))
            //    throw new ArgumentNullException(nameof(typeIsInsurer));

            //if (string.IsNullOrWhiteSpace(typeIsTc))
            //    throw new ArgumentNullException(nameof(typeIsTc));

            TypeName = typeName;
            TypeValue = typeValue;
            TypeIsTc = typeIsTc;
            TypeIsInsurer = typeIsInsurer;
            TypeIsClient = typeIsClient;
            TypeIsBroker = typeIsBroker;
        }

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
        //ruleroletc, rulerolebroker, ruleroleinsurer
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
