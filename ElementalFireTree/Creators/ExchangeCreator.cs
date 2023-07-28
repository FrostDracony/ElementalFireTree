using System;
using System.Collections.Generic;
using System.Linq;
using Console = SRML.Console.Console;
using UnityEngine;
using MonomiPark.SlimeRancher.Regions;
using ElementalFireTree;
using SRML;
using SRML.SR;

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

        //MyOwnStuff
        public static bool isViktorQuestChanged = false;

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

        public static void SetViktorsQuestToNormal()
        {
            ExchangeDirector.ProgressOfferEntry progressOfferEntry2 = exchangeDirector.progressOffers[2];
            RancherChatMetadata.Entry[] endingAndNewBeginningConversation1 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "SO, NOW ALL I NEED TO DO IS-",
                    "UH, A CALL INCOMING?",
                    "GIVE ME A SECOND-",
                    "Oh, hello Beatrix! Glad to see you are back! I wonder what brings you here this time?",
                    "There should not be any other problems with the Slimeulation, not any I know, and I am still analyzing the samples you brought me ...",
                    "Oh, you discovered a new sort of slime? Mind you tell me how it looks, but most importantly, how?",
                    "Hmm...",
                    "...",
                    "...",
                    "b...",
                    "B R I L I A N T!!! I never thought of using warmed up diamonds in such a way!",
                    "I seriously cannot wait any longer to analyze it, just wish you told me earlier about this!",
                    "Made out of what appears to be pure fire... may they be long lost relatives to the fire slimes?",
                    "Enough chit-chat, give me some plorts of this slime as well.",
                    "The faster I get the plorts, the faster I get to add them to the Slimeulation.",
                    "... and I guess give you more informations on how to handle it's... problematic thermal radiation you said? Yeah, that too... of course.",
                    "Well-",
                    "UNTIL THEN, I WILL PREPARE MY SLIMEULATION TO WELCOME THE NEW SLIMES, HOPE TO SEE YOU SOON!",
                    },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_bubble_work"),
                    SRObjects.Get<Sprite>("viktor_bubble_surprise"),
                    SRObjects.Get<Sprite>("viktor_bubble_surprise"),
                    SRObjects.Get<Sprite>("viktor_debubbling"),
                    SRObjects.Get<Sprite>("viktor_greeting"),
                    SRObjects.Get<Sprite>("viktor_confused"),
                    SRObjects.Get<Sprite>("viktor_thinking"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_speechless"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_eureka"),
                    SRObjects.Get<Sprite>("viktor_happy"),
                    SRObjects.Get<Sprite>("viktor_thinking"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_explain"),
                    SRObjects.Get<Sprite>("viktor_speechless"),
                    SRObjects.Get<Sprite>("viktor_debubbling"),
                    SRObjects.Get<Sprite>("viktor_bubble_point")
                },
                "intro",
                5
            );

            /*RancherChatMetadata.Entry[] introConversation2 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "SO, NOW ALL I NEED TO DO IS-",
                    "UH, A CALL INCOMING?",
                    "GIVE ME A SECOND-",
                    "Oh, hello Beatrix! Glad to see you are back! I wonder what brings you here this time?",
                    "There should not be any other problems with the Slimeulation, not any I know, and I am still analyzing the samples you brought me ...",
                    "Oh, you discovered a new sort of slime? Mind you tell me how it looks, but most importantly, how?",
                    "Hmm...",
                    "...",
                    "...",
                    "b...",
                    "B R I L I A N T!!! I never thought of using warmed up diamonds in such a way!",
                    "I seriously cannot wait any longer to analyze it, just wish you told me earlier about this!",
                    "Made out of what appears to be pure fire... may they be long lost relatives to the fire slimes?",
                    "Enough chit-chat, give me some plorts of this slime as well.",
                    "The faster I get the plorts, the faster I get to add them to the Slimeulation.",
                    "... and I guess give you more informations on how to handle it's... problematic thermal radiation you said? Yeah, that too... of course.",
                    "Well-",
                    "UNTIL THEN, I WILL PREPARE MY SLIMEULATION TO WELCOME THE NEW SLIMES, HOPE TO SEE YOU SOON!",
                    },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_bubble_work"),
                    SRObjects.Get<Sprite>("viktor_bubble_surprise"),
                    SRObjects.Get<Sprite>("viktor_bubble_surprise"),
                    SRObjects.Get<Sprite>("viktor_debubbling"),
                    SRObjects.Get<Sprite>("viktor_greeting"),
                    SRObjects.Get<Sprite>("viktor_confused"),
                    SRObjects.Get<Sprite>("viktor_thinking"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_speechless"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_eureka"),
                    SRObjects.Get<Sprite>("viktor_happy"),
                    SRObjects.Get<Sprite>("viktor_thinking"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_explain"),
                    SRObjects.Get<Sprite>("viktor_speechless"),
                    SRObjects.Get<Sprite>("viktor_debubbling"),
                    SRObjects.Get<Sprite>("viktor_bubble_point")
                },
                "intro",
                5
            );*/

            RancherChatMetadata.Entry[] repeatConversation2 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "Oh, checking on any progress?",
                    "I would love to keep you updated... but...",
                    "I sadly have to announce tragic news...",
                    "I am not sure if you will be able to bare it...",
                    "My coffee machine broke! Can you believe it?!",
                    "Why do you sound disappointed? It is every scientists' deepest fears to not have their daily dose of caffeine",
                    "Do you know how hard it is to remain awake, working long hours and barely sleep, every day, without a good cup of coffee?",
                    "Anyways, while I continue repairing my coffee machine, feel free to continue collecting the plorts I will need later on",
                    "Maybe I might be able to add them to my coffee machine in order to warm the water...",
                    "On that note-",
                    "ENOUGH CHATTING, SEE YOU LATER ON BEATRIX.",
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_work"),
                    SRObjects.Get<Sprite>("viktor_uneasy"),
                    SRObjects.Get<Sprite>("viktor_sad"),
                    SRObjects.Get<Sprite>("viktor_guilty"),
                    SRObjects.Get<Sprite>("viktor_guilty"),
                    SRObjects.Get<Sprite>("viktor_speechless"),
                    SRObjects.Get<Sprite>("viktor_confused"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_thinking"),
                    SRObjects.Get<Sprite>("viktor_debubbling"),
                    SRObjects.Get<Sprite>("viktor_bubble_point")
                },
                "repeat",
                5
            );


            RancherChatMetadata rancherChatMetadataIntro2 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataIntro2.entries = endingAndNewBeginningConversation1; //.Concat(introConversation2).ToArray();

            RancherChatMetadata rancherChatMetadataRepeat2 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataRepeat2.entries = repeatConversation2;

            rancherChatMetadataIntro2.entries = rancherChatMetadataIntro2.entries.ToArray();

            progressOfferEntry2.rewardLevels[4] = ExchangeCreator.CreateRewardLevel(
                                        25,
                                        rancherChatMetadataIntro2,
                                        rancherChatMetadataRepeat2,
                                        Ids.ELEMENTAL_FIRE_PLORT,
                                        Ids.THERMAL_REGULATOR_REW
                                      );
        }

        public static RancherChatMetadata.Entry[] CreateRancherChatConversation(this ExchangeDirector exchangeDirector, string rancherId, string[] messages1, Sprite[] sprites1, string[] messages2, Sprite[] sprites2, string introrepeat, int level)
        {
            List<RancherChatMetadata.Entry> entryToReturn = new List<RancherChatMetadata.Entry>();
            int i = 0;
            string[] messages = messages1.Concat(messages2).ToArray();
            Sprite[] sprites = sprites1.Concat(sprites2).ToArray();

            Array.ForEach(messages, text =>
            {
                entryToReturn.Add(new RancherChatMetadata.Entry
                {
                    rancherName = (RancherChatMetadata.Entry.RancherName)Enum.Parse(typeof(RancherChatMetadata.Entry.RancherName), rancherId.ToUpperInvariant()),
                    rancherImage = sprites[i],
                    messageBackground = exchangeDirector.GetRancher(rancherId).chatBackground,
                    messageText = text
                });

                string offerId = "";


                offerId = string.
                Concat(new object[]
                {
                    "m.offer.",
                    rancherId + "_rewards",
                    "_level",
                    level,
                    "." + introrepeat + ".",
                    i
                });

                
                
                TranslationPatcher.AddExchangeTranslation(offerId, text);

                i++;
            });

            return entryToReturn.ToArray();
        }

        public static RancherChatMetadata.Entry[] CreateRancherChatConversation(this ExchangeDirector exchangeDirector, string rancherId, string[] messages, Sprite[] sprites, string introrepeat, int level = -1, RancherChatMetadata.Entry[] otherRecurEntries = null)
        {
            List<RancherChatMetadata.Entry> entryToReturn = new List<RancherChatMetadata.Entry>();
            int i = 0;

            if (level == -1)
            {

                Array.Resize(ref otherRecurEntries, messages.Length);
                for (int i2 = 0; i2 < otherRecurEntries.Length; i2++)
                {
                    string[] subs = otherRecurEntries[i2].messageText.Split(".");
                    string finalMessage = "";
                    int number = int.Parse(subs[subs.Length - 1]);
                    
                    number += messages.Length;

                    subs[subs.Length - 1] = number.ToString();

                    for (int i3 = 0; i3 < subs.Length; i3++)
                    {
                        if(finalMessage == "")
                        {
                            finalMessage = string.Concat(new object[] { finalMessage, subs[i3] });
                            continue;
                        }

                        finalMessage = string.Concat(new object[] { finalMessage, ".", subs[i3] });

                    }

                    otherRecurEntries[i2].messageText = finalMessage;
                }
            }

           /* Array.ForEach(messages, text =>
            {
                entryToReturn.Add(new RancherChatMetadata.Entry
                {
                    rancherName = (RancherChatMetadata.Entry.RancherName)Enum.Parse(typeof(RancherChatMetadata.Entry.RancherName), rancherId.ToUpperInvariant()),
                    rancherImage = sprites[i],
                    messageBackground = exchangeDirector.GetRancher(rancherId).chatBackground,
                    messageText = text
                });

                string offerId = "";

                if (level != -1)
                {
                    offerId = string.
                    Concat(new object[]
                    {
                        "m.offer.",
                        rancherId + "_rewards",
                        "_level",
                        level,
                        "." + introrepeat + ".",
                        i
                    });

                }
                else
                {
                    offerId = string.
                    Concat(new object[]
                    {
                        "m.offer.",
                        rancherId + "_recur",
                        "." + introrepeat + ".",
                        i
                    });
                }
                TranslationPatcher.AddExchangeTranslation(offerId, text);

                i++;
            });*/

            return entryToReturn.ToArray();
        }

        //It's unused, so I don't know what you're hoping to find in this function
        public static void CreateRancherQuest(this ExchangeDirector exchangeDirector, ExchangeDirector.OfferType rancherOffer, int count, Identifiable.Id requestedItem,
            ExchangeDirector.NonIdentReward reward, RancherChatMetadata rancherChatMetadataRepeat, RancherChatMetadata rancherChatMetadataIntro, RancherChatMetadata.Entry[] messagesAtEndRepeat,
            RancherChatMetadata.Entry[] messagesAtEndIntro, Sprite spriteToRegister, bool finalQuest = false)
        {
            //("REGISTERING RANCHER QUEST").Log();
            foreach (ExchangeDirector.ProgressOfferEntry progressOfferEntry in exchangeDirector.progressOffers)
            {
                //("Loop initiated with: " + progressOfferEntry.specialOfferType).Log();
                if (progressOfferEntry.specialOfferType == rancherOffer)
                {
                    //"Printing content of rancherChatEndIntro".Log();
                    ExchangeDirector.ProgressOfferEntry currentProgressOfferEntry = exchangeDirector.GetProgressEntry(rancherOffer);
                    foreach (RancherChatMetadata.Entry entry in currentProgressOfferEntry.rancherChatEndIntro.entries)
                    {
                        /*("Checking Entries for: " + rancherOffer + " in rancherChatEndIntro").Log();
                        ("messageBackground: " + entry.messageBackground).Log();
                        ("messagePrefab: " + entry.messagePrefab + ", containing: " + entry.messagePrefab.GetComponents<Component>()).Log();
                        ("messageText: " + entry.messageText).Log();
                        ("rancherImage: " + entry.rancherImage).Log();
                        ("rancherName: " + entry.rancherName).Log();*/
                    }
                    foreach (RancherChatMetadata.Entry entry in currentProgressOfferEntry.rancherChatEndRepeat.entries)
                    {
                        /*("Checking Entries for: " + rancherOffer + " in rancherChatEndRepeat").Log();
                        ("messageBackground: " + entry.messageBackground).Log();
                        ("messagePrefab: " + entry.messagePrefab + ", containing: " + entry.messagePrefab.GetComponents<Component>()).Log();
                        ("messageText: " + entry.messageText).Log();
                        ("rancherImage: " + entry.rancherImage).Log();
                        ("rancherName: " + entry.rancherName).Log();*/
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
                        //("finalQuest is true").Log();

                        Array.ForEach(messagesAtEndIntro, x => newRewardLevel.rancherChatIntro.entries.Prepend(x));
                        Array.ForEach(messagesAtEndRepeat, x => newRewardLevel.rancherChatRepeat.entries.Prepend(x));

                    }
                    else
                    {
                        //("finalQuest is false").Log();
                        ExchangeDirector.OfferType offerType = (ExchangeDirector.OfferType)Enum.Parse(typeof(ExchangeDirector.OfferType), rancherOffer.ToString() + "_RECUR");
                        //("offerType: " + offerType).Log();

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