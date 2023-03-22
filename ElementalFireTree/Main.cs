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

namespace ElementalFireTree
{
    public class Main : ModEntryPoint
    {
        //Specials static variables for our Mod
        public static List<FireColumn> specialColumns = new List<FireColumn>();
        public static bool playerIsInDesert;

        static Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Main), "elementalfire");
        public static AssetBundle assetBundle = AssetBundle.LoadFromStream(manifestResourceStream);

        public static GameObject exampleFireColumn;

        //Called before GameContext.Awake
        //You want to register new things and enum values here, as well as do all your harmony patching
        public override void PreLoad()
        {
            "PreLoad".Log();
            HarmonyInstance.PatchAll();
        }


        // Called before GameContext.Start
        // Used for registering things that require a loaded gamecontext
        public override void Load()
        {
            "Load".Log();

            // We'll need this one later on when we patch the firecolumns
            exampleFireColumn = SRObjects.GetInst<GameObject>("zoneDESERT/cellDesert_ScorchedPlainsNorthEast/Sector/FireColumn_ScorchedPlainsNorthEast");

            #region Creating Fluid Fire
            Material slimeFireBase = SRObjects.Get<Material>("slimeFireBase");
            LiquidCreation.CreateLiquid(Ids.FIRE_LIQUID, "fire", Identifiable.Id.WATER_LIQUID, assetBundle.LoadAsset<Sprite>("LiquidFire"), Color.red, slimeFireBase);
            #endregion

            #region Adding Custom Liquidfire souruce

            #endregion

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






            
            /*SRCallbacks.OnZoneEntered += (ZoneDirector.Zone zone, PlayerState playerState) => {
                "Well... there we are".Log();

                //Updating our personal zonetracker variable when the player enters a new zone
                playerIsInDesert = zone == ZoneDirector.Zone.DESERT;
                ("Player is in:" + playerIsInDesert).Log();

                //If the player left the desert, we're resetting all of the special columns in our list
                if (!playerIsInDesert)
                {
                    "Player is not in desert anymore".Log();

                    foreach (FireColumn fireColumn in specialColumns)
                    {
                        "Well".Log();
                        //Resetting the variables to default
                        fireColumn.SetField("lifetimeHrs", 0.5f);
                        fireColumn.gameObject.transform.localScale = new Vector3(1, 1, 1);
                        fireColumn.SetPrivateProperty("isInOasis", false);
                        fireColumn.SetField("minFireballDelay", 60f);
                        fireColumn.SetField("maxFireballDelay", 80f);
                        
                        "Nothing here".Log();

                        for (int key = 0; key < fireColumn.fireballs.Length; ++key)
                        {
                            "Hmm".Log();
                            //We're going to increase the damage it does to the player
                            FireColumn.FireballEntry currentFireBall = fireColumn.fireballs[key];
                            DamagePlayerOnTouch damagePlayerOnTouch = currentFireBall.prefab.GetComponent<DamagePlayerOnTouch>();
                            ExplodingFireBall explodingFireBall = currentFireBall.prefab.GetComponent<ExplodingFireBall>();
                            if (explodingFireBall != null)
                            {
                                fireColumn.SetField("explodePower", 600f);
                                fireColumn.SetField("explodeRadius", 7f);
                                fireColumn.SetField("minPlayerDamage", 15f);
                                fireColumn.SetField("maxPlayerDamage", 45f);
                            }
                            "Nope".Log();

                            if (damagePlayerOnTouch != null)
                                damagePlayerOnTouch.damagePerTouch /= 2;
                            "DAMMIT".Log();

                            //Also we're going to make them faster
                            currentFireBall.minBallEjectForce /= 1.5f;
                            currentFireBall.maxBallEjectForce /= 1.5f;
                            "I give up".Log();


                        }

                        "Last...".Log();
                        //Removing the column from the list 
                        specialColumns.Remove(fireColumn);
                        "Nvm, you win Bug".Log();
                    }
                }

            };*/

        }

    }
}
