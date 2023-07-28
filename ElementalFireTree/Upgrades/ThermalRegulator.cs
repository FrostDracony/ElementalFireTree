using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRML;
using SRML.SR;
using SRML.Utils;
using UnityEngine;
using Creators;

namespace ElementalFireTree.Upgrades
{
    public class ThermalRegulator : PlotUpgrader
    {
        /*public class Upgrade : PlotUpgrader
        {*/
            public override void Apply(LandPlot.Upgrade upgrade)
            {
                if (upgrade == Ids.THERMAL_REGULATOR)
                {

                    GameObject ThermalRegulator = Instantiate(Main.assetBundle.LoadAsset<GameObject>("ThermalRegulator"), gameObject.transform);
                    ThermalRegulator.SetActive(true);

                    ThermalRegulator.FindChild("ThermalRegulator Region").AddComponent<ThermalRegulatorRegion>();

                    //1st bond: x:918.5, 19.7, -217.4
                }
            }

        //}

        public static LandPlotUpgradeRegistry.UpgradeShopEntry CreateThermalRegulatorEntry()
        {
            return new LandPlotUpgradeRegistry.UpgradeShopEntry
            {
                icon = Main.assetBundle.LoadAsset<Sprite>("LiquidFire"),
                upgrade = Ids.THERMAL_REGULATOR,
                mainImg = Main.assetBundle.LoadAsset<Sprite>("LiquidFire"),
                cost = 25000,
                landplotPediaId = PediaDirector.Id.CORRAL,
                isUnlocked = plot =>
                {
                    if (SceneContext.Instance.ExchangeDirector.progressDir.GetProgress(SceneContext.Instance.ExchangeDirector.GetProgressEntry(ExchangeDirector.OfferType.VIKTOR).progressType) >= 5)
                    {
                        return true;
                    }

                    return false;//* Remember to add back the lock
                },
                LandPlotName = "corral"
            };
        }

    }



}
