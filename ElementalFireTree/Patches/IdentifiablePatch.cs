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
    [HarmonyPatch(typeof(Identifiable), "IsWater")]
    class IdentifiablePatch
    {
        public static bool Prefix(Identifiable __instance, Identifiable.Id id)
        => id != Identifiable.Id.GLITCH_DEBUG_SPRAY_LIQUID
            && id != Ids.FIRE_LIQUID
            && Identifiable.LIQUID_CLASS.Contains(id);
    }
}
