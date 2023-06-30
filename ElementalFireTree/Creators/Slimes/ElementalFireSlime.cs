using SRML;
using SRML.SR;
using SRML.Utils;
using UnityEngine;
using ElementalFireTree;
using Console = SRML.Console.Console;
using MonomiPark.SlimeRancher.Regions;

namespace Creators
{
    public class ElementalFireSlime
    {
        public static (SlimeDefinition, GameObject) CreateElementalFireSlimePrefab(bool autoRegister = true)
        {
            //Material slimeFireBaseMat = SRObjects.Get<Material>("slimeFireBase");
            Material material = Object.Instantiate(SRSingleton<GameContext>.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.FIRE_SLIME).AppearancesDefault[0].Structures[0].DefaultMaterials[0]);

            (SlimeDefinition, GameObject) SlimeTuple = Slimes.CreateBaseSlime(
                Ids.ELEMENTAL_FIRE_SLIME,
                "elementalFireSlime",
                new SlimeEat.FoodGroup[1]
                {
                    SlimeEat.FoodGroup.VEGGIES
                },

                new Identifiable.Id[1]
                {
                    Ids.ELEMENTAL_FIRE_PLORT
                },
                new Identifiable.Id[1]
                {
                    Identifiable.Id.GINGER_VEGGIE
                },
                ElementalFireTree.Main.assetBundle.LoadAsset<Sprite>("ElementalFireSlime"),
                /*new SlimeAppearance.Palette
                {
                    Top = Color.red,
                    Middle = Color.red,
                    Bottom = Color.red
                },*/
                SlimeAppearance.Palette.FromMaterial(material),
                Color.red,
                Identifiable.Id.RAD_SLIME,
                autoRegister);
            
            SlimeDefinition slimeDefinition = SlimeTuple.Item1;
            GameObject slimeObject = SlimeTuple.Item2;
            SlimeAppearance slimeAppearance = slimeObject.GetComponent<SlimeAppearanceApplicator>().Appearance;

            SlimeAppearanceStructure[] structures = slimeAppearance.Structures;
            SlimeAppearanceStructure slimeAppearanceStructure = structures[0];

            Material[] defaultMaterials = slimeAppearanceStructure.DefaultMaterials;
            if (defaultMaterials != null && defaultMaterials.Length != 0)
            {
                 slimeAppearanceStructure.DefaultMaterials[0] = material;
            }
            /*int i = 0;
            int i2 = 0;*/
            for (int i = 0; i < structures.Length; i++)
            {
                if(i == 0)
                    continue;
                GameObject prefab = structures[i].Element.Prefabs[0].gameObject;
                prefab.GetComponent<Renderer>().enabled = false;
            }

            //structures[1].Element.Prefabs[0].
            /*foreach (Renderer rdr in structures[1].Element.Prefabs[0].GetComponentsInChildren<Renderer>())
            {
                rdr.enabled = false;
            }*/
            /*foreach (SlimeAppearanceStructure s[limeAppearanceStructure in structures)
            {
                Material[] defaultMaterials = slimeAppearanceStructure.DefaultMaterials;
                if (defaultMaterials != null && defaultMaterials.Length != 0)
                {
                    material.SetColor("_TopColor", new Color32(190, 28, 255, 255));
                    material.SetColor("_MiddleColor", new Color32(159, 28, 255, 255));
                    material.SetColor("_BottomColor", new Color32(120, 28, 255, 255));
                    material.SetColor("_SpecColor", new Color32(205, 28, 255, 255));

                    material.SetFloat(Shader.PropertyToID("_Gloss"), 3.5F);
                    material.SetFloat(Shader.PropertyToID("_GlossPower"), 6F);
                    material.SetFloat(Shader.PropertyToID("_Shininess"), 2F);

                    slimeAppearanceStructure.DefaultMaterials[0] = material;
                }
            }*/

            SlimeExpressionFace[] expressionFaces = slimeAppearance.Face.ExpressionFaces;
            for (int k = 0; k < expressionFaces.Length; k++)
            {
                SlimeExpressionFace slimeExpressionFace = expressionFaces[k];
                if ((bool)slimeExpressionFace.Mouth)
                {
                    slimeExpressionFace.Mouth.SetColor("_MouthBot", new Color32(205, 190, 255, 255));
                    slimeExpressionFace.Mouth.SetColor("_MouthMid", new Color32(182, 170, 226, 255));
                    slimeExpressionFace.Mouth.SetColor("_MouthTop", new Color32(182, 170, 205, 255));
                }
                if ((bool)slimeExpressionFace.Eyes)
                {
                    slimeExpressionFace.Eyes.SetColor("_EyeRed", Color.white); //new Color32(205, 190, 255, 255)
                    slimeExpressionFace.Eyes.SetColor("_EyeGreen", Color.white); //new Color32(182, 170, 226, 255)
                    slimeExpressionFace.Eyes.SetColor("_EyeBlue", Color.white); //new Color32(182, 170, 205, 255)
                }
            }
            slimeAppearance.Face.OnEnable();


            //slimeObject.AddComponent<FireSlimeIgnition>(); ///--------------------------------------------------->>>>>>>>
            //slimeObject.AddComponent<ShootWhenAgitated>();

            slimeObject.AddComponent<DamagePlayerOnTouch>().GetCopyOf(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.ROCK_SLIME).GetComponentInChildren<DamagePlayerOnTouch>());
            slimeObject.GetComponent<DamagePlayerOnTouch>().damagePerTouch = 140;
            slimeObject.AddComponent<ElementalFireSlimeIgnition>();

            //Replace DamagePlayerOnTouch with FireSlimeIgnition asap. Idea: when elemental fire slime gets hit by a liquid fire, it gets boosted up by a bit for a bi

            GameObject particles = ElementalFireTree.Main.assetBundle.LoadAsset<GameObject>("ElementalFireParticles");
            particles.transform.position = new Vector3(0, 0.5F, 0);

            if (Shader.Find("SR/Particles/Additive") != null)
            {
                foreach (ParticleSystemRenderer particleSystemRenderer in particles.GetComponentsInChildren<ParticleSystemRenderer>())
                {
                    particleSystemRenderer.material.shader = Shader.Find("SR/Particles/Additive");
                }
            }

            Object.Instantiate(particles, slimeObject.transform);

            slimeObject.GetComponent<SlimeRandomMove>().scootSpeedFactor *= 3;
            slimeObject.GetComponent<SlimeRandomMove>().verticalFactor *= 3;
            slimeObject.AddComponent<ChangeParticlesAngry>();
            //slimeObject.AddComponent<ChangeParticlesNormal>();

            slimeObject.FindChild("RadSource(Clone)").GetComponent<RadSource>().radPerSecond = 100;
            /*Object.Destroy(slimeObject.FindChild("rad_aura_LOD0(Clone)", true));
            Object.Destroy(slimeObject.FindChild("rad_core_LOD0(Clone)", true));*/
            //Object.Destroy(slimeObject.GetComponent(typeof(PinkSlimeFoodTypeTracker)));

            return (slimeDefinition, slimeObject);
        }
    }
}
