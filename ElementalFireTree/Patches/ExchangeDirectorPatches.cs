using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Console = SRML.Console.Console;
using MonomiPark.SlimeRancher.Regions;

namespace ElementalFireTree.Patches
{
    [HarmonyPatch(typeof(ExchangeRewardItemEntryUI), "SetEntry")]
    class DebugPatchesPre10
    {
        public static bool Prefix(ExchangeRewardItemEntryUI __instance, ExchangeDirector.ItemEntry entry)
        {
            if (entry == null)
            {
                __instance.gameObject.SetActive(false);
                return false;
            }
            __instance.gameObject.SetActive(true);

            if (entry.specReward != ExchangeDirector.NonIdentReward.NONE)
            {
                __instance.icon.sprite = __instance.exchangeDir.GetSpecRewardIcon(entry.specReward);
                __instance.amountText.text = __instance.GetCountDisplayForReward(entry.specReward);
                return false;
            }

            __instance.icon.sprite = __instance.lookupDir.GetIcon(entry.id);
            __instance.amountText.text = entry.count.ToString();
            return false;
        }
    }

    [HarmonyPatch(typeof(ExchangeDirector), "GetSpecRewardIcon")]
    class DebugPatches10
    {
        public static bool Prefix(ExchangeDirector __instance, ExchangeDirector.NonIdentReward specReward, ref Sprite __result)
        {
            ("specReward is null check: " + specReward != null).Log();
            ("__result is null check: " + __result != null).Log();

            if (__instance.nonIdentRewardDict.ContainsKey(specReward))
            {
                __result = __instance.nonIdentRewardDict[specReward];
            }
            else
            {
                __result = Main.assetBundle.LoadAsset<Sprite>("electricMeter");
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(ExchangeDirector), "Update")]
    class DebugPatches4Dot1
    {
        public static bool Prefix(ExchangeDirector __instance)
        {
            if (!__instance.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.VIKTOR_RECUR) && (__instance.worldModel.currOffers.ContainsKey(ExchangeDirector.OfferType.VIKTOR) || __instance.progressDir.GetProgress(ProgressDirector.ProgressType.VIKTOR_REWARDS) >= 4f))
            {
                __instance.worldModel.currOffers[ExchangeDirector.OfferType.VIKTOR_RECUR] = __instance.CreateViktorRecurOffer();
                __instance.OfferDidChange();
                return false;
            }

            return true;
        }
    }


    [HarmonyPatch(typeof(ExchangeDirector), "CreateProgressOffer")]
    class DebugPatches5
    {
        public static bool Prefix(ExchangeDirector __instance, ExchangeDirector.OfferType offerType, ProgressDirector.ProgressType progressType, ExchangeDirector.RewardLevel[] rewardLevels, ref ExchangeDirector.Offer __result)
        {
            int num = __instance.progressDir.GetProgress(progressType) + 1;

            foreach (ExchangeDirector.RewardLevel reward in rewardLevels)
            {
                reward.reward.ToString().Log();
            }

            ExchangeDirector.RewardLevel rewardLevel = rewardLevels[num - 1]; //If the progress is a vanilla one (from  0 to 2, so 3 quests in total) then dont change the code snippet. Else, make it "rewardLevels[num - 4]" for the modded quests (if you're doing your 4th quest, then num will be 4, so 4 - 4 and you get 0, that means the first modded quest)
            string offerId = string.Concat(new object[]
            {
                "m.offer.",
                progressType.ToString().ToLowerInvariant(),
                "_level",
                num
            });
            List<ExchangeDirector.RequestedItemEntry> list = new List<ExchangeDirector.RequestedItemEntry>();
            list.Add(new ExchangeDirector.RequestedItemEntry(rewardLevel.requestedItem, rewardLevel.count, 0));
            List<ExchangeDirector.ItemEntry> list2 = new List<ExchangeDirector.ItemEntry>();
            list2.Add(new ExchangeDirector.ItemEntry(rewardLevel.reward));

            (offerId + ", " + rewardLevel.requestedItem + ", " + rewardLevel.count + ", " + rewardLevel.reward).Log();

            __result = new ExchangeDirector.Offer(offerId, offerType.ToString().ToLowerInvariant(), double.PositiveInfinity, double.NegativeInfinity, list, list2);
            return false;
        }
    }

    /*[HarmonyPatch(typeof(ExchangeAcceptor), "Awake")]
    class DebugPatches6
    {
        public static void Postfix(ExchangeAcceptor __instance)
        {

            __instance.awarders.PrintContent();

            foreach(var award in __instance.awarders)
            {
                Console.Log("");
            }
        }
    }*/

    [HarmonyPatch(typeof(UITemplates), "CreatePurchaseUI")]
    class DebugPatches7
    {
        public static void Postfix(UITemplates __instance, Sprite titleIcon, string titleKey, PurchaseUI.Purchasable[] purchasables, bool hideNubuckCost, PurchaseUI.OnClose onClose, bool unavailInMainList = false)
        {
            "UITemplates.CreatePurchaseUI Patch".Log();
            ("titleKey: " + titleKey).Log();
            ("Parent to: " + __instance.transform.parent).Log();
        }
    }
}
