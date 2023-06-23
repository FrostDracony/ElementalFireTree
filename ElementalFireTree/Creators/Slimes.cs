using SRML.SR;
using SRML.Utils;
using SRML.SR.Translation;
using UnityEngine;
using Console = SRML.Console.Console;
using MonomiPark.SlimeRancher.Regions;
using ElementalFireTree;

namespace Creators
{
    public class Slimes
    {
        public static void RegisterAllSlimes()
        {
            RegisterSlime(ElementalFireSlime.CreateElementalFireSlimePrefab());
        }

        public static void RegisterAllSlimePedia()
        {
            new SlimePediaEntryTranslation(Ids.ELEMENTAL_FIRE_SLIMES)
                .SetTitleTranslation("Elemental Fire Slimes")
                .SetIntroTranslation("Lorem Ipsum")
                .SetSlimeologyTranslation("Lorem Ipsum")
                .SetPlortonomicsTranslation("Lorem Ipsum");

            PediaRegistry.SetPediaCategory(Ids.ELEMENTAL_FIRE_SLIMES, PediaRegistry.PediaCategory.SLIMES);

            RegisterSlimePedia(Ids.ELEMENTAL_FIRE_SLIME);
            TranslationPatcher.AddActorTranslation("l." + Ids.ELEMENTAL_FIRE_SLIME.ToString().ToLower(), "Elemental Fire Slime");
        }

        public static void RegisterSlimePedia(Identifiable.Id slimeId)
        {
            PediaRegistry.RegisterIdentifiableMapping(Ids.ELEMENTAL_FIRE_SLIMES, slimeId);
        }

        public static void RegisterSlime((SlimeDefinition, GameObject) Tuple)
        {

            //Getting the SlimeDefinition and GameObject separated
            SlimeDefinition Slime_Slime_Definition = Tuple.Item1;
            GameObject Slime_Slime_Object = Tuple.Item2;

            //And well, registering it!

            LookupRegistry.RegisterIdentifiablePrefab(Slime_Slime_Object);
            SlimeRegistry.RegisterSlimeDefinition(Slime_Slime_Definition);

            AmmoRegistry.RegisterAmmoPrefab(PlayerState.AmmoMode.DEFAULT, Slime_Slime_Object);

            SlimeDefinition slimeByIdentifiableId = SRSingleton<GameContext>.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.TARR_SLIME);
            slimeByIdentifiableId.Diet.EatMap.Add(new SlimeDiet.EatMapEntry
            {
                eats = Slime_Slime_Definition.IdentifiableId,
                becomesId = Identifiable.Id.NONE,
                driver = SlimeEmotions.Emotion.NONE,
                producesId = Identifiable.Id.NONE
            });

        }

        public static (SlimeDefinition, GameObject) CreateBaseSlime(Identifiable.Id SlimeId, string SlimeName, SlimeEat.FoodGroup[] foodGroup, Identifiable.Id[] produces, Identifiable.Id[] favorites, Sprite icon, SlimeAppearance.Palette spashColor, Color inventoryColor, Identifiable.Id slimeObjectToUse, bool autoRegister)
        {
            SlimeDefinition slimeDefinitionToUse = SRSingleton<GameContext>.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(slimeObjectToUse);

            SlimeDefinition slimeDefinition = (SlimeDefinition)PrefabUtils.DeepCopyObject(slimeDefinitionToUse);
            slimeDefinition.AppearancesDefault = new SlimeAppearance[1];
            slimeDefinition.Diet.Produces = produces;

            slimeDefinition.Diet.MajorFoodGroups = foodGroup;

            slimeDefinition.Diet.AdditionalFoods = new Identifiable.Id[0];

            slimeDefinition.Diet.Favorites = favorites;

            slimeDefinition.Diet.EatMap?.Clear();

            slimeDefinition.CanLargofy = false;
            slimeDefinition.FavoriteToys = new Identifiable.Id[0];
            slimeDefinition.Name = SlimeName;
            slimeDefinition.IdentifiableId = SlimeId;

            GameObject slimeObject = PrefabUtils.CopyPrefab(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(slimeObjectToUse));
            slimeObject.name = SlimeName;
            slimeObject.GetComponent<PlayWithToys>().slimeDefinition = slimeDefinition;
            slimeObject.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = slimeDefinition;
            slimeObject.GetComponent<SlimeEat>().slimeDefinition = slimeDefinition;
            slimeObject.GetComponent<Identifiable>().id = SlimeId;
            if (slimeObject.GetComponent<PinkSlimeFoodTypeTracker>())
                GameObject.Destroy(slimeObject.GetComponent<PinkSlimeFoodTypeTracker>());

            SlimeAppearance slimeAppearance = (SlimeAppearance)PrefabUtils.DeepCopyObject(slimeDefinitionToUse.AppearancesDefault[0]);
            slimeDefinition.AppearancesDefault[0] = slimeAppearance;

            LookupRegistry.RegisterVacEntry(SlimeId, inventoryColor, icon);

            slimeAppearance.Icon = icon;

            slimeAppearance.ColorPalette = spashColor;

            slimeAppearance.ColorPalette.Ammo = inventoryColor;

            slimeObject.GetComponent<SlimeAppearanceApplicator>().Appearance = slimeAppearance;

            return (slimeDefinition, slimeObject);
        }

    }
}
