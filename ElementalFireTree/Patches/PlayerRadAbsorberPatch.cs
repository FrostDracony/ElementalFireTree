using System;
using System.Reflection;
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
    [HarmonyPatch(typeof(PlayerRadAbsorber), "Absorb")]
    class PlayerRadAbsoberPatch
    {
        public static bool Prefix(PlayerRadAbsorber __instance, GameObject source, float rads)
        {
            int healthLoss = __instance.playerState.AddRads(rads);
            healthLoss = source.transform.parent.gameObject.GetComponent<Identifiable>().id == Ids.ELEMENTAL_FIRE_SLIME ? healthLoss*5 : healthLoss;
            if (healthLoss > 0 && __instance.damageable.Damage(healthLoss, null))
                DeathHandler.Kill(__instance.gameObject, DeathHandler.Source.SLIME_RAD, source, "PlayerRadAbsorber.Absorb");
            __instance.absorbingThisFrame = true;

            return false;
        }

    }
}