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
using SRML.SR.Translation;

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

            /*foreach (KeyValuePair<ExchangeDirector.NonIdentReward, Sprite> entry in SRSingleton<SceneContext>.Instance.ExchangeDirector.nonIdentRewardDict)
            {
                ("\tkey: ").Log();
                entry.Key.Log();
                ("\tvalue: ").Log();
                entry.Value.Log();
            }*/

            

            PediaRegistry.RegisterIdentifiableMapping(Ids.ELEMENTAL_FIRE_SLIME_ENTRY, Ids.ELEMENTAL_FIRE_SLIME);
            PediaRegistry.SetPediaCategory(Ids.ELEMENTAL_FIRE_SLIME_ENTRY, PediaRegistry.PediaCategory.SLIMES);

            PediaRegistry.RegisterIdentifiableMapping(PediaDirector.Id.PLORTS, Ids.ELEMENTAL_FIRE_PLORT);
            Identifiable.PLORT_CLASS.Add(Ids.ELEMENTAL_FIRE_PLORT);
            Identifiable.NON_SLIMES_CLASS.Add(Ids.ELEMENTAL_FIRE_PLORT);
            
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

            new ResourcePediaEntryTranslation(Ids.ELEMENTAL_FIRE_ENHANCER_ENTRY)
                .SetDescriptionTranslation("Created after infusing it with fire liquid, it seems as if it was red, shining by the power of fire itself.")
                .SetTitleTranslation("Elemental Fire Enhancer")
                .SetIntroTranslation("A variant of strange diamond.")
                .SetResourceTypeTranslation("???")
                .SetFavoredByTranslation("(not slime food)")
                .SetFavoredByLabelTranslation("(not slime food)")
                .SetHowToUseTranslation("It seems as if it would resonate with an object and another droplet of fire liquid...");

            PediaRegistry.RegisterIdentifiableMapping(Ids.ELEMENTAL_FIRE_ENHANCER_ENTRY, Ids.ELEMENTAL_FIRE_ENHANCER);
            PediaRegistry.SetPediaCategory(Ids.ELEMENTAL_FIRE_ENHANCER_ENTRY, PediaRegistry.PediaCategory.RESOURCES);
            PediaRegistry.RegisterIdEntry(Ids.ELEMENTAL_FIRE_ENHANCER_ENTRY, SRObjects.Get<Sprite>("iconCraftStrangeDiamond"));
            
            LandPlotUpgradeRegistry.RegisterPurchasableUpgrade<CorralUI>(ThermalRegulator.CreateThermalRegulatorEntry());
            LandPlotUpgradeRegistry.RegisterPlotUpgrader<ThermalRegulator>(LandPlot.Id.CORRAL);

            HarmonyInstance.PatchAll();
        }


        // Called before GameContext.Start
        // Used for registering things that require a loaded gamecontext
        public override void Load()
        {
            "Load".Log();

            

            Plorts.RegisterAllPlorts();

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
            
            
            
            //SRSingleton<SceneContext>.Instance.PediaDirector.IsUnlocked(Ids.ELEMENTAL_FIRE_SLIME_ENTRY).Log();



            GameObject fireQuad = PrefabUtils.CopyPrefab(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.FIRE_PLORT).FindChild("FireQuad"));
            fireQuad.transform.localScale = new Vector3(4.5f, 4.5f, 4.5f);

            //SRObjects.Get<Shader>("SR/Actor/Recolor x8").PrintContent();
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color00", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color01", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color10", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color11", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color20", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color21", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color30", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color31", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color40", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color41", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color50", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color51", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color60", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color61", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color70", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color71", Color.red);
            testObject.FindChild("strangeDiamond_ld").GetComponent<MeshRenderer>().material.SetColor("_Color01", Color.red);

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

            SRSingleton<SceneContext>.Instance.ExchangeDirector.nonIdentRewardDict.Add(
                Ids.FIRE_LIQUID_VAC_REW,
                assetBundle.LoadAsset<Sprite>("FireUpgrade")
            );

            SRSingleton<SceneContext>.Instance.ExchangeDirector.nonIdentRewardDict.Add(
                Ids.THERMAL_REGULATOR_REW,
                assetBundle.LoadAsset<Sprite>("LiquidFire")
            );

            SRCallbacks.OnSaveGameLoaded += SRCallbacks_OnSaveGameLoaded;

            SRCallbacks.OnSaveGameLoaded += (SceneContext t) => {
                //Updating our personal zonetracker variable as soon as the game is loaded
                playerIsInDesert = t.PlayerZoneTracker.GetCurrentZone() == ZoneDirector.Zone.DESERT;
                //("Player is in:" + playerIsInDesert).Log();
            };


            SRCallbacks.OnZoneEntered += (ZoneDirector.Zone zone, PlayerState playerState) =>
            {
                playerIsInDesert = zone == ZoneDirector.Zone.DESERT;
                //("Player is in:" + playerIsInDesert).Log();
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
            ExchangeDirector.ProgressOfferEntry progressOfferEntry2 = exchangeDirector.progressOffers[2];
            RancherChatMetadata.Entry[] OGEndIntro = progressOfferEntry2.rancherChatEndIntro.entries.DeepCopy();
            //https://www.youtube.com/watch?v=DNM0WhKFmm8
            #region FirstConvo
            RancherChatMetadata.Entry[] introConversation1 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "Oh, actually, before you leave Beatrix",
                    "There's something important I have to tell you. Do you have a minute?",
                    "I have been seeing some... distubations in the glass deserts.",
                    "Like, a huge amount of energy is being released during each firestorm.",
                    "But I really cannot afford to leave my research behind in this moment.",
                    "Would... it be possible for you to maybe check it out on my behalf?",
                    "If there truly is this amount of energy, then bring me back some plorts of fire slimes. They must be linked to all of this somehow",
                    "Thank you Beatrix, hope to hear back from you soon.",
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_greeting"),
                    SRObjects.Get<Sprite>("viktor_explain"),
                    SRObjects.Get<Sprite>("viktor_confused"),
                    SRObjects.Get<Sprite>("viktor_work"),
                    SRObjects.Get<Sprite>("viktor_sad"),
                    SRObjects.Get<Sprite>("viktor_uneasy"),
                    SRObjects.Get<Sprite>("viktor_thinking"),
                    SRObjects.Get<Sprite>("viktor_default2")
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
             * viktor_looking_down
            */

            RancherChatMetadata.Entry[] repeatConversation1 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "OH, HELLO BEATRIX",
                    "AS YOU CAN SEE, I AM IN THE MIDDLE OF A VERY IMPORTANT EXPERIMENT RIGHT NOW",
                    "COULD YOU PLEASE COME BACK ONCE YOU ARE DONE WITH COLLECTING THE SAMPLES? BY THEN I SHOULD BE DONE TOO.",
                    "SORRY AGAIN THAT I FORGOT TO WARN YOU, BUT FIRE SLIME PLORTS GET PRETTY INSTABLE WHILE I ANALYZE THEM, SO I NEED A HUGE AMOUNT OF THEM",
                    "YOU CAN TAKE YOUR TIME WITH IT, IN THE MEANWHILE, I WILL JUST TRY TO FIGURE OUT ON MY OWN WHAT'S HAPPENING, HOPE SEE YOU SOON!",
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_bubble_surprise"),
                    SRObjects.Get<Sprite>("viktor_bubble_work"),
                    SRObjects.Get<Sprite>("viktor_bubble_work"),
                    SRObjects.Get<Sprite>("viktor_bubble_surprise"),
                    SRObjects.Get<Sprite>("viktor_bubble_point")
                }
            );
            

            RancherChatMetadata.Entry[] endingConversation1 = exchangeDirector.CreateRancherChatConversation("viktor",
                new string[]
                {
                    "What a wonderful job you completed there!",
                    "So, it appears that...",
                    "...",
                    "Listen, I know that it will appear strange, but I can not get any valuable information out of these plorts",
                    "Well, no information about what is happening over in the glass desert.",
                    "Nothing new other than giant, dangerous fire storms, and what appears to be... liquified fire?",
                    "The other only thing I was able to identify is that the DNA of the fire slimes seems to be... changing?",
                    "But only of some, not of everyone one.",
                    "I wonder what it means...",
                    "But hey, since I was never before able to get this many fire plorts, I was able to build this thingy",
                    "An upgrade that should allow you to get a hold of these special liquified fire. I was able to get some samples of the strange material for my own experiments.",
                    "That's why I do not have an use for it at the moment, other than use it to clean my strange diamonds",
                    "What, why are you looking at me with that bizzare expression? By applying small quantities of it, it can remove nearly everything on it's surface, and it makes it look as clean as ever!",
                    "I do admit that it then looks a bit... more red than usual? But it then returns back to it's original colors, so I assume it's just the crystal warming up.",
                    "If you want tho, you can play around with it I guess, maybe just try to not make it too hot.",
                    "Alright, enough chatting-",
                    "IF YOU WILL EXCUSE ME, I HAVE SOME EXPERIMENTS TO RUN, FEEL FREE TO CALL ME IF YOU FIND SOMETHING, I WISH YOU A WONDERFUL DAY MY FRIEND",
                },
                new Sprite[]
                {
                    SRObjects.Get<Sprite>("viktor_greeting"),
                    SRObjects.Get<Sprite>("viktor_explaining"),
                    SRObjects.Get<Sprite>("viktor_default2"),
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
                }
            );
            
            
            "Still working?".Log();

            RancherChatMetadata rancherChatMetadataIntro1 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataIntro1.entries = introConversation1;

            "Damn, still working".Log();

            RancherChatMetadata rancherChatMetadataRepeat1 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataRepeat1.entries = repeatConversation1;

            /*"Holy, is this really really still working?".Log();


            int debugIndex1 = 0;

            "".Log();
            "For rewardLevel 3, the repeattext is:".Log();
            ("It's length: " + progressOfferEntry2.rewardLevels.Length).Log();
            ("And it has a length of: " + progressOfferEntry2.rewardLevels[progressOfferEntry2.rewardLevels.Length-1].rancherChatRepeat.entries.Length).Log();
            Array.ForEach(progressOfferEntry2.rewardLevels[progressOfferEntry2.rewardLevels.Length-1].rancherChatRepeat.entries,
                x =>
                {
                    ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x.messageText) + " at index: " + debugIndex1 + " and image " + x.rancherImage).Log();
                    debugIndex1++;
                }
            );

            debugIndex1 = 0;

            "For final, the text is:".Log();
            Array.ForEach(progressOfferEntry2.rancherChatEndIntro.entries,
                x =>
                {
                    ("      " + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("exchange").Get(x.messageText) + " at index: " + debugIndex1 + " and image " + x.rancherImage).Log();
                    debugIndex1++;
                }
            );*/

            Array.Resize(ref progressOfferEntry2.rewardLevels, progressOfferEntry2.rewardLevels.Length + 2);

            rancherChatMetadataIntro1.entries = OGEndIntro.Concat(rancherChatMetadataIntro1.entries).ToArray();
            progressOfferEntry2.rancherChatEndIntro.entries = OGEndIntro;

            "No fucking way".Log();

            progressOfferEntry2.rewardLevels[3] = ExchangeCreator.CreateRewardLevel(
                                        750,
                                        rancherChatMetadataIntro1,
                                        rancherChatMetadataRepeat1,
                                        Identifiable.Id.FIRE_PLORT,
                                        Ids.FIRE_LIQUID_VAC_REW
                                      );

            "Nope still not".Log();

            
            #endregion

            #region SecondConvo
            //Hmm
            RancherChatMetadata.Entry[] introConversation2 = exchangeDirector.CreateRancherChatConversation("viktor",
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
                }
            );


            /**All the Images:
             *viktor_greeting
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
             * viktor_speechless*/



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
                }
            );

            RancherChatMetadata.Entry[] endingConversation2 = exchangeDirector.CreateRancherChatConversation("viktor",
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
                    /*"Now-",
                    "IF YOU WILL EXCUSE ME, I HAVE SOME EXPERIMENTS TO RUN, I WISH YOU A WONDERFUL DAY MY FRIEND",*/
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
                    /*SRObjects.Get<Sprite>("viktor_debubbling"),
                    SRObjects.Get<Sprite>("viktor_bubble_work")*/
                }
            );


            "Still working?".Log();

            RancherChatMetadata rancherChatMetadataIntro2 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataIntro2.entries = introConversation2;

            "Damn, still working".Log();

            RancherChatMetadata rancherChatMetadataRepeat2 = ScriptableObject.CreateInstance<RancherChatMetadata>();
            rancherChatMetadataRepeat2.entries = repeatConversation2;

            "Holy, is this really really still working?".Log();

            rancherChatMetadataIntro2.entries = rancherChatMetadataIntro2.entries.ToArray();
            progressOfferEntry2.rancherChatEndIntro.entries = endingConversation2;

            "No fucking way".Log();

            progressOfferEntry2.rewardLevels[4] = ExchangeCreator.CreateRewardLevel(
                                        25,
                                        rancherChatMetadataIntro2,
                                        rancherChatMetadataRepeat2,
                                        Ids.ELEMENTAL_FIRE_PLORT,
                                        Ids.THERMAL_REGULATOR_REW
                                      );

            "Nope still not".Log();

            
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
