using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SRML.SR;

namespace ElementalFireTree
{
    public class LiquidCreation
    {
        public static GameObject CreateLiquid(Identifiable.Id ID, string name, Identifiable.Id liquidBase, Sprite icon, Color32 vacColor, Material mat)
        {

            string objName = name;

            GameObject Prefab = SRML.Utils.PrefabUtils.CopyPrefab(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(liquidBase));
            Prefab.name = "liquid" + objName;
            Prefab.GetComponent<Identifiable>().id = ID;
            LookupRegistry.RegisterIdentifiablePrefab(Prefab);
            VacItemDefinition vacItemDefinition = VacItemDefinition.CreateVacItemDefinition(ID, vacColor, icon);
            vacItemDefinition.name = "vac" + objName;
            LookupRegistry.RegisterVacEntry(vacItemDefinition);
            AmmoRegistry.RegisterPlayerAmmo(PlayerState.AmmoMode.DEFAULT, ID);

            GameObject sphere = Prefab.FindChild("Sphere");
            MeshRenderer render = sphere.GetComponent<MeshRenderer>();

            render.sharedMaterial = mat;
            sphere.FindChild("FX Water Glops").GetComponent<ParticleSystemRenderer>().material = mat;
            //Prefab.GetComponent<SphereCollider>().isTrigger = true;

            GameObject InFx = SRSingleton<GameContext>.Instance.LookupDirector.GetLiquidIncomingFX(liquidBase);
            GameObject VacFailFx = SRSingleton<GameContext>.Instance.LookupDirector.GetLiquidVacFailFX(liquidBase);
            LiquidDefinition definition = ScriptableObject.CreateInstance<LiquidDefinition>();
            definition.name = objName;
            InFx.FindChild("Water Glops").GetComponent<ParticleSystemRenderer>().material = mat;

            typeof(LiquidDefinition).GetField("id", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(definition, ID);
            typeof(LiquidDefinition).GetField("inFX", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(definition, InFx);
            typeof(LiquidDefinition).GetField("vacFailFX", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(definition, VacFailFx);
            LookupRegistry.RegisterLiquid(definition);

            /*GameObject liquidTrigger = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            liquidTrigger.name = (name+"Trigger");
            liquidTrigger.transform.localScale = Prefab.transform.localScale;
            liquidTrigger.transform.SetParent(Prefab.transform);
            //fSTrigger.GetComponent<Renderer>().enabled = false;*/
            
            /*Rigidbody liquidTriggerRb = Prefab.GetComponent<Rigidbody>();//liquidTrigger.AddComponent<Rigidbody>();
            liquidTriggerRb.useGravity = true;
            liquidTriggerRb.detectCollisions = true;*/

            return Prefab;
        }
        public static Material CreateLiquidMaterial(Identifiable.Id baseId, string name, Texture ramp, float waveFade, float waveSpeed, float waveNoise, float waveHeight, float refractedLightFade, float refractionAmount, Texture dirt, float dirtFade)
        {
            GameObject Prefab = SRML.Utils.PrefabUtils.CopyPrefab(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(baseId));
            GameObject sphere = Prefab.FindChild("Sphere");
            MeshRenderer render = sphere.GetComponent<MeshRenderer>();
            Material mat = UnityEngine.Object.Instantiate(render.sharedMaterial);
            mat.name = name;
            mat.SetTexture("_ColorRamp", ramp);
            mat.SetFloat("_WaveFade", waveFade);
            mat.SetFloat("_WaveSpeed", waveSpeed);
            mat.SetFloat("_WaveNoise", waveNoise);
            mat.SetFloat("_WaveHeight", waveHeight);
            mat.SetFloat("_RefractedLightFade", refractedLightFade);
            mat.SetFloat("_RefractionAmount", refractionAmount);
            mat.SetTexture("_Dirt", dirt);
            mat.SetFloat("_DirtFade", dirtFade);
            return mat;
        }
    }
}