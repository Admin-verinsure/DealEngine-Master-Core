using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace techcertain2015rebuildcore.Models.ViewModels
{
    public class MaterialDamageViewModel
    {
        public Guid AnswerSheetId { get; set; }

        public Guid MaterialDamageId { get; set; }

        public int NonHirePlant { get; set; }

        public int Stock { get; set; }

        public int Miscellaneous { get; set; }

        public int HirePlant { get; set; }

        public int Marquees { get; set; }

        public int Transit { get; set; }

        public int DualWages { get; set; }

        public int MoneyA { get; set; }

        public int MoneyB { get; set; }

        public int AINZ { get; set; }

        public int CustomersGoods { get; set; }

        public int ToolsAndStock { get; set; }

        public int MDCapitalAdditions { get; set; }

        public int MDCapitalAdditionsPre { get; set; }

        public int MDConstructionLimit { get; set; }

        public int MDConstructionLimitPre { get; set; }

        public Guid MaterialDamageLocation { get; set; }

        public MaterialDamage ToEntity(User creatingUser)
        {
            MaterialDamage materialDamage = new MaterialDamage(creatingUser);
            UpdateEntity(materialDamage);
            return materialDamage;
        }

        public MaterialDamage UpdateEntity(MaterialDamage materialDamage)
        {
            materialDamage.NonHirePlant = NonHirePlant;
            materialDamage.Stock = Stock;
            materialDamage.Miscellaneous = Miscellaneous;
            materialDamage.HirePlant = HirePlant;
            materialDamage.Marquees = Marquees;
            materialDamage.Transit = Transit;
            materialDamage.MoneyA = MoneyA;
            materialDamage.MoneyB = MoneyB;
            materialDamage.AINZ = AINZ;
            materialDamage.CustomersGoods = CustomersGoods;
            materialDamage.ToolsAndStock = ToolsAndStock;
            materialDamage.MDCapitalAdditions = MDCapitalAdditions;
            materialDamage.MDCapitalAdditionsPre = MDCapitalAdditionsPre;
            materialDamage.MDConstructionLimit = MDConstructionLimit;
            materialDamage.MDConstructionLimitPre = MDConstructionLimitPre;

            return materialDamage;
        }

        public static MaterialDamageViewModel FromEntity(MaterialDamage materialDamage)
        {
            MaterialDamageViewModel model = new MaterialDamageViewModel
            {
                MaterialDamageId = materialDamage.Id,
                NonHirePlant = materialDamage.NonHirePlant,
                Stock = materialDamage.Stock,
                Miscellaneous = materialDamage.Miscellaneous,
                HirePlant = materialDamage.HirePlant,
                Marquees = materialDamage.Marquees,
                Transit = materialDamage.Transit,
                MoneyA = materialDamage.MoneyA,
                MoneyB = materialDamage.MoneyB,
                AINZ = materialDamage.AINZ,
                CustomersGoods = materialDamage.CustomersGoods,
                ToolsAndStock = materialDamage.ToolsAndStock,
                MDCapitalAdditions = materialDamage.MDCapitalAdditions,
                MDCapitalAdditionsPre = materialDamage.MDCapitalAdditionsPre,
                MDConstructionLimit = materialDamage.MDConstructionLimit,
                MDConstructionLimitPre = materialDamage.MDConstructionLimitPre,
            };
            if (materialDamage.Location != null)
                model.MaterialDamageLocation = materialDamage.Location.Id;

            return model;
        }
    }

}