using SRML;
using SRML.SR;
using SRML.Utils;
using SRML.SR.Translation;
using UnityEngine;
using Console = SRML.Console.Console;
using MonomiPark.SlimeRancher.Regions;
using ElementalFireTree;
using System.Linq;

namespace Creators
{
    static class Plorts
    {
        public static void RegisterAllPlorts()
        {
            RegisterPlort(
                CreateElectricPlortPrefab(
                    "elemntalFirePlort",
                    Ids.ELEMENTAL_FIRE_PLORT,
                    Identifiable.Id.FIRE_PLORT),
                SRObjects.Get<Sprite>("iconPlortFire"),
                Color.red,
                850,
                9
            );
        }

        unsafe public static void RegisterPlort(GameObject plort, Sprite icon, Color backGroundColor, int price, int saturation)
        {
            Identifiable.Id id = plort.GetComponent<Identifiable>().id;
            AmmoRegistry.RegisterAmmoPrefab(PlayerState.AmmoMode.DEFAULT, plort);
            // Icon that is below is just a placeholder. You can change it to anything or add your own!	
            //Sprite PlortIcon = ((IEnumerable<PediaDirector.IdEntry>)SRSingleton<SceneContext>.Instance.PediaDirector.entries).First<PediaDirector.IdEntry>((Func<PediaDirector.IdEntry, bool>)(x => x.id == PediaDirector.Id.PLORTS)).icon;
            //Sprite icon = SRSingleton<SceneContext>.Instance.PediaDirector.entries.First((PediaDirector.IdEntry x) => x.id == plortIcon).icon;

            //TranslationPatcher.add

            //Color PureWhite = new Color32(255, 255, 255, byte.MaxValue); // RGB	
            LookupRegistry.RegisterVacEntry(VacItemDefinition.CreateVacItemDefinition(id, backGroundColor, icon));
            AmmoRegistry.RegisterSiloAmmo((x => x == SiloStorage.StorageType.NON_SLIMES || x == SiloStorage.StorageType.PLORT), id);

            //float price = 600f; //Starting price for plort	
            //float saturation = 10f; //Can be anything. The higher it is, the higher the plort price changes every day. I'd recommend making it small so you don't destroy the economy lol.	
            PlortRegistry.AddEconomyEntry(id, price, saturation); //Allows it to be sold while the one below this adds it to plort market.	
            PlortRegistry.AddPlortEntry(id); //PlortRegistry.AddPlortEntry(YourCustomEnum, new ProgressDirector.ProgressType[1]{ProgressDirector.ProgressType.NONE});	
            DroneRegistry.RegisterBasicTarget(id);
            AmmoRegistry.RegisterRefineryResource(id); //For if you want to make a gadget that uses it	
        }

        unsafe public static GameObject CreateElectricPlortPrefab(string name, Identifiable.Id id, Identifiable.Id plortToCopyFrom)
        {
            GameObject Prefab = PrefabUtils.CopyPrefab(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(plortToCopyFrom));
            Prefab.name = name;

            Prefab.GetComponent<Identifiable>().id = id;
            Prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
            Prefab.FindChild("FireQuad").transform.localScale = new Vector3(3, 4.5f, 3);
            LookupRegistry.RegisterIdentifiablePrefab(Prefab);
            TranslationPatcher.AddActorTranslation("l." + Ids.ELEMENTAL_FIRE_PLORT.ToString().ToLower(), "Elemental-Fire-Plort");

            return Prefab;
        }
    }

}
