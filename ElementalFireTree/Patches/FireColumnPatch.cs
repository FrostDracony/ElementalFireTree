using System;
using Object = System.Object;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRML;
using HarmonyLib;
using UnityEngine;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;

namespace ElementalFireTree.Patches
{
    [HarmonyPatch(typeof(FireColumn), "DeactivateFire")]
    class FireColumnPatch
    {
        public static void Postfix(FireColumn __instance)
        {

            __instance.gameObject.PrintAllChildren();
            for(int i = Main.specialColumns.Count - 1; i >= 0; i--)
            {
                FireColumn column = Main.specialColumns[i];

                Transform liquidSourceContainer = column.transform.Find("liquidSourceContainer");
                if (liquidSourceContainer != null)
                    liquidSourceContainer.gameObject.Destroy();
                Main.specialColumns.Remove(column);
            }

            /*This makes an error so we're not using it'
            foreach (FireColumn column in Main.specialColumns)
            {
                ("lets see if column is a special column: " + (column == __instance)).Log();

                Transform liquidSourceContainer = column.transform.Find("liquidSourceContainer");
                if (liquidSourceContainer != null)
                    liquidSourceContainer.gameObject.Destroy();
                Main.specialColumns.Remove(column);
            }*/
            
        }
    }
}
