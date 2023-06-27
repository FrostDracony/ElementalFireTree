using System;
using System.Collections.Generic;
using System.Linq;
using Console = SRML.Console.Console;
using UnityEngine;
using MonomiPark.SlimeRancher.Regions;
using ElementalFireTree;
namespace Creators
{
    static class ExchangeCreator
    {
        //Entries
        public static List<RancherChatMetadata.Entry> finalEntriesIntro = new List<RancherChatMetadata.Entry>();
        public static List<RancherChatMetadata.Entry> finalEntriesRepeat = new List<RancherChatMetadata.Entry>();
        public static List<RancherChatMetadata.Entry> entriesAtEndingIntro = new List<RancherChatMetadata.Entry>();
        public static List<RancherChatMetadata.Entry> entriesAtEndingRepeat = new List<RancherChatMetadata.Entry>();

        //ExchangeDirector
        public static ExchangeDirector exchangeDirector = SRSingleton<SceneContext>.Instance.ExchangeDirector;
        //DictionaryForCustomEntries
        public static Dictionary<ExchangeDirector.OfferType, List<ExchangeDirector.RewardLevel>> moddedEntries = new Dictionary<ExchangeDirector.OfferType, List<ExchangeDirector.RewardLevel>>();
        public static Dictionary<ExchangeDirector.NonIdentReward, Sprite> moddedSpecRewardIcons = new Dictionary<ExchangeDirector.NonIdentReward, Sprite>();

        public static ExchangeDirector.RewardLevel CreateRewardLevel(int cnt, RancherChatMetadata intro, RancherChatMetadata repeat, Identifiable.Id item, ExchangeDirector.NonIdentReward rew)
        {
            return new ExchangeDirector.RewardLevel()
            {
                count = cnt,
                rancherChatIntro = intro,
                rancherChatRepeat = repeat,
                requestedItem = item,
                reward = rew
            };
        }

        public static bool MaybeAddRewardLevels(ExchangeDirector.ProgressOfferEntry progressOfferEntry)
        {
            if (moddedEntries.ContainsKey(progressOfferEntry.specialOfferType))
            {
                return true;
            }
            return false;
        }

        public static RancherChatMetadata.Entry[] CreateRancherChatConversation(this ExchangeDirector exchangeDirector, string rancherId, string[] messages, Sprite[] sprites)
        {
            List<RancherChatMetadata.Entry> entryToReturn = new List<RancherChatMetadata.Entry>();
            int i = 0;
            Array.ForEach(messages, text =>
            {
                entryToReturn.Add(new RancherChatMetadata.Entry
                {
                    rancherName = (RancherChatMetadata.Entry.RancherName)Enum.Parse(typeof(RancherChatMetadata.Entry.RancherName), rancherId.ToUpperInvariant()),
                    rancherImage = sprites[i],
                    messageBackground = exchangeDirector.GetRancher(rancherId).chatBackground,
                    messageText = text
                });

                i++;
            });

            return entryToReturn.ToArray();
        }


        public static void CreateRancherQuest(this ExchangeDirector exchangeDirector, ExchangeDirector.OfferType rancherOffer, int count, Identifiable.Id requestedItem,
            ExchangeDirector.NonIdentReward reward, RancherChatMetadata rancherChatMetadataRepeat, RancherChatMetadata rancherChatMetadataIntro, RancherChatMetadata.Entry[] messagesAtEndRepeat,
            RancherChatMetadata.Entry[] messagesAtEndIntro, Sprite spriteToRegister, bool finalQuest = false)
        {
            ("REGISTERING RANCHER QUEST").Log();
            foreach (ExchangeDirector.ProgressOfferEntry progressOfferEntry in exchangeDirector.progressOffers)
            {
                ("Loop initiated with: " + progressOfferEntry.specialOfferType).Log();
                if (progressOfferEntry.specialOfferType == rancherOffer)
                {
                    "Printing content of rancherChatEndIntro".Log();
                    ExchangeDirector.ProgressOfferEntry currentProgressOfferEntry = exchangeDirector.GetProgressEntry(rancherOffer);
                    foreach (RancherChatMetadata.Entry entry in currentProgressOfferEntry.rancherChatEndIntro.entries)
                    {
                        ("Checking Entries for: " + rancherOffer + " in rancherChatEndIntro").Log();
                        ("messageBackground: " + entry.messageBackground).Log();
                        ("messagePrefab: " + entry.messagePrefab + ", containing: " + entry.messagePrefab.GetComponents<Component>()).Log();
                        ("messageText: " + entry.messageText).Log();
                        ("rancherImage: " + entry.rancherImage).Log();
                        ("rancherName: " + entry.rancherName).Log();
                    }
                    foreach (RancherChatMetadata.Entry entry in currentProgressOfferEntry.rancherChatEndRepeat.entries)
                    {
                        ("Checking Entries for: " + rancherOffer + " in rancherChatEndRepeat").Log();
                        ("messageBackground: " + entry.messageBackground).Log();
                        ("messagePrefab: " + entry.messagePrefab + ", containing: " + entry.messagePrefab.GetComponents<Component>()).Log();
                        ("messageText: " + entry.messageText).Log();
                        ("rancherImage: " + entry.rancherImage).Log();
                        ("rancherName: " + entry.rancherName).Log();
                    }
                    //List<ExchangeDirector.RewardLevel> allModdedRewards = new List<ExchangeDirector.RewardLevel>();

                    /*if (moddedEntries.ContainsKey(rancherOffer))
                    {
                        allModdedRewards = moddedEntries.Get(rancherOffer);
                    }*/
                    /*else
                    {
                        moddedEntries.Add(rancherOffer, allModdedRewards);
                    }*/

                    ExchangeDirector.RewardLevel newRewardLevel = new ExchangeDirector.RewardLevel();
                    newRewardLevel.requestedItem = requestedItem;
                    newRewardLevel.reward = reward;
                    newRewardLevel.count = count;
                    newRewardLevel.rancherChatRepeat = rancherChatMetadataRepeat;
                    newRewardLevel.rancherChatIntro = rancherChatMetadataIntro;

                    if (finalQuest)
                    {
                        ("finalQuest is true").Log();

                        Array.ForEach(messagesAtEndIntro, x => newRewardLevel.rancherChatIntro.entries.Prepend(x));
                        Array.ForEach(messagesAtEndRepeat, x => newRewardLevel.rancherChatRepeat.entries.Prepend(x));

                    }
                    else
                    {
                        ("finalQuest is false").Log();
                        ExchangeDirector.OfferType offerType = (ExchangeDirector.OfferType)Enum.Parse(typeof(ExchangeDirector.OfferType), rancherOffer.ToString() + "_RECUR");
                        ("offerType: " + offerType).Log();

                        Array.ForEach(messagesAtEndIntro, x => newRewardLevel.rancherChatIntro.entries.Prepend(x));
                        Array.ForEach(messagesAtEndRepeat, x => newRewardLevel.rancherChatRepeat.entries.Prepend(x));

                        /*//These will be last, hence why we're adding the other stuff at the beginning
                        Array.ForEach(messagesAtEndIntro, x => currentProgressOfferEntry.rancherChatEndIntro.entries.Prepend(x));
                        Array.ForEach(messagesAtEndRepeat, x => currentProgressOfferEntry.rancherChatEndRepeat.entries.Prepend(x));

                        *//*//it's a loop so we gotta clear this
                        finalEntriesIntro.Clear();
                        finalEntriesRepeat.Clear();

                        //These will be last, hence why we're appending it to the rest
                        Array.ForEach(messagesAtEndIntro, x => finalEntriesIntro.Append(x));
                        Array.ForEach(messagesAtEndRepeat, x => finalEntriesRepeat.Append(x));*//*

                        if (entriesAtEndingIntro.Count > 0)
                        {
                            entriesAtEndingIntro.ForEach(x => newRewardLevel.rancherChatIntro.entries.Prepend(x));
                            entriesAtEndingIntro.Clear();
                        }

                        if (entriesAtEndingRepeat.Count > 0)
                        {
                            entriesAtEndingRepeat.ForEach(x => newRewardLevel.rancherChatRepeat.entries.Prepend(x));
                            entriesAtEndingRepeat.Clear();
                        }*/

                    }

                    /*if (allModdedRewards.Contains(newRewardLevel))
                    {
                        //Well, that already exists, so lets not create it again
                        return;
                    }
                    allModdedRewards.Add(newRewardLevel);*/

                    moddedSpecRewardIcons.Add(reward, spriteToRegister);
                    exchangeDirector.nonIdentRewardDict.Add(reward, spriteToRegister);

                    Array.Resize(ref currentProgressOfferEntry.rewardLevels, currentProgressOfferEntry.rewardLevels.Length + 1);
                    currentProgressOfferEntry.rewardLevels.Append(newRewardLevel);
                    moddedEntries.Add(rancherOffer, currentProgressOfferEntry.rewardLevels.ToList());
                }
            }
        }

        public static ExchangeDirector.Offer CreateOffer(string rancherId, int num)
        {
            return exchangeDirector.offerGenerators[rancherId].Generate(exchangeDirector,
                exchangeDirector.CreateWhiteList(),
                exchangeDirector.timeDir.GetNextHourAtLeastHalfDay(12f),
                exchangeDirector.timeDir.HoursFromNow(2f), num, false,
                SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().exchangeRewardsGoldPlorts);
        }
    }
}