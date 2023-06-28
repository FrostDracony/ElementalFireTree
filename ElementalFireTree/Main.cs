using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRML;
using SRML.SR;
using SRML.Utils;
using UnityEngine;
using Creators;
using ElementalFireTree.Upgrades;

namespace ElementalFireTree
{
    public class Main : ModEntryPoint
    {
        //Specials static variables for our Mod
        public static List<FireColumn> specialColumns = new List<FireColumn>();
        public static List<FireColumn> recoveryColumns = new List<FireColumn>();
        public static bool playerIsInDesert;
        delegate PlayerState.UnlockCondition Delegate();

        static Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Main), "elementalfire");
        public static AssetBundle assetBundle = AssetBundle.LoadFromStream(manifestResourceStream);

        public static GameObject exampleFireColumn;


        //Called before GameContext.Awake
        //You want to register new things and enum values here, as well as do all your harmony patching
        public override void PreLoad()
        {
            "PreLoad".Log();

            #region PLAYER'S_UPGRADE
            Ids.FIRE_LIQUID_VAC.GetTranslation().SetDescriptionTranslation("Useful to absorb the liquid fire from ancient fire storms").SetNameTranslation("Vaccable Liquid Fire Upgrade");
            
            PersonalUpgradeRegistry.RegisterUpgradeLock(
                Ids.FIRE_LIQUID_VAC,
                x => x.CreateBasicLock(
                    new PlayerState.Upgrade?(PlayerState.Upgrade.LIQUID_SLOT),
                    () => SRSingleton<SceneContext>.Instance.PediaDirector.pediaModel != null
                            && SRSingleton<SceneContext>.Instance
                            .PediaDirector
                            .IsUnlocked(PediaDirector.Id.FIRE_SLIME)
                            && SceneContext.Instance.ExchangeDirector.progressDir.GetProgress(SceneContext.Instance.ExchangeDirector.GetProgressEntry(ExchangeDirector.OfferType.VIKTOR).progressType) == 4,
                    12f
                )
            );
            #endregion

            Slimes.RegisterAllSlimePedia();
            LandPlotUpgradeRegistry.RegisterPurchasableUpgrade<CorralUI>(ThermalRegulator.CreateThermalRegulatorEntry());
            LandPlotUpgradeRegistry.RegisterPlotUpgrader<ThermalRegulator>(LandPlot.Id.CORRAL);
            HarmonyInstance.PatchAll();
        }


        // Called before GameContext.Start
        // Used for registering things that require a loaded gamecontext
        public override void Load()
        {
            "Load".Log();


            GameObject testObject = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.STRANGE_DIAMOND_CRAFT).CreatePrefabCopy();
            testObject.GetComponent<Identifiable>().id = Ids.ELEMENTAL_FIRE_ENHANCER;
            testObject.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
            //UnityEngine.Object.Destroy(testObject.GetComponent<EventBreakOnImpact>());

            GameObject radiusBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            radiusBall.AddComponent<StartFusionProcess>();
            radiusBall.transform.SetParent(testObject.transform);
            radiusBall.transform.localScale = new Vector3(4, 4, 4);

            radiusBall.layer = LayerMask.NameToLayer("Launched");

            ("Lets see the scale: " + radiusBall.transform.localScale).Log();

            /*Rigidbody fsTriggerRb = radiusBall.AddComponent<Rigidbody>();
            //fsTriggerRb.useGravity = false;
            fsTriggerRb.detectCollisions = false;//true;*/

            foreach (Renderer rdr in radiusBall.GetComponentsInChildren<Renderer>())
            {
                rdr.enabled = false;
            }
            /*foreach (SphereCollider coll in radiusBall.GetComponentsInChildren<SphereCollider>())
            {
                coll.enabled = true;
                coll.isTrigger = true;
            }*/

            //radiusBall.GetComponent<SphereCollider>().isTrigger = true;

            /*foreach (SphereCollider coll in radiusBall.GetComponentsInChildren<SphereCollider>())
            {
                //coll.enabled = true;
                coll.isTrigger = true;
            }*/

            radiusBall.GetComponent<SphereCollider>().isTrigger = true;
            //radiusBall.GetComponent<SphereCollider>().enabled = true;

            
            LookupRegistry.RegisterIdentifiablePrefab(testObject);
            AmmoRegistry.RegisterAmmoPrefab(PlayerState.AmmoMode.DEFAULT, testObject);
            Sprite icon = SRSingleton<SceneContext>.Instance.PediaDirector.entries.First((PediaDirector.IdEntry x) => x.id == PediaDirector.Id.ORNAMENTS).icon;
            LookupRegistry.RegisterVacEntry(Ids.ELEMENTAL_FIRE_ENHANCER, new Color32(138, 87, 40, 255), icon);
            
            //It is said to enhance the strenghts of everything that's fire, but to activate it it requires some ancient form of liquid...
            TranslationPatcher.AddActorTranslation("l.elemental_fire_combinator", "Elemental Fire Combinator");
            

            // We'll need this one later on when we patch the firecolumns
            exampleFireColumn = SRObjects.GetInst<GameObject>("zoneDESERT/cellDesert_ScorchedPlainsNorthEast/Sector/FireColumn_ScorchedPlainsNorthEast");

            #region PLAYER'S_UPGRADE
            LookupRegistry.RegisterUpgradeEntry(Ids.FIRE_LIQUID_VAC, assetBundle.LoadAsset<Sprite>("FireUpgrade"), 30000);
            #endregion

            #region Creating Fluid Fire
            Material slimeFireBase = SRObjects.Get<Material>("slimeFireBase");
            GameObject fireLiquidObj = LiquidCreation.CreateLiquid(Ids.FIRE_LIQUID, "fire", Identifiable.Id.WATER_LIQUID, assetBundle.LoadAsset<Sprite>("LiquidFire"), Color.red, slimeFireBase);

            fireLiquidObj.AddComponent<FireLiquidIgnition>();
            #endregion

            #region Handling Slimes
            Slimes.RegisterAllSlimes();
            #endregion

            //LookupRegistry.RegisterUpgradeEntry(Ids.FIRE_LIQUID_VAC, assetBundle.LoadAsset<Sprite>("gravityCuff"), 2000);

        }

        //Called after all mods Load's have been called
        //Used for editing existing assets in the game, not a registry step
        public override void PostLoad()
        {
            "PostLoad".Log();
            SRCallbacks.OnSaveGameLoaded += SRCallbacks_OnSaveGameLoaded;

            SRCallbacks.OnSaveGameLoaded += (SceneContext t) => {
                //Updating our personal zonetracker variable as soon as the game is loaded
                playerIsInDesert = t.PlayerZoneTracker.GetCurrentZone() == ZoneDirector.Zone.DESERT;
                ("Player is in:" + playerIsInDesert).Log();
            };


            SRCallbacks.OnZoneEntered += (ZoneDirector.Zone zone, PlayerState playerState) =>
            {
                playerIsInDesert = zone == ZoneDirector.Zone.DESERT;
                ("Player is in:" + playerIsInDesert).Log();
            };

            /*GameObject fireSlime = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.FIRE_SLIME);
            GameObject fSTrigger = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fSTrigger.name = ("FireSlime Trigger");
            fSTrigger.transform.SetParent(fireSlime.transform);
            //fSTrigger.GetComponent<Renderer>().enabled = false;
            fSTrigger.AddComponent<SphereCollider>();
            fSTrigger.GetComponent<SphereCollider>().isTrigger = true;
            fSTrigger.GetComponent<SphereCollider>().enabled = true;
            fSTrigger.transform.localScale = fireSlime.transform.localScale;
            
            Rigidbody fsTriggerRb = fSTrigger.AddComponent<Rigidbody>();
            fsTriggerRb.useGravity = false;
            fsTriggerRb.detectCollisions = false;*/


            //Making RegularFireSlimes transform into elemental ones
            SRSingleton<GameContext>.Instance.SlimeDefinitions
                .GetSlimeByIdentifiableId(Identifiable.Id.FIRE_SLIME)
                .Diet.EatMap.Add(new SlimeDiet.EatMapEntry
            {
                eats = Ids.FIRE_LIQUID,
                becomesId = Ids.ELEMENTAL_FIRE_SLIME,
                driver = SlimeEmotions.Emotion.NONE,
                producesId = Identifiable.Id.NONE
            });

            SRSingleton<GameContext>
                .Instance
                .LookupDirector
                .GetPrefab(Identifiable.Id.STRANGE_DIAMOND_CRAFT)
                .AddComponent<CrystalAbsorbElementalFire>();

            //https://answers.unity.com/questions/131279/istrigger-causing-object-to-be-non-rigid.html




            "End of PostLoad".Log();
        }

        void SRCallbacks_OnSaveGameLoaded(SceneContext t)
        {
            ExchangeDirector exchangeDirector = SRSingleton<SceneContext>.Instance.ExchangeDirector;
            ExchangeDirector.ProgressOfferEntry progressOfferEntry2 = exchangeDirector.progressOffers[2];
            Array.Resize(ref progressOfferEntry2.rewardLevels, progressOfferEntry2.rewardLevels.Length + 2);

            #region Changing Mochi's RewardLevels
            /*int i = 0;
            Array.ForEach(exchangeDirector.progressOffers, 
                x => { 
                    ("index " + i + " + " + x.specialOfferType + " and a progresstype of " + x.progressType).Log();
                    ("And it has as intro text: ").Log();
                    Array.ForEach(x.rancherChatEndIntro.entries, x1 => ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x1.messageText)).Log());
                    ("And it has as repeat text: ").Log();
                    Array.ForEach(x.rancherChatEndRepeat.entries, x1 => ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x1.messageText)).Log());

                    i++;
                }
            );

            int debugIndex = 0;

            progressOfferEntry2.rewardLevels[0].requestedItem = Identifiable.Id.PINK_SLIME;
            progressOfferEntry2.rewardLevels[0].count = 1;

            "".Log();
            "For rewardLevel 0, the introtext is:".Log();
            Array.ForEach(progressOfferEntry2.rewardLevels[0].rancherChatIntro.entries,
                x => 
                { 
                    ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x.messageText) + " at index: " + debugIndex + " and image " + x.rancherImage).Log(); 
                    debugIndex++;
                }
            );

            debugIndex = 0;

            "".Log();
            "For rewardLevel 0, the repeattext is:".Log();
            Array.ForEach(progressOfferEntry2.rewardLevels[0].rancherChatRepeat.entries,
                x =>
                {
                    ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x.messageText) + " at index: " + debugIndex + " and image " + x.rancherImage).Log();
                    debugIndex++;
                }
            );

            debugIndex = 0;

            progressOfferEntry2.rewardLevels[1].requestedItem = Identifiable.Id.PINK_SLIME;
            progressOfferEntry2.rewardLevels[1].count = 1;

            "".Log();
            "For rewardLevel 1, the introtext is:".Log();
            Array.ForEach(progressOfferEntry2.rewardLevels[1].rancherChatIntro.entries,
                x =>
                {
                    ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x.messageText) + " at index: " + debugIndex + " and image " + x.rancherImage).Log();
                    debugIndex++;
                }
            );

            debugIndex = 0;

            "".Log();
            "For rewardLevel 1, the repeattext is:".Log();
            Array.ForEach(progressOfferEntry2.rewardLevels[1].rancherChatRepeat.entries,
                x =>
                {
                    ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x.messageText) + " at index: " + debugIndex + " and image " + x.rancherImage).Log();
                    debugIndex++;
                }
            );

            debugIndex = 0;

            progressOfferEntry2.rewardLevels[2].requestedItem = Identifiable.Id.PINK_SLIME;
            progressOfferEntry2.rewardLevels[2].count = 1;

            "".Log();
            "For rewardLevel 2, the introtext is:".Log();
            Array.ForEach(progressOfferEntry2.rewardLevels[2].rancherChatIntro.entries,
                x =>
                {
                    ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x.messageText) + " at index: " + debugIndex + " and image " + x.rancherImage).Log();
                    debugIndex++;
                }
            );

            debugIndex = 0;

            "".Log();
            "For rewardLevel 2, the repeattext is:".Log();
            ("And it has a length of: " + progressOfferEntry2.rewardLevels[2].rancherChatRepeat.entries.Length).Log();
            Array.ForEach(progressOfferEntry2.rewardLevels[2].rancherChatRepeat.entries,
                x =>
                {
                    ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x.messageText) + " at index: " + debugIndex + " and image " + x.rancherImage).Log();
                    debugIndex++;
                }
            );

            debugIndex = 0;*/
            #endregion

            "huh it worked until here?".Log();

            #region Conversations
            #region FirstConvo
            RancherChatMetadata.Entry[] introConversation1 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "SO, NOW ALL I NEED TO DO IS-",
                    "UH, A CALL INCOMING?",
                    "GIVE ME A SECOND-",
                    "Oh, hello Beatrix! Glad to see you are back! I wonder what brings you here?",
                    "There should not be any other problems with the Slimeulation, not any I know...",
                    "Oh, a new sort of slime?",
                    "Hmm...",
                    "...",
                    "...",
                    "b...",
                    "B R I L I A N T!!!",
                    "I seriously cannot wait any longer to analyze it!",
                    "Being able to manipulate fire... may they be long lost relatives to the fire slimes?",
                    "Enough chit-chat, give me some plorts of this slime",
                    "The faster I get the plorts, the faster I get to add them to the Slimeulation",
                    "... and I guess give you more informations on how to handle it's problematic thermal radiation too... of course",
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
                });

            /*
             * All the Images:
             * viktor_greeting
             * viktor_happy
             * viktor_default2
             * viktor_explain
             * viktor_thinking
             * viktor_bubble_work 
             * viktor_bubble_surprise
             * viktor_debubbling
             * viktor_confused
             * viktor_eureka
             * viktor_sad
             * viktor_work
             * viktor_uneasy
             * viktor_guilty
             * viktor_static
             * viktor_bubble_point
             * viktor_speechless
            */

            RancherChatMetadata.Entry[] repeatConversation1 = exchangeDirector.CreateRancherChatConversation("viktor",
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
                });

            RancherChatMetadata.Entry[] endingConversation1 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "GREAT JOB!",
                    "Oh...",
                    "Great job!",
                    "So... I have two good news and a bad new",
                    "The first good one...",
                    "My coffee machine is back on track! Meaning I was able to work back to my normal workrythm.",
                    "Oh..., I guess that is not the news you were expecting",
                    "The other one is that I was able to find a solution to keep your new slimes from burning everything down near them.",
                    "The bad new... I sadly was not able to include the new slimes to my Slimeulation",
                    "Something is impeding me, but it seems to be outside of my current undrestanding",
                    "But hey, thanks to Thora I was able to prepare your solution",
                    "Hmm, Thora? Are you surprised that I have a coffee and music friend?",
                    "She was even able to repair my authentic, broken coffee machine! I truly could not continue my work without it.",
                    "I still don't know how she managed to find the solution.",
                    "But as long as I get my cup of coffee in the morning while listening to some jazz, I will not question it.",
                    "Anyways, still feel free to enter my Slimeulation and gather more bug reports.",
                    "Now-",
                    "IF YOU WILL EXCUSE ME, I HAVE SOME EXPERIMENTS TO RUN, I WISH YOU A WONDERFUL DAY MY FRIEND",
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_bubble_work"),
                    SRObjects.Get<Sprite>("viktor_bubble_surprise"),
                    SRObjects.Get<Sprite>("viktor_debubbling"),
                    SRObjects.Get<Sprite>("viktor_greeting"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_eureka"),
                    SRObjects.Get<Sprite>("viktor_confused"),
                    SRObjects.Get<Sprite>("viktor_happy"),
                    SRObjects.Get<Sprite>("viktor_sad"),
                    SRObjects.Get<Sprite>("viktor_uneasy"),
                    SRObjects.Get<Sprite>("viktor_explain"),
                    SRObjects.Get<Sprite>("viktor_speechless"),
                    SRObjects.Get<Sprite>("viktor_eureka"),
                    SRObjects.Get<Sprite>("viktor_thinking"),
                    SRObjects.Get<Sprite>("viktor_default2"),
                    SRObjects.Get<Sprite>("viktor_work"),
                    SRObjects.Get<Sprite>("viktor_debubbling"),
                    SRObjects.Get<Sprite>("viktor_bubble_work")
                });
            
            
            "Still working?".Log();

            RancherChatMetadata rancherChatMetadataIntro1 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataIntro1.entries = introConversation1;

            "Damn, still working".Log();

            RancherChatMetadata rancherChatMetadataRepeat1 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataRepeat1.entries = repeatConversation1;

            "Holy, is this really really still working?".Log();

            rancherChatMetadataIntro1.entries = progressOfferEntry2.rancherChatEndIntro.entries.Concat(rancherChatMetadataIntro1.entries).ToArray();
            progressOfferEntry2.rancherChatEndIntro.entries = endingConversation1;

            "No fucking way".Log();

            progressOfferEntry2.rewardLevels[3] = ExchangeCreator.CreateRewardLevel(
                                        1,
                                        rancherChatMetadataIntro1,
                                        rancherChatMetadataRepeat1,
                                        Identifiable.Id.BLUE_ECHO,
                                        Ids.FIRE_LIQUID_VAC_REW
                                      );

            "Nope still not".Log();

            exchangeDirector.nonIdentRewardDict.Add(
                Ids.FIRE_LIQUID_VAC_REW,
                assetBundle.LoadAsset<Sprite>("FireUpgrade")
            );
            #endregion

            #region SecondConvo
            RancherChatMetadata.Entry[] introConversation2 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "UNTIL THEN, I WILL PREPARE MY SLIMEULATION TO WELCOME THE NEW SLIMES, HOPE TO SEE YOU SOON!",
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_bubble_point")
                }
            );

            /*
             * All the Images:
             * viktor_greeting
             * viktor_happy
             * viktor_default2
             * viktor_explain
             * viktor_thinking
             * viktor_bubble_work 
             * viktor_bubble_surprise
             * viktor_debubbling
             * viktor_confused
             * viktor_eureka
             * viktor_sad
             * viktor_work
             * viktor_uneasy
             * viktor_guilty
             * viktor_static
             * viktor_bubble_point
             * viktor_speechless
            */

            RancherChatMetadata.Entry[] repeatConversation2 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "ENOUGH CHATTING, SEE YOU LATER ON BEATRIX."
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_bubble_point")
                });

            RancherChatMetadata.Entry[] endingConversation2 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "GREAT JOB!",
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_bubble_work")
                });


            "Still working?".Log();

            RancherChatMetadata rancherChatMetadataIntro2 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataIntro2.entries = introConversation1;

            "Damn, still working".Log();

            RancherChatMetadata rancherChatMetadataRepeat2 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataRepeat2.entries = repeatConversation2;

            "Holy, is this really really still working?".Log();

            rancherChatMetadataIntro2.entries = progressOfferEntry2.rancherChatEndIntro.entries.Concat(rancherChatMetadataIntro2.entries).ToArray();
            progressOfferEntry2.rancherChatEndIntro.entries = endingConversation2;

            "No fucking way".Log();

            progressOfferEntry2.rewardLevels[4] = ExchangeCreator.CreateRewardLevel(
                                        1,
                                        rancherChatMetadataIntro2,
                                        rancherChatMetadataRepeat2,
                                        Identifiable.Id.BLUE_ECHO,
                                        Ids.THERMAL_REGULATOR_REW
                                      );

            "Nope still not".Log();

            exchangeDirector.nonIdentRewardDict.Add(
                Ids.THERMAL_REGULATOR_REW,
                assetBundle.LoadAsset<Sprite>("LiquidFire")
            );
            #endregion

            #region SkippingViktorsDialogue
            RancherChatMetadata.Entry[] Conversation3 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "GREAT JOB!",
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_bubble_work")
                });

            RancherChatMetadata rancherChatMetadata3 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadata3.entries = Conversation3;

            progressOfferEntry2.rewardLevels[0] = ExchangeCreator.CreateRewardLevel(
                                        1,
                                        rancherChatMetadata3,
                                        rancherChatMetadata3,
                                        Identifiable.Id.PINK_SLIME,
                                        progressOfferEntry2.rewardLevels[0].reward
                                      );
            progressOfferEntry2.rewardLevels[1] = ExchangeCreator.CreateRewardLevel(
                                        1,
                                        rancherChatMetadata3,
                                        rancherChatMetadata3,
                                        Identifiable.Id.PINK_SLIME,
                                        progressOfferEntry2.rewardLevels[0].reward
                                      );
            progressOfferEntry2.rewardLevels[2] = ExchangeCreator.CreateRewardLevel(
                                        1,
                                        rancherChatMetadata3,
                                        rancherChatMetadata3,
                                        Identifiable.Id.PINK_SLIME,
                                        progressOfferEntry2.rewardLevels[0].reward
                                      );
            #endregion

            #endregion
        }


    }
}
