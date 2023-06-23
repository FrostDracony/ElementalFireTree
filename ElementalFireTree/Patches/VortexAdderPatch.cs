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
    [HarmonyPatch(typeof(Ammo), "MaybeAddToSlot")]
    class VortexAdderPatch
    {
        public static bool Prefix(Ammo __instance, Identifiable.Id id, Identifiable identifiable)
        {
            /*"MaybeAddToSlotFunction".Log();

            
            (id + ": is what I'm tryin to add").Log();

            ("Lets see if I have the upgrade: " + SRSingleton<SceneContext>.Instance.PlayerState.HasUpgrade(Ids.FIRE_LIQUID_VAC)).Log();*/
            if (id == Ids.FIRE_LIQUID && !SRSingleton<SceneContext>.Instance.PlayerState.HasUpgrade(Ids.FIRE_LIQUID_VAC))
            {
                //If the player's trying to get the liquid fire without having the upgrade, they just won't be able to do so
                return false;
            }
            
            return true;
        }
    }
}
