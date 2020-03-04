using System;
using DealEngine.Domain.Entities;


//package product insurer
namespace DealEngine.Infrastructure.Payment.EGlobalAPI.BaseClasses
{
    public class EGlobalInsurerConfig
    {
        public Guid EGlobalInsurerConfig_PolicyRiskID { get; set; }
        public Guid EGlobalInsurerConfig_ID { get; set; }
        public decimal EGlobalInsurerConfig_InsurerProportion { get; set; }
        public string EGlobalInsurerConfig_Code { get; set; }
        public string EGlobalInsurerConfig_Branch { get; set; }
        public int EGlobalInsurerConfig_tLeadOrder { get; set; }
        public EGlobalPolicyRiskConfig EGlobalInsurerConfig_PolicyRisk { get; set; }

        public EGlobalInsurerConfig(PackageProductInsurer packageProductInsurer)
        {
            Load(packageProductInsurer);
        }

        void Load(PackageProductInsurer packageProductInsurer)
        {
            EGlobalInsurerConfig_PolicyRiskID = packageProductInsurer.PPInsurerPackageProduct.Id;
            EGlobalInsurerConfig_ID = packageProductInsurer.Id;
            EGlobalInsurerConfig_InsurerProportion = packageProductInsurer.PPInsurerProportionShare;
            EGlobalInsurerConfig_Code = packageProductInsurer.PPInsurerCode;
            EGlobalInsurerConfig_Branch = packageProductInsurer.PPInsurerBranch;
            EGlobalInsurerConfig_tLeadOrder = packageProductInsurer.PPInsurerLead;
        }
    }
}

