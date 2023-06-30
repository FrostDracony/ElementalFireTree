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
    [HarmonyPatch(typeof(SlimeIgniteReact), "Ignite")]
    class SlimeIgniteReactPatch
    {
        public static bool Prefix(SlimeIgniteReact __instance, GameObject igniter)
        {
            if(igniter.name == "liquidfire(Clone)")
            {
                if (__instance.selfIsIgniter || Time.time < __instance.throttle)
                    return false;
                __instance.throttle = Time.time + 0.2f;
                if (__instance.igniteFX != null)
                    SRBehaviour.SpawnAndPlayFX(__instance.igniteFX, __instance.transform.position, __instance.transform.rotation);
                __instance.faceAnim.SetTrigger("triggerAlarm");
                __instance.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, 0f);
                return false;
            }
            return true;
        }

    }
}


/*using System;
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
    [HarmonyPatch(typeof(CalmedByWaterSpray), "AddLiquid")]
    class CalmedByWaterSprayPatch
    {
        public static bool Prefix(CalmedByWaterSpray __instance, Identifiable.Id liquidId)
        {
            ("liquidId: " + liquidId).Log();
            ("is liquid water: " +)
            if (!Identifiable.IsWater(liquidId)) //"liquidfire(Clone)"
            {
                return false;
            }
            if (liquidId == Ids.FIRE_LIQUID)
            {
                if (this.selfIsIgniter || Time.time < this.throttle)
                    return;
                this.throttle = Time.time + 0.2f;
                if ((Object)this.igniteFX != (Object)null)
                    SRBehaviour.SpawnAndPlayFX(this.igniteFX, this.transform.position, this.transform.rotation);
                this.faceAnim.SetTrigger("triggerAlarm");
                this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, 0.5f);
                this.body.AddForce((this.transform.position - igniter.transform.position).normalized * 1f + Vector3.up * 3f, ForceMode.Impulse);
                return false;
            }
            return true;
        }

    }
}
*/