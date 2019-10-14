using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Infrastructure.Payment.EGlobalAPI.BaseClasses
{
    public class EGlobalPolicyConfig
    {
        public EGlobalPolicyConfig(){ }
        public virtual string RiskCode { get; set; }
        public virtual string Branch { get; set; }
        public virtual string DescriptionNew { get; set; }
        public virtual string DescriptionChange { get; set; }
        public virtual string DescriptionRenew { get; set; }
        public virtual string DescriptionCancel { get; set; }
        public virtual string DescriptionLapse { get; set; }
        public virtual string StatementNew { get; set; }
        public virtual string StatementChange { get; set; }
        public virtual string StatementRenew { get; set; }
        public virtual string StatementCancel { get; set; }
        public virtual string ContractCode { get; set; }
        public virtual bool HasInvoicePayment { get; set; }
        public virtual bool HasCCPayment { get; set; }
        public virtual bool HasPremiumPayment { get; set; }
        public virtual string FTPFolder { get; set; }                  
    }
}

