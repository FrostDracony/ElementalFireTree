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
    [HarmonyPatch(typeof(FirestormActivator), "MaybeTriggerNearbyColumns")]
    class FirestormActivatorPatch
    {
        public static bool Prefix(FirestormActivator __instance)
        {
            if (!__instance.GetPrivateField<TimeDirector>("timeDir").HasReached(__instance.GetPrivateField<WorldModel>("worldModel").nextFirecolumnTime) || __instance.GetPrivateField<TimeDirector>("timeDir").HasReached(__instance.GetPrivateField<WorldModel>("worldModel").endFirecolumnsTime))
                return false;
            __instance.GetPrivateField<WorldModel>("worldModel").currFirestormMode = FirestormActivator.Mode.ACTIVE;
            List<FireColumn> nearbyColumns = new List<FireColumn>();
            foreach (Component region in __instance.GetPrivateField<RegionMember>("member").regions)
            {
                FirestormController component = region.GetComponent<FirestormController>();
                if (component != null)
                    component.AddColumnsToList(nearbyColumns);
            } 
            if (nearbyColumns.Count > 0)
            {
                Dictionary<FireColumn, float> weightMap = new Dictionary<FireColumn, float>();
                foreach (FireColumn key in nearbyColumns)
                {
                    if (!key.IsInOasis() && !key.IsFireActive())
                        weightMap[key] = 1f / Mathf.Max(0.1f, (key.transform.position - __instance.transform.position).sqrMagnitude);
                }
                FireColumn fireColumn = Randoms.SHARED.Pick(weightMap, null);


                // ----------------------------------------------- \\
                // ----------------------------------------------- \\
                // ----------------------------------------------- \\
                // ----------------------------------------------- \\

                #region Modifying our special column
                //If we already have enough of the special columns, then we won't need more
                if (Main.specialColumns.Count <= 10)
                {
                    //Getting a random column from all of them
                    int randomColumn = Extensions.rnd.Next(nearbyColumns.Count);
                    FireColumn column = nearbyColumns[randomColumn];
                    /*Main.recoveryColumns.Add(column);

                    Vector3 originalPosition = column.transform.position;
                    Quaternion originalRotation = column.transform.rotation;
                    column.Destroy();
                    column = Main.exampleFireColumn.CreatePrefabCopy().GetComponentInChildren<FireColumn>();

                    //Setting the original position/rotation
                    column.transform.position = originalPosition;
                    column.transform.rotation = originalRotation;*/

                    //Setting the lifetime, aka how long it's going to last around
                    //("Printing columns lifetime: " + column.GetField("lifetimeHrs")).Log();
                    //^^^ = 0.5
                    column.SetField("lifetimeHrs", 2f);
                    
                    //Setting the size
                    //("Printing columns size: " + column.gameObject.transform.localScale).Log();
                    //^^^ = 1,1,1
                    column.gameObject.transform.localScale = new Vector3(7, 7, 7);

                    //We don't want that the game can block these special firestorms, so they can spawn inside an oasis, regardless by the fact that the oasis should be stopping it
                    column.SetPrivateProperty("isInOasis", false);

                    //Making the fireball spawn rate faster
                    column.SetField("minFireballDelay", 2f); //Original is 60
                    column.SetField("maxFireballDelay", 5f); //Original is 80

                    //Adding custom Liquidfire source

                    LiquidSource liquidSource = null;

                    if (column.transform.gameObject.GetComponentInChildren<LiquidSource>(true) == null) //In case there isn't a LiquidSource already present
                    {

                        //GameObject @object = new GameObject("liquidSourceContainer"); //We're using this GameObject that way we have more control on where to position our 
                        GameObject @object = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        @object.name = "liquidSourceContainer";
                        @object.layer = LayerMask.NameToLayer("Water");

                        CapsuleCollider capsuleCollider = @object.AddComponent<CapsuleCollider>(); //We're adding a CapsuleCollider component to our gameobject for the LiquidSource

                        #region Setting CapsuleColliders properties
                        capsuleCollider.direction = 1;
                        capsuleCollider.height = 2f; //2f
                        capsuleCollider.radius = 3.5f; //7f
                        capsuleCollider.contactOffset = 0.01f;
                        capsuleCollider.isTrigger = true;
                        #endregion

                        //USE NORMAL WATER INSTEAD OF THE SPECIAL ONE YOU BAKA
                        //asdfghjklm

                        liquidSource = @object.AddComponent<LiquidSource>(); //We're adding the LiquidSource component to our gameobject
                        @object.transform.position = column.transform.position + new Vector3(0f, 4f); //Setting the position of the GameObject
                        @object.transform.localScale = new Vector3(1f, 1f, 1f);
                        @object.transform.SetParent(column.transform, true); //Setting the parent of this GameObject to the firecolumn, also we're going to keep the position we assigned before



                        /*("column's parent is: " + column.transform.parent.name).Log();
                        "Little GameObject test".Log();
                        @object.name.Log();
                        "Verified - Little GameObject test".Log();

                        "Little Transform Parent test".Log();
                        liquidSource.gameObject.transform.parent.name.Log();
                        "Verified - Little Transform Parent test".Log();*/
                    }
                    else //Else
                    {
                        liquidSource = column.transform.gameObject.GetComponentInChildren<LiquidSource>(); //We're just getting the already present LiquidSource
                    }

                    liquidSource.bounceDamp = 0.8f;
                    liquidSource.floatForcePerDepth = 10;
                    liquidSource.liquidId = Ids.FIRE_LIQUID;

                    /*liquidSource.SetField("model",
                        new LiquidSourceModel()
                        {
                            pos = liquidSource.transform.position,
                            isScaling = false,
                            unitsFilled = 0f,
                        });*/
                    //liquidSource.GetField<LiquidSourceModel>("model").SetField("pos", liquidSource.transform.position);




                    //For each fireball in the column
                    for (int key = 0; key < column.fireballs.Length; ++key)
                    {
                        //We're going to increase the damage it does to the player
                        FireColumn.FireballEntry currentFireBall = column.fireballs[key];
                        DamagePlayerOnTouch damagePlayerOnTouch = currentFireBall.prefab.GetComponent<DamagePlayerOnTouch>();
                        ExplodingFireBall explodingFireBall = currentFireBall.prefab.GetComponent<ExplodingFireBall>();
                        if(explodingFireBall != null)
                        {
                            column.SetField("explodePower", 600f*2);
                            column.SetField("explodeRadius", 7f*2);
                            column.SetField("minPlayerDamage", 15f * 2);
                            column.SetField("maxPlayerDamage", 45f * 2);
                        }

                        if (damagePlayerOnTouch != null)
                            damagePlayerOnTouch.damagePerTouch *= 2;

                        //Also we're going to make them faster
                        currentFireBall.minBallEjectForce *= 1.5f;
                        currentFireBall.maxBallEjectForce *= 1.5f;
                    }

                    //("Adding a new column from the activator " + __instance.name + " to the List").Log();
                    //^^^ SimplePlayer is the FirestormActivator

                    Main.specialColumns.Add(column);
                }

                //zoneDESERT/cellDesert_ScorchedPlainsNorthEast/Sector/FireColumn_ScorchedPlainsNorthEast-disableable

                //column.gameObject.AddComponent<LiquidSource>().GetCopyOf(SRObjects.GetInst<LiquidSource>("zoneDESERT/cellDesert_ScorchedPlainsNorthEast/Sector/desertOasis/waterFountain01/Water Collider").GetComponent<LiquidSource>());
                //GameObject waterFountain = SRObjects.GetInst<GameObject>("zoneREEF/cellReef_Hub/Sector/Resources/waterFountain01", column.gameObject);


                #endregion

                // ----------------------------------------------- \\
                // ----------------------------------------------- \\
                // ----------------------------------------------- \\
                // ----------------------------------------------- \\


                if (fireColumn != null)
                    fireColumn.ActivateFire();
            }
            __instance.GetPrivateField<WorldModel>("worldModel").nextFirecolumnTime = __instance.GetPrivateField<TimeDirector>("timeDir").HoursFromNow(Randoms.SHARED.GetInRange(__instance.minColumnDelayHrs, __instance.maxColumnDelayHrs));
            
            return false;
        }
    }
}
