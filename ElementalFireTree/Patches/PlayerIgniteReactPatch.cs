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
    [HarmonyPatch(typeof(PlayerDamageable), "Damage")]
    class PlayerIgniteReactPatch
    {
        public static bool Prefix(ref bool __result, PlayerDamageable __instance, int healthLoss, GameObject source)
        {
            /*("What is damaging the player? Its name is: " + source.name).Log();
            ("Damage amount is: " + healthLoss).Log();
            */
            source = source == null ? __instance.gameObject : source;
            if(source.name == "FireBallExploding(Clone)" || source.name == "FireBall(Clone)")
            {
                healthLoss = Mathf.RoundToInt(Mathf.Clamp(healthLoss * SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().playerDamageMultiplier, 20f, 120f));
            }
            else
            {
                healthLoss = Mathf.RoundToInt(healthLoss * SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().playerDamageMultiplier);
            }
            if (!__instance.GetPrivateField<PlayerState>("playerState").CanBeDamaged())
                return false;
            SRSingleton<Overlay>.Instance.PlayDamage();
            
            MethodInfo dynMethod = __instance.GetType().GetMethod("GetDamageCue", BindingFlags.NonPublic | BindingFlags.Instance);
            
            __instance.GetPrivateField<SECTR_AudioSource>("playerAudio").Cue = (SECTR_AudioCue)dynMethod.Invoke(__instance, new object[] { source }); ;
            __instance.GetPrivateField<SECTR_AudioSource>("playerAudio").Play();
            __instance.GetPrivateField<ScreenShaker>("screenShaker").ShakeDamage(0.2f * healthLoss);

            __result = __instance.GetPrivateField<PlayerState>("playerState").Damage(healthLoss, source);

            return false;
        }

    }
}
