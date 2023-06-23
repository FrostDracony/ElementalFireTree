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
    [HarmonyPatch(typeof(LiquidSource), "Register")]
    class TestPatch
    {
        public static bool Prefix(LiquidSource __instance, GameModel game)
        {
            
            ("GameModel's name: " + game.name).Log();
            ("LiquidSource's name: " + __instance.name).Log();
            ("LiquidSource's active state: " + __instance.enabled).Log();
            ("LiquidSource's transform's name: " + __instance.transform.name).Log();
            "Well, lets see if there exists an liquidSourceContainer object:".Log();
            ("liquidSourceContainer is: " + GameObject.Find("liquidSourceContainer")).Log();
            "LiquidSource's parent's name: ".Log();
            if (__instance.name == "liquidSourceContainer")
                return false;
            //__instance.Register(SceneContext.Instance.GameModel);
            "CMON".Log();
            return true;
        }
    }
}

/*[21:51:25] [Unity] [ERRO] NullReferenceException: Object reference not set to an instance of an object
(wrapper dynamic-method) IdHandler.IdHandler.get_id_Patch1(IdHandler)
IdHandler`1[M].GetId()(at < 00ea3b7d451b4f908b7d688af16ee773 >:0)
MonomiPark.SlimeRancher.DataModel.GameModel + IdContainer`1[M].Register(MonomiPark.SlimeRancher.DataModel.IdHandlerModel + Participant participant)(at < 00ea3b7d451b4f908b7d688af16ee773 >:0)


<----------------------------------------------------------------------------------------------------------------->
LiquidSource.Register(MonomiPark.SlimeRancher.DataModel.GameModel game)(at < 00ea3b7d451b4f908b7d688af16ee773 >:0)
<----------------------------------------------------------------------------------------------------------------->


IdHandler`1[M].Awake()(at < 00ea3b7d451b4f908b7d688af16ee773 >:0)
UnityEngine.GameObject:AddComponent()
ElementalFireTree.Patches.FirestormActivatorPatch:Prefix(FirestormActivator)
FirestormActivator: FirestormActivator.MaybeTriggerNearbyColumns_Patch1(FirestormActivator)
FirestormActivator: Update()*/