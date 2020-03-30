using System;
using System.Collections.Generic;
using System.Linq;

using log4net;

using ACE.Database;
using ACE.DatLoader;
using ACE.DatLoader.FileTypes;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.WorldObjects;
using ACE.Server.Managers;

namespace ACE.Server.Factories
{
    public static class PlayerFactory
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum CreateResult
        {
            Success,
            TooManySkillCreditsUsed,
            InvalidSkillRequested,
            FailedToTrainSkill,
            FailedToSpecializeSkill,
        }

        public static CreateResult Create(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, out Player player)
        {
            var heritageGroup = DatManager.PortalDat.CharGen.HeritageGroups[characterCreateInfo.Heritage];

            if (weenieType == WeenieType.Admin)
                player = new Admin(weenie, guid, accountId);
            else if (weenieType == WeenieType.Sentinel)
                player = new Sentinel(weenie, guid, accountId);
            else
                player = new Player(weenie, guid, accountId);

            player.SetProperty(PropertyInt.HeritageGroup, (int)characterCreateInfo.Heritage);
            player.SetProperty(PropertyString.HeritageGroup, heritageGroup.Name);
            player.SetProperty(PropertyInt.Gender, (int)characterCreateInfo.Gender);
            player.SetProperty(PropertyString.Sex, characterCreateInfo.Gender == 1 ? "Male" : "Female");

            //player.SetProperty(PropertyDataId.Icon, cgh.IconImage); // I don't believe this is used anywhere in the client, but it might be used by a future custom launcher

            // pull character data from the dat file
            var sex = heritageGroup.Genders[(int)characterCreateInfo.Gender];

            player.SetProperty(PropertyDataId.MotionTable, sex.MotionTable);
            player.SetProperty(PropertyDataId.SoundTable, sex.SoundTable);
            player.SetProperty(PropertyDataId.PhysicsEffectTable, sex.PhysicsTable);
            player.SetProperty(PropertyDataId.Setup, sex.SetupID);
            player.SetProperty(PropertyDataId.PaletteBase, sex.BasePalette);
            player.SetProperty(PropertyDataId.CombatTable, sex.CombatTable);

            // Check the character scale
            if (sex.Scale != 100u)
                player.SetProperty(PropertyFloat.DefaultScale, (sex.Scale / 100f)); // Scale is stored as a percentage

            // Get the hair first, because we need to know if you're bald, and that's the name of that tune!
            var hairstyle = sex.HairStyleList[Convert.ToInt32(characterCreateInfo.Apperance.HairStyle)];

            // Olthoi and Gear Knights have a "Body Style" instead of a hair style. These styles have multiple model/texture changes, instead of a single head/hairstyle.
            // Storing this value allows us to send the proper appearance ObjDesc
            if (hairstyle.ObjDesc.AnimPartChanges.Count > 1)
                player.SetProperty(PropertyInt.Hairstyle, (int)characterCreateInfo.Apperance.HairStyle);

            // Certain races (Undead, Tumeroks, Others?) have multiple body styles available. This is controlled via the "hair style".
            if (hairstyle.AlternateSetup > 0)
                player.SetProperty(PropertyDataId.Setup, hairstyle.AlternateSetup);

            player.SetProperty(PropertyDataId.EyesTexture, sex.GetEyeTexture(characterCreateInfo.Apperance.Eyes, hairstyle.Bald));
            player.SetProperty(PropertyDataId.DefaultEyesTexture, sex.GetDefaultEyeTexture(characterCreateInfo.Apperance.Eyes, hairstyle.Bald));
            player.SetProperty(PropertyDataId.NoseTexture, sex.GetNoseTexture(characterCreateInfo.Apperance.Nose));
            player.SetProperty(PropertyDataId.DefaultNoseTexture, sex.GetDefaultNoseTexture(characterCreateInfo.Apperance.Nose));
            player.SetProperty(PropertyDataId.MouthTexture, sex.GetMouthTexture(characterCreateInfo.Apperance.Mouth));
            player.SetProperty(PropertyDataId.DefaultMouthTexture, sex.GetDefaultMouthTexture(characterCreateInfo.Apperance.Mouth));
            player.Character.HairTexture = sex.GetHairTexture(characterCreateInfo.Apperance.HairStyle);
            player.Character.DefaultHairTexture = sex.GetDefaultHairTexture(characterCreateInfo.Apperance.HairStyle);
            // HeadObject can be null if we're dealing with GearKnight or Olthoi
            var headObject = sex.GetHeadObject(characterCreateInfo.Apperance.HairStyle);
            if (headObject != null)
                player.SetProperty(PropertyDataId.HeadObject, (uint)headObject);

            // Skin is stored as PaletteSet (list of Palettes), so we need to read in the set to get the specific palette
            var skinPalSet = DatManager.PortalDat.ReadFromDat<PaletteSet>(sex.SkinPalSet);
            player.SetProperty(PropertyDataId.SkinPalette, skinPalSet.GetPaletteID(characterCreateInfo.Apperance.SkinHue));
            player.SetProperty(PropertyFloat.Shade, characterCreateInfo.Apperance.SkinHue);

            // Hair is stored as PaletteSet (list of Palettes), so we need to read in the set to get the specific palette
            var hairPalSet = DatManager.PortalDat.ReadFromDat<PaletteSet>(sex.HairColorList[Convert.ToInt32(characterCreateInfo.Apperance.HairColor)]);
            player.SetProperty(PropertyDataId.HairPalette, hairPalSet.GetPaletteID(characterCreateInfo.Apperance.HairHue));

            // Eye Color
            player.SetProperty(PropertyDataId.EyesPalette, sex.EyeColorList[Convert.ToInt32(characterCreateInfo.Apperance.EyeColor)]);

            string templateName = heritageGroup.Templates[characterCreateInfo.TemplateOption].Name;
            //player.SetProperty(PropertyString.Title, templateName);
            player.SetProperty(PropertyString.Template, templateName);
            player.AddTitle(heritageGroup.Templates[characterCreateInfo.TemplateOption].Title, true);


            // set player attributes to 10/100/10/10/100/100
            player.Strength.StartingValue = 10;
            player.Endurance.StartingValue = 100;
            player.Coordination.StartingValue = 10;
            player.Quickness.StartingValue = 60;
            player.Focus.StartingValue = 100;
            player.Self.StartingValue = 100;

            //unspecialize and untrain all skills
            //retrain/specialize appropriate war mage skills
            player.AvailableSkillCredits = 80;
            player.UnspecializeSkill(Skill.WarMagic, 0);
            player.UnspecializeSkill(Skill.Alchemy, 0);
            player.UnspecializeSkill(Skill.ArcaneLore, 0);
            player.UnspecializeSkill(Skill.ArmorTinkering, 0);
            player.UnspecializeSkill(Skill.AssessCreature, 0);
            player.UnspecializeSkill(Skill.Cooking, 0);
            player.UnspecializeSkill(Skill.CreatureEnchantment, 0);
            player.UnspecializeSkill(Skill.Deception, 0);
            player.UnspecializeSkill(Skill.DirtyFighting, 0);
            player.UnspecializeSkill(Skill.DualWield, 0);
            player.UnspecializeSkill(Skill.FinesseWeapons, 0);
            player.UnspecializeSkill(Skill.Fletching, 0);
            player.UnspecializeSkill(Skill.Healing, 0);
            player.UnspecializeSkill(Skill.HeavyWeapons, 0);
            player.UnspecializeSkill(Skill.ItemEnchantment, 0);
            player.UnspecializeSkill(Skill.ItemTinkering, 0);
            player.UnspecializeSkill(Skill.Jump, 0);
            player.UnspecializeSkill(Skill.Leadership, 0);
            player.UnspecializeSkill(Skill.LifeMagic, 0);
            player.UnspecializeSkill(Skill.LightWeapons, 0);
            player.UnspecializeSkill(Skill.Lockpick, 0);
            player.UnspecializeSkill(Skill.Loyalty, 0);
            player.UnspecializeSkill(Skill.MagicDefense, 0);
            player.UnspecializeSkill(Skill.MagicItemTinkering, 0);
            player.UnspecializeSkill(Skill.ManaConversion, 0);
            player.UnspecializeSkill(Skill.MeleeDefense, 0);
            player.UnspecializeSkill(Skill.MissileDefense, 0);
            player.UnspecializeSkill(Skill.MissileWeapons, 0);
            player.UnspecializeSkill(Skill.Recklessness, 0);
            player.UnspecializeSkill(Skill.Run, 0);
            player.UnspecializeSkill(Skill.Salvaging, 0);
            player.UnspecializeSkill(Skill.Shield, 0);
            player.UnspecializeSkill(Skill.SneakAttack, 0);
            player.UnspecializeSkill(Skill.Summoning, 0);
            player.UnspecializeSkill(Skill.TwoHandedCombat, 0);
            player.UnspecializeSkill(Skill.VoidMagic, 0);
            player.UnspecializeSkill(Skill.WeaponTinkering, 0);

            player.UntrainSkill(Skill.WarMagic, 0);
            player.UntrainSkill(Skill.Alchemy, 0);
            player.UntrainSkill(Skill.ArcaneLore, 0);
            player.UntrainSkill(Skill.ArmorTinkering, 0);
            player.UntrainSkill(Skill.AssessCreature, 0);
            player.UntrainSkill(Skill.Cooking, 0);
            player.UntrainSkill(Skill.CreatureEnchantment, 0);
            player.UntrainSkill(Skill.Deception, 0);
            player.UntrainSkill(Skill.DirtyFighting, 0);
            player.UntrainSkill(Skill.DualWield, 0);
            player.UntrainSkill(Skill.FinesseWeapons, 0);
            player.UntrainSkill(Skill.Fletching, 0);
            player.UntrainSkill(Skill.Healing, 0);
            player.UntrainSkill(Skill.HeavyWeapons, 0);
            player.UntrainSkill(Skill.ItemEnchantment, 0);
            player.UntrainSkill(Skill.ItemTinkering, 0);
            player.UntrainSkill(Skill.Jump, 0);
            player.UntrainSkill(Skill.Leadership, 0);
            player.UntrainSkill(Skill.LifeMagic, 0);
            player.UntrainSkill(Skill.LightWeapons, 0);
            player.UntrainSkill(Skill.Lockpick, 0);
            player.UntrainSkill(Skill.Loyalty, 0);
            player.UntrainSkill(Skill.MagicDefense, 0);
            player.UntrainSkill(Skill.MagicItemTinkering, 0);
            player.UntrainSkill(Skill.ManaConversion, 0);
            player.UntrainSkill(Skill.MeleeDefense, 0);
            player.UntrainSkill(Skill.MissileDefense, 0);
            player.UntrainSkill(Skill.MissileWeapons, 0);
            player.UntrainSkill(Skill.Recklessness, 0);
            player.UntrainSkill(Skill.Run, 0);
            player.UntrainSkill(Skill.Salvaging, 0);
            player.UntrainSkill(Skill.Shield, 0);
            player.UntrainSkill(Skill.SneakAttack, 0);
            player.UntrainSkill(Skill.Summoning, 0);
            player.UntrainSkill(Skill.TwoHandedCombat, 0);
            player.UntrainSkill(Skill.VoidMagic, 0);
            player.UntrainSkill(Skill.WeaponTinkering, 0);

            player.TrainSkill(Skill.ArcaneLore, 0);
            player.TrainSkill(Skill.Jump, 0);
            player.TrainSkill(Skill.MagicDefense, 0);
            player.TrainSkill(Skill.Run, 0);
            player.TrainSkill(Skill.WarMagic, 0);
            player.TrainSkill(Skill.ManaConversion, 0);
            player.TrainSkill(Skill.LifeMagic, 0);

            player.SpecializeSkill(Skill.WarMagic, 0);
            player.SpecializeSkill(Skill.MagicDefense, 0);
            player.SpecializeSkill(Skill.ManaConversion, 0);
            player.SpecializeSkill(Skill.LifeMagic, 0);
            
           
            // data we don't care about
            //characterCreateInfo.CharacterSlot;
            //characterCreateInfo.ClassId;

            // characters start with max vitals
            player.Health.Current = player.Health.Base;
            player.Stamina.Current = player.Stamina.Base;
            player.Mana.Current = player.Mana.Base;


            // Set Heritage based Melee and Ranged Masteries
            GetMasteries(player.HeritageGroup, out WeaponType meleeMastery, out WeaponType rangedMastery);

            player.SetProperty(PropertyInt.MeleeMastery, (int)meleeMastery);
            player.SetProperty(PropertyInt.RangedMastery, (int)rangedMastery);


            SetInnateAugmentations(player);

            AddWeeniesToInventoryX(player, new HashSet<uint> { 15271 , 6799 , 6801 , 1000036 , 25702 , 1000030 , 24207 , 1000031 });

            player.Name = characterCreateInfo.Name;
            player.Character.Name = characterCreateInfo.Name;

            player.Instantiation = new Position(8323335, 260.085388f, -20.421343f, -59.994999f, 0.0f, -0.0f, -0.707107f, -0.707107f);
            player.Sanctuary = new Position(8323335, 260.085388f, -20.421343f, -59.994999f, 0.0f, -0.0f, -0.707107f, -0.707107f);
            player.SetProperty(PropertyBool.RecallsDisabled, false);

            player.AvailableSkillCredits = 0;
            player.AvailableExperience += 191226310247;
            player.TotalExperience += 191226310247;
            player.Level = 275;

            SpendAllXpX(player);

            if (PropertyManager.GetBool("pk_server").Item)
                player.SetProperty(PropertyInt.PlayerKillerStatus, (int)PlayerKillerStatus.PK);
            else if (PropertyManager.GetBool("pkl_server").Item)
                player.SetProperty(PropertyInt.PlayerKillerStatus, (int)PlayerKillerStatus.NPK);

            if ((PropertyManager.GetBool("pk_server").Item || PropertyManager.GetBool("pkl_server").Item) && PropertyManager.GetBool("pk_server_safe_training_academy").Item)
            {
                player.SetProperty(PropertyFloat.MinimumTimeSincePk, -PropertyManager.GetDouble("pk_new_character_grace_period").Item);
                player.SetProperty(PropertyInt.PlayerKillerStatus, (int)PlayerKillerStatus.NPK);
            }

            if (player is Sentinel || player is Admin)
            {
                player.Character.IsPlussed = true;
                player.CloakStatus = CloakStatus.Off;
                player.ChannelsAllowed = player.ChannelsActive;
            }

            CharacterCreateSetDefaultCharacterOptions(player);

            return CreateResult.Success;
        }
       
        private static WorldObject GetClothingObject(uint weenieClassId, uint palette, double shade)
        {
            var weenie = DatabaseManager.World.GetCachedWeenie(weenieClassId);

            if (weenie == null)
                return null;

            var worldObject = (Clothing)WorldObjectFactory.CreateNewWorldObject(weenie);

            worldObject.SetProperties((int)palette, shade);
            
            return worldObject;
        }

        private static void SpendAllXpX(Player player)
        {
            player.SpendAllXp(false);

            player.Health.Current = player.Health.MaxValue;
            player.Stamina.Current = player.Stamina.MaxValue;
            player.Mana.Current = player.Mana.MaxValue;
        }

        private static void AddWeeniesToInventoryX(Player player, IEnumerable<uint> weenieIds, ushort? stackSize = null)
        {
            foreach (uint weenieId in weenieIds)
            {
                var loot = WorldObjectFactory.CreateNewWorldObject(weenieId);

                if (loot == null) // weenie doesn't exist
                    continue;

                if (stackSize == null)
                    stackSize = loot.MaxStackSize;

                if (stackSize > 1)
                    loot.SetStackSize(stackSize);

                // Make sure the item is full of mana
                if (loot.ItemCurMana.HasValue)
                    loot.ItemCurMana = loot.ItemMaxMana;

                player.TryAddToInventory(loot);
            }
        }

        /// <summary>
        /// Set Heritage based Melee and Ranged Masteries
        /// </summary>
        private static void GetMasteries(HeritageGroup heritageGroup, out WeaponType meleeMastery, out WeaponType rangedMastery)
        {
            switch (heritageGroup)
            {
                case HeritageGroup.Aluvian:
                    meleeMastery = WeaponType.Dagger;
                    rangedMastery = WeaponType.Magic;
                    break;
                case HeritageGroup.Gharundim:
                    meleeMastery = WeaponType.Staff;
                    rangedMastery = WeaponType.Magic;
                    break;
                case HeritageGroup.Sho:
                    meleeMastery = WeaponType.Unarmed;
                    rangedMastery = WeaponType.Magic;
                    break;
                case HeritageGroup.Viamontian:
                    meleeMastery = WeaponType.Sword;
                    rangedMastery = WeaponType.Magic;
                    break;
                case HeritageGroup.Penumbraen:
                case HeritageGroup.Shadowbound:
                    meleeMastery = WeaponType.Unarmed;
                    rangedMastery = WeaponType.Magic;
                    break;
                case HeritageGroup.Gearknight:
                    meleeMastery = WeaponType.Mace;
                    rangedMastery = WeaponType.Magic;
                    break;
                case HeritageGroup.Tumerok:
                    meleeMastery = WeaponType.Spear;
                    rangedMastery = WeaponType.Magic;
                    break;
                case HeritageGroup.Undead:
                case HeritageGroup.Lugian:
                    meleeMastery = WeaponType.Axe;
                    rangedMastery = WeaponType.Magic;
                    break;
                case HeritageGroup.Empyrean:
                    meleeMastery = WeaponType.Sword;
                    rangedMastery = WeaponType.Magic;
                    break;
                default:
                    meleeMastery = WeaponType.Undef;
                    rangedMastery = WeaponType.Undef;
                    break;
            }
        }

        private static void SetInnateAugmentations(Player player)
        {
            player.AugmentationJackOfAllTrades = 1;
            player.AugmentationIncreasedSpellDuration = 5;
            player.AugmentationSkilledMagic = 1;
            player.AugmentationCriticalDefense = 1;
            player.AugmentationInfusedCreatureMagic = 1;
            player.AugmentationInfusedItemMagic = 1;
            player.AugmentationInfusedWarMagic = 1;
            player.AugmentationInfusedVoidMagic = 1;
            player.AugmentationInfusedLifeMagic = 1;
            player.AugmentationIncreasedCarryingCapacity = 5;
            player.AugmentationInnateQuickness = 10;
            player.AugmentationInnateFamily = 10;
        }

        public static WorldObject CreateIOU(uint missingWeenieId)
        {
            var iou = (Book)WorldObjectFactory.CreateNewWorldObject("parchment");

            iou.SetProperties("IOU", "An IOU for a missing database object.", "Sorry about that chief...", "ACEmulator", "prewritten");
            iou.AddPage(uint.MaxValue, "ACEmulator", "prewritten", false, $"{missingWeenieId}\n\nSorry but the database does not have a weenie for weenieClassId #{missingWeenieId} so in lieu of that here is an IOU for that item.");
            iou.Bonded = BondedStatus.Bonded;
            iou.Attuned = AttunedStatus.Attuned;
            iou.IsSellable = false;
            iou.Value = 0;
            iou.EncumbranceVal = 0;

            return iou;
        }

        /// <summary>
        /// Checks if the total credits is more than this class is allowed.
        /// </summary>
        /// <returns>The original value or the max allowed.</returns>
        private static ushort ValidateAttributeCredits(uint attributeValue, uint allAttributes, uint maxAttributes)
        {
            attributeValue = Math.Clamp(attributeValue, 10, 100);

            if ((attributeValue + allAttributes) > maxAttributes)
                return (ushort)(maxAttributes - allAttributes);

            return (ushort)attributeValue;
        }
        
        private static void CharacterCreateSetDefaultCharacterOptions(Player player)
        {
            player.SetCharacterOption(CharacterOption.VividTargetingIndicator, true);
            player.SetCharacterOption(CharacterOption.Display3dTooltips, true);
            player.SetCharacterOption(CharacterOption.ShowCoordinatesByTheRadar, true);
            player.SetCharacterOption(CharacterOption.DisplaySpellDurations, true);
            player.SetCharacterOption(CharacterOption.IgnoreFellowshipRequests, true);
            player.SetCharacterOption(CharacterOption.ShareFellowshipExpAndLuminance, true);
            player.SetCharacterOption(CharacterOption.LetOtherPlayersGiveYouItems, true);
            player.SetCharacterOption(CharacterOption.RunAsDefaultMovement, true);
            player.SetCharacterOption(CharacterOption.AutoTarget, true);
            player.SetCharacterOption(CharacterOption.AutoRepeatAttacks, true);
            player.SetCharacterOption(CharacterOption.UseChargeAttack, true);
            player.SetCharacterOption(CharacterOption.LeadMissileTargets, true);
            player.SetCharacterOption(CharacterOption.ListenToAllegianceChat, true);
            player.SetCharacterOption(CharacterOption.ListenToGeneralChat, true);
            player.SetCharacterOption(CharacterOption.ListenToTradeChat, true);
            player.SetCharacterOption(CharacterOption.ListenToLFGChat, true);

            // Not official client defaults, might have been creation defaults however to avoid initial confusion about helm/cloak equipping
            player.SetCharacterOption(CharacterOption.ShowYourHelmOrHeadGear, true);
            player.SetCharacterOption(CharacterOption.ShowYourCloak, true);
        }
    }
}
