using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class MaterialDamage : EntityBase, IAggregateRoot
    {
        protected MaterialDamage() : base(null) { }

        public MaterialDamage(User createdBy)
            : base(createdBy)
        {

        }

        public virtual Location Location
        {
            get;
            set;
        }

        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }

        public virtual int NonHirePlant
        {
            get;
            set;
        }

        public virtual int Stock
        {
            get;
            set;
        }

        public virtual int Miscellaneous
        {
            get;
            set;
        }

        public virtual int HirePlant
        {
            get;
            set;
        }

        public virtual int Marquees
        {
            get;
            set;
        }

        public virtual int Transit
        {
            get;
            set;
        }

        public virtual int MoneyA
        {
            get;
            set;
        }

        public virtual int MoneyB
        {
            get;
            set;
        }

        public virtual int AINZ
        {
            get;
            set;
        }

        public virtual int CustomersGoods
        {
            get;
            set;
        }

        public virtual int MDCapitalAdditions
        {
            get;
            set;
        }

        public virtual int ToolsAndStock
        {
            get;
            set;
        }

        public virtual int MDCapitalAdditionsPre
        {
            get;
            set;
        }

        public virtual int MDConstructionLimit
        {
            get;
            set;
        }

        public virtual int MDConstructionLimitPre
        {
            get;
            set;
        }

        public virtual bool Removed
        {
            get;
            set;
        }

    }
}
