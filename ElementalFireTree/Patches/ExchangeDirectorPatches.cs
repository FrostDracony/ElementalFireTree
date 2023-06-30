﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Console = SRML.Console.Console;
using MonomiPark.SlimeRancher.Regions;
using SRML;
using Creators;

namespace ElementalFireTree.Patches
{


    /*[HarmonyPatch(typeof(ExchangeDirector), "MaybeStartNext")]
    class ExchangeDirectorMaybeStartNextPatch
    {
        public static bool Prefix(ExchangeDirector __instance, ExchangeDirector.OfferType offerType)
        {
            if (__instance.worldModel.currOffers.ContainsKey(offerType))
                return false;
            ExchangeDirector.ProgressOfferEntry progressEntry = __instance.GetProgressEntry(offerType);
            if (progressEntry == null || __instance.worldModel.currOffers.ContainsKey(progressEntry.specialOfferType) || __instance.progressDir.GetProgress(progressEntry.progressType) >= progressEntry.rewardLevels.Length)
                return false;

            //The actual new part
            if(__instance.progressDir.GetProgress(progressEntry.progressType) == 3)

            __instance.worldModel.currOffers[progressEntry.specialOfferType] = __instance.CreateProgressOffer(progressEntry.specialOfferType, progressEntry.progressType, progressEntry.rewardLevels);
            __instance.OfferDidChange();
            return __instance.CreateRancherChatUI(offerType, true);
            
            return false;
        }
    }*/


    /*[HarmonyPatch(typeof(ExchangeRewardItemEntryUI), "SetEntry")]
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
    }*/

    [HarmonyPatch(typeof(ExchangeDirector), "GetSpecRewardIcon")]
    class DebugPatchesPre11
    {
        public static bool Prefix(ExchangeDirector __instance, ExchangeDirector.NonIdentReward specReward, ref Sprite __result)
        {
            if (specReward == Ids.FIRE_LIQUID_VAC_REW)
            {
                __result = Main.assetBundle.LoadAsset<Sprite>("FireUpgrade");
                return false;
            }
            if (specReward == Ids.THERMAL_REGULATOR_REW)
            {
                __result = Main.assetBundle.LoadAsset<Sprite>("LiquidFire");
                return false;
            }
            return true;
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

    [HarmonyPatch(typeof(ExchangeDirector), "MaybeStartNext")]
    class DebugPatches5_2
    {
        public static bool Prefix(ExchangeDirector __instance, ExchangeDirector.OfferType offerType)
        {
            if (__instance.worldModel.currOffers.ContainsKey(offerType))
                return false;
            ExchangeDirector.ProgressOfferEntry progressEntry = __instance.GetProgressEntry(offerType);
            if (progressEntry == null || __instance.worldModel.currOffers.ContainsKey(progressEntry.specialOfferType) || __instance.progressDir.GetProgress(progressEntry.progressType) >= progressEntry.rewardLevels.Length)
                return false;

            ("CreateProgressOffer is at: " + __instance.progressDir.GetProgress(ProgressDirector.ProgressType.VIKTOR_REWARDS) + " with a length of: " + __instance.GetProgressEntry(offerType).rewardLevels.Length + " Kek").Log();

            if (offerType == ExchangeDirector.OfferType.VIKTOR && __instance.progressDir.GetProgress(ProgressDirector.ProgressType.VIKTOR_REWARDS) == 4  && !__instance.pediaDir.IsUnlocked(Ids.ELEMENTAL_FIRE_SLIME_ENTRY))
            {
                if(ExchangeCreator.isViktorQuestChanged)
                {
                    "No need to rechange the quest again, just play the UI".Log();
                    __instance.CreateRancherChatUI(offerType, true);
                    return false;
                }

                RancherChatMetadata.Entry[] introConversation1 = __instance.CreateRancherChatConversation("viktor",
                    new string[]
                    {
                        "Huh, Beatrix?",
                        "Oh, more questions about what happened with the strange diamond?",
                        "Well, as already said, I was cleaning a dirty sample with some liquid fire.",
                        "The diamond became a bit red, but then white again and the surface so clean I could not believe my own eyes.",
                        "Oh, now that you say it, while it was red, it was indeed behaving a bit strange...",
                        "I tried applying even more droplets of liquid fire, but they were suspended in the air.",
                        "And then they fell back on the diamond once it turned back red. I do not think it was something particular tho, because I was working with in an antigravitional field zone... for some long and complicated reasons.",
                        "If you wanna experiment around with it, feel free to do so Beatrix, but I first will have to properly store those fire plorts of yours. If you have found out some results from your own experiments, feel free to contact me tho.",
                        "I will now go back to work, it was nice chatting with you. Just, watch out to not hurt yourself while doing so, you should know best, after being in the glass desert, that fire is not something to joke with.",
                    },
                    new Sprite[]
                    {
                        SRObjects.Get<Sprite>("viktor_confused"),
                        SRObjects.Get<Sprite>("viktor_thinking"),
                        SRObjects.Get<Sprite>("viktor_explain"),
                        SRObjects.Get<Sprite>("viktor_work"),
                        SRObjects.Get<Sprite>("viktor_confused"),
                        SRObjects.Get<Sprite>("viktor_thinking"),
                        SRObjects.Get<Sprite>("viktor_default2"),
                        SRObjects.Get<Sprite>("viktor_greeting"),
                        SRObjects.Get<Sprite>("viktor_default2")
                    }
                );

                RancherChatMetadata.Entry[] repeatConversation1 = __instance.CreateRancherChatConversation("viktor",
                    new string[]
                    {
                    "OH, HELLO BEATRIX",
                    "AS YOU CAN SEE, I AM IN THE MIDDLE OF A VERY IMPORTANT EXPERIMENT RIGHT NOW",
                    "COULD YOU PLEASE COME BACK LATER ON?. SEE YOU SOON!",
                    },
                    new Sprite[]
                    {
                    SRObjects.Get<Sprite>("viktor_bubble_surprise"),
                    SRObjects.Get<Sprite>("viktor_bubble_work"),
                    SRObjects.Get<Sprite>("viktor_bubble_point")
                    }
                );

                "Still working?".Log();

                RancherChatMetadata rancherChatMetadataIntro1 = ScriptableObject.CreateInstance<RancherChatMetadata>();
                rancherChatMetadataIntro1.entries = introConversation1;

                "Damn, still working".Log();

                RancherChatMetadata rancherChatMetadataRepeat1 = ScriptableObject.CreateInstance<RancherChatMetadata>();
                rancherChatMetadataRepeat1.entries = repeatConversation1;

                __instance.GetProgressEntry(offerType).rewardLevels[4] = ExchangeCreator.CreateRewardLevel(
                                        25,
                                        rancherChatMetadataIntro1,
                                        rancherChatMetadataRepeat1,
                                        Ids.ELEMENTAL_FIRE_PLORT,
                                        Ids.THERMAL_REGULATOR_REW
                                      );
                "Ok, why are you playing twice?".Log();
                __instance.CreateRancherChatUI(offerType, true);
                ExchangeCreator.isViktorQuestChanged = true;
                return false;
            }

            return true;
        }
    }

    /*[HarmonyPatch(typeof(ExchangeDirector), "CreateProgressOffer")]
    class DebugPatches5
    {
        public static bool Prefix(ExchangeDirector __instance, ExchangeDirector.OfferType offerType, ProgressDirector.ProgressType progressType, ExchangeDirector.RewardLevel[] rewardLevels, ref ExchangeDirector.Offer __result)
        {
            int num = __instance.progressDir.GetProgress(progressType) >= 4 ? __instance.progressDir.GetProgress(progressType) : __instance.progressDir.GetProgress(progressType) + 1;

            ("CreateProgressOffer is at: " + __instance.progressDir.GetProgress(progressType) + " with a length of: " + __instance.GetProgressEntry(offerType).rewardLevels.Length + " Kek").Log();
            ("num: " + num).Log();
            foreach (ExchangeDirector.RewardLevel reward in rewardLevels)
            {
                if (reward == null || reward.reward == ExchangeDirector.NonIdentReward.NONE)
                    continue;
                reward.reward.ToString().Log();
            }

            ExchangeDirector.RewardLevel rewardLevel = rewardLevels[num - 1]; // (rewardLevels[num - 1];)  If the progress is a vanilla one (from  0 to 2, so 3 quests in total) then dont change the code snippet. Else, make it "rewardLevels[num - 4]" for the modded quests (if you're doing your 4th quest, then num will be 4, so 4 - 4 and you get 0, that means the first modded quest)

            "Huh.... Kek".Log();

            string offerId = string.Concat(new object[]
            {
                "m.offer.",
                progressType.ToString().ToLowerInvariant(),
                "_level",
                num
            });
            List<ExchangeDirector.RequestedItemEntry> list = new List<ExchangeDirector.RequestedItemEntry>();

            "Kek how is it still working".Log();

            list.Add(new ExchangeDirector.RequestedItemEntry(rewardLevel.requestedItem, rewardLevel.count, 0));

            "Still... working".Log();

            List<ExchangeDirector.ItemEntry> list2 = new List<ExchangeDirector.ItemEntry>();
            list2.Add(new ExchangeDirector.ItemEntry(rewardLevel.reward));

            "Yup".Log();

            ("Sooo... " + offerId + ", " + offerType + ", " + rewardLevel.requestedItem + ", " + rewardLevel.count + ", " + rewardLevel.reward).Log();

            __result = new ExchangeDirector.Offer(offerId, offerType.ToString().ToLowerInvariant(), double.PositiveInfinity, double.NegativeInfinity, list, list2);

            "Yeah i really, truly does".Log();

            return false;
        }
    }*/

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
