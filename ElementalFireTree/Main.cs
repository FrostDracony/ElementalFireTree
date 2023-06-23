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
                    () => SRSingleton<SceneContext>.Instance.PediaDirector.pediaModel != null && SRSingleton<SceneContext>.Instance.PediaDirector.IsUnlocked(PediaDirector.Id.FIRE_SLIME),
                    12f
                )
            );
            #endregion

            Slimes.RegisterAllSlimePedia();

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
                //rdr.enabled = false;
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


            //https://answers.unity.com/questions/131279/istrigger-causing-object-to-be-non-rigid.html
            "End of PostLoad".Log();
        }

    }
}
