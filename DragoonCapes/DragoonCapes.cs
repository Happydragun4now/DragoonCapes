using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Skills;
using Logger = Jotunn.Logger;
using Paths = BepInEx.Paths;

namespace DragoonCapes
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class DragoonCapes : BaseUnityPlugin
    {
        public const string PluginGUID = "com.HappyDragoon.DragoonCapes";
        public const string PluginName = "DragoonCapes";
        public const string PluginVersion = "1.3.1";

        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        //Reference this instance of the class as the local instance? Used to reference config values from other classes.
        public static DragoonCapes Instance;
        public DragoonCapes()
        {
            Instance = this;
        }

        private void Awake()
        {
            CreateConfigValues();
            //Clones the vanilla prefabs when they are available
            PrefabManager.OnVanillaPrefabsAvailable += AddClonedItems;

            //Run the Patchs for cape functions etc.
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), (string)null);
        }

        //Declare Configuration Variables (Doesn't seem to be needed for server Configs)
        public ConfigEntry<bool> vanillaCapeEffectsEnabled;
        public ConfigEntry<bool> dragoonCapeEffectsEnabled;
        public ConfigEntry<bool> featherBeltEnabled;

        public ConfigEntry<int> WolfArmor;
        public ConfigEntry<int> WolfArmorPerLevel;
        public ConfigEntry<int> WolfSkill;

        public ConfigEntry<float> DeerMoveSpeed;
        public ConfigEntry<int> DeerSkill;

        public ConfigEntry<float> LoxHealing;
        public ConfigEntry<float> LoxHealth;
        public ConfigEntry<float> LoxHealInterval;

        public ConfigEntry<float> LinenStagger;
        public ConfigEntry<float> LinenDamageMult;
        public ConfigEntry<int> LinenProjs;

        public ConfigEntry<float> TrollDamageMult;

        public ConfigEntry<int> NeckSkill;
        public ConfigEntry<bool> NeckWaterproof;

        public ConfigEntry<int> CultistSkill;
        public ConfigEntry<float> CultistRegen;

        public ConfigEntry<int> DvergrSkill;
        public ConfigEntry<float> DvergrRegen;

        public ConfigEntry<float> SerpentDamageMult;

        public ConfigEntry<int> KnightSkill;
        public ConfigEntry<float> KnightRegen;
        public ConfigEntry<float> KnightStaggerRes;

        public ConfigEntry<int> CrusaderSkill;
        public ConfigEntry<float> CrusaderDamageSpirit;
        public ConfigEntry<float> CrusaderDamageBlunt;

        public ConfigEntry<float> BerserkDamageMult;
        public ConfigEntry<float> BerserkMoveSpeed;

        public ConfigEntry<int> BushSkill;
        public ConfigEntry<float> BushVelocity;
        public ConfigEntry<float> BushNoiseReduction;

        public ConfigEntry<int> BoarSkill;
        public ConfigEntry<float> BoarRegen;
        public ConfigEntry<float> BoarDodgeMult;

        public ConfigEntry<int> GreySkill;
        public ConfigEntry<float> GreyEitr;

        public ConfigEntry<float> WraithPenalty;
        public ConfigEntry<int> WraithFlightTime;
        public ConfigEntry<float> WraithVel;
        public ConfigEntry<float> WraithFall;

        public ConfigEntry<float> EinherjarVelocity;
        public ConfigEntry<float> EinherjarDamageMult;
        public ConfigEntry<bool> EinherjarEffect;
        public ConfigEntry<float> EinherjarCostMult;

        public ConfigEntry<float> SurtlingDamageMult;

        public ConfigEntry<int> HrugnirCarryWeight;
        public ConfigEntry<float> HrugnirDamageMult;

        public ConfigEntry<float> LeechLifesteal;

        public ConfigEntry<float> nightstalkerRegen;
        public ConfigEntry<int> nightstalkerSkill;

        public ConfigEntry<int> AdventurerComfortBonus;
        public ConfigEntry<bool> AdventurerEffect;
        

        private void CreateConfigValues()
        {
            //From JotunnModStub, no idea what it does
            Config.SaveOnConfigSet = true;

            vanillaCapeEffectsEnabled = Config.Bind("Server config", "vanillaCapeEffectsEnabled", true,
            new ConfigDescription("Enable Effects on vanilla capes", null,
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            dragoonCapeEffectsEnabled = Config.Bind("Server config", "dragoonCapeEffectsEnabled", true,
            new ConfigDescription("Enable Effects on modded capes", null,
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            featherBeltEnabled = Config.Bind("Server config", "featherBeltEnabled", true,
            new ConfigDescription("$featherBeltEnabled", null,
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            WolfArmor = Config.Bind("Server config", "WolfArmor", 4,
            new ConfigDescription("Wolf cape base armor.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            WolfArmorPerLevel = Config.Bind("Server config", "WolfArmorPerLevel", 2,
            new ConfigDescription("Wolf cape armor increase per Level", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            WolfSkill = Config.Bind("Server config", "WolfSkill", 10,
            new ConfigDescription("Wolf cape skill bonuses", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            DeerMoveSpeed = Config.Bind("Server config", "DeerMoveSpeed", 0.05f,
            new ConfigDescription("Deer cape nove speed multipler bonus.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeerSkill = Config.Bind("Server config", "DeerSkill", 10,
            new ConfigDescription("Deer cape skill bonuses.", null,
            new AcceptableValueRange<float>(0f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            LoxHealing = Config.Bind("Server config", "LoxHealing", 5f,
            new ConfigDescription("Lox cape heal per tick.", null,
            new AcceptableValueRange<float>(0f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            LoxHealInterval = Config.Bind("Server config", "LoxHealInterval", 10f,
            new ConfigDescription("Lox cape interval between healing.", null,
            new AcceptableValueRange<float>(0.1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            LoxHealth = Config.Bind("Server config", "LoxHealth", 1.15f,
            new ConfigDescription("Lox cape health multiplier.", null,
            new AcceptableValueRange<float>(0.1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            LinenStagger = Config.Bind("Server config", "LinenStagger", 0.25f,
            new ConfigDescription("Linen cape added melee stagger damage, 1 would mean double stagger", null,
            new AcceptableValueRange<float>(0.1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            LinenDamageMult = Config.Bind("Server config", "LinenDamageMult", 0.42f,
            new ConfigDescription("Linen cape projectile damage multiplier, should be lower if proj is higher to balance the damage output.", null,
            new AcceptableValueRange<float>(0.1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            LinenProjs = Config.Bind("Server config", "LinenProjs", 3,
            new ConfigDescription("Linen cape number of projectiles.", null,
            new AcceptableValueRange<int>(1, 10),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            TrollDamageMult = Config.Bind("Server config", "TrollDamageMult", 1.4f,
            new ConfigDescription("Troll cape damage multipler at 100 sneak", null,
            new AcceptableValueRange<float>(1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            NeckSkill = Config.Bind("Server config", "NeckSkill", 30,
            new ConfigDescription("Neck cape skill bonus.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            NeckWaterproof = Config.Bind("Server config", "NeckWaterproof", true,
            new ConfigDescription("Neck cape waterproofness. true=waterproof", null,
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            CultistSkill = Config.Bind("Server config", "CultistSkill", 10,
            new ConfigDescription("Cultist cape skill bonuses.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            CultistRegen = Config.Bind("Server config", "CultistRegen", 1.15f,
            new ConfigDescription("Cultist cape health regen multiplier.", null,
            new AcceptableValueRange<float>(1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            DvergrSkill = Config.Bind("Server config", "DvergrSkill", 10,
            new ConfigDescription("Dvergr cape skill bonuses.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DvergrRegen = Config.Bind("Server config", "DvergrRegen", 1.15f,
            new ConfigDescription("Dvergr cape eitr regen multiplier.", null,
            new AcceptableValueRange<float>(1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SerpentDamageMult = Config.Bind("Server config", "SerpentDamageMult", 0.25f,
            new ConfigDescription("Serpent cape added poison damage.", null,
            new AcceptableValueRange<float>(1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            KnightSkill = Config.Bind("Server config", "KnightSkill", 10,
            new ConfigDescription("Knight cape skill bonuses.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            KnightRegen = Config.Bind("Server config", "KnightRegen", 1.15f,
            new ConfigDescription("Knight cape stamina regen multiplier.", null,
            new AcceptableValueRange<float>(1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            KnightStaggerRes = Config.Bind("Server config", "KnightStaggerRes", 0.25f,
            new ConfigDescription("Knight cape stagger resistance percentage.", null,
            new AcceptableValueRange<float>(0.01f, 0.99f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            CrusaderSkill = Config.Bind("Server config", "CrusaderSkill", 10,
            new ConfigDescription("Crusader cape skill bonuses.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            CrusaderDamageSpirit = Config.Bind("Server config", "CrusaderDamageSpirit", 0.25f,
            new ConfigDescription("Crusader cape added spirit damage.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            CrusaderDamageBlunt = Config.Bind("Server config", "CrusaderDamageBlunt", 0.1f,
            new ConfigDescription("Crusader cape added blunt damage.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BerserkDamageMult = Config.Bind("Server config", "BerserkDamageMult", 0.4f,
            new ConfigDescription("Berserk cape damage multiplier when at lowest health.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BerserkMoveSpeed = Config.Bind("Server config", "BerserkMoveSpeed", 0.13f,
            new ConfigDescription("Berserk cape movement speed modifier.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BushSkill = Config.Bind("Server config", "BushSkill", 20,
            new ConfigDescription("Bush cape skill bonuses.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BushNoiseReduction = Config.Bind("Server config", "BushNoiseReduction", 0.67f,
            new ConfigDescription("Bush cape bow firing noise reduction, 0 is silent.", null,
            new AcceptableValueRange<float>(0.01f, 1f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BushVelocity = Config.Bind("Server config", "BushVelocity", 0.2f,
            new ConfigDescription("Bush cape bow firing velocity multiplier.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BoarSkill = Config.Bind("Server config", "BoarSkill", 10,
            new ConfigDescription("Boar cape skill bonuses.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BoarRegen = Config.Bind("Server config", "BoarRegen", 1.1f,
            new ConfigDescription("Boar cape stamina regen multiplier.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BoarDodgeMult = Config.Bind("Server config", "BoarDodgeMult", 0.3f,
            new ConfigDescription("Boar cape dodge stamina discount.", null,
            new AcceptableValueRange<float>(0.01f, 1f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            GreySkill = Config.Bind("Server config", "GreySkill", 5,
            new ConfigDescription("Greydwarf cape skill bonuses.", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            GreyEitr = Config.Bind("Server config", "GreyEitr", 20f,
            new ConfigDescription("Greydwarf cape flat eitr bonus.", null,
            new AcceptableValueRange<float>(1f, 1000f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            WraithPenalty = Config.Bind("Server config", "WraithRegenPenalty", 0.3f,
            new ConfigDescription("Wraith cape health and stamina regen penalty.", null,
            new AcceptableValueRange<float>(0.01f, 0.9f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            WraithVel = Config.Bind("Server config", "WraithVel", 3f,
            new ConfigDescription("Wraith cape flying speed. Low values will be less glitchy.", null,
            new AcceptableValueRange<float>(1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            WraithFlightTime = Config.Bind("Server config", "WraithFlightTime", 500,
            new ConfigDescription("How many update ticks you can remain flying with the wraith cape. -1 is disabled", null,
            new AcceptableValueRange<int>(-1, 10000),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            WraithFall = Config.Bind("Server config", "WraithFall", 3f,
            new ConfigDescription("Wraith cape fall speed limiter.", null,
            new AcceptableValueRange<float>(1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));


            EinherjarDamageMult = Config.Bind("Server config", "EinherjarDamageMult", 0.08f,
            new ConfigDescription("Einherjar cape added lighting damage.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            EinherjarVelocity = Config.Bind("Server config", "EinherjarVelocity", 1.3f,
            new ConfigDescription("Einherjar cape added spear velocity.", null,
            new AcceptableValueRange<float>(1f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            EinherjarCostMult = Config.Bind("Server config", "EinherjarCostMult", 2f,
            new ConfigDescription("Einherjar cape throwing attack stamina cost multipler. Disabled if effect is false.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            EinherjarEffect = Config.Bind("Server config", "EinherjarEffect", true,
            new ConfigDescription("If infinite throwing spears is enabled.", null,
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SurtlingDamageMult = Config.Bind("Server config", "SurtlingDamageMult", 0.4f,
            new ConfigDescription("Surtling cape added fire damage when burning.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            HrugnirCarryWeight = Config.Bind("Server config", "HrugnirCarryWeight", 35,
            new ConfigDescription("Hrugnir cape added carry weight.", null,
            new AcceptableValueRange<int>(0, 1000),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            HrugnirDamageMult = Config.Bind("Server config", "HrugnirDamageMult", 0.25f,
            new ConfigDescription("Hrugnir cape added unarmed damage while mead is on cooldown.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            LeechLifesteal = Config.Bind("Server config", "LeechLifesteal", 0.05f,
            new ConfigDescription("Leech cape lifesteal rate.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            nightstalkerRegen = Config.Bind("Server config", "nightstalkerRegen", 1.3f,
            new ConfigDescription("Nightstalker cape stamina regen multiplier when cold.", null,
            new AcceptableValueRange<float>(0.01f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            nightstalkerSkill = Config.Bind("Server config", "nightstalkerSkill", 30,
            new ConfigDescription("Nightstalker cape bow skill bonus during night-time", null,
            new AcceptableValueRange<int>(0, 100),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            AdventurerEffect = Config.Bind("Server config", "AdventurerEffect", true,
            new ConfigDescription("Adventurer Cape lets you rest anywhere.", null,
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
            AdventurerComfortBonus = Config.Bind("Server config", "AdventurerComfortBonus", 10,
            new ConfigDescription("Adventurer Cape rested duration bonus in minutes.", null,
            new AcceptableValueRange<int>(0, 1000),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

            // You can subscribe to a global event when config got synced initially and on changes
            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                if (attr.InitialSynchronization)
                {
                    Jotunn.Logger.LogMessage("Initial Config sync event received");
                }
                else
                {
                    Jotunn.Logger.LogMessage("Config sync event received");
                }
            };
        }

        //Declare Status Effect structures for the items
        private static SE_Stats neckCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats cultistCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats knightCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats crusaderCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats dvergrCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats shamanCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats serpentCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats berserkCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats bushCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats boarCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats dwarfCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats wraithCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats surtlingCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats einherjarCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats brawlerCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats leechCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats stalkerCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats adventurerCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats featherBeltStatus = ScriptableObject.CreateInstance<SE_Stats>();
        private static SE_Stats meadBeltStatus = ScriptableObject.CreateInstance<SE_Stats>();//archived

        //adding the items
        private void AddClonedItems()
        {
            //To-Do Ideas:

            //Change Shaman/Bush cape feather trail effect? (Custom unity model?)
            
            //Midas cape that makes every enemy drop a tiny bit of gold?

            //Localization
            
            //update Bepinex, valheim references https://jotunnlib.github.io/jotunnlib/tutorials/getting-started.html

            //variable paths that dont rely on the folder being named properly
            //var CultistIconPath = Path.Combine(Path.GetDirectoryName(Info.Location), "cultistIcon.png");
            //Jotunn.Logger.LogMessage("Cultist Path: " + CultistIconPath);//message,debug or info?
            //Sprite CultistIcon = AssetUtils.LoadSpriteFromFile(CultistIconPath);

            //Cape Ideas
            //ashlands capes? White ash lox cape?
            //Rabbit cape, jump twice as high using stalker result method tweaking the jump skill factor
            //demister cape, carry weight cape to provide alternatives for utility slot.

            //Read the Server Config to check if status effects are enabled
            bool statuses = (bool)Config["Server config", "dragoonCapeEffectsEnabled"].BoxedValue;
            Logger.LogWarning("Mod Statuses Enabled: " + statuses);

            //Neck Cape-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //Neck Status Effect
            neckCapeStatus.name = "$status_neckcape";
            neckCapeStatus.m_name = "$status_neckcape";
            neckCapeStatus.m_tooltip = "$status_neckcape_tooltip";
            neckCapeStatus.m_startMessage = "$status_neckcape_startmessage";
            neckCapeStatus.m_skillLevel = Skills.SkillType.Swim;
            neckCapeStatus.m_skillLevelModifier = NeckSkill.Value;
            neckCapeStatus.m_category = "NeckCape";

            //Neck Item Config
            ItemConfig neckCapeConf = new ItemConfig();
            neckCapeConf.Name = "$item_neckcape";
            neckCapeConf.Description = "$item_neckcape_description";
            neckCapeConf.CraftingStation = "forge";
            neckCapeConf.AddRequirement(new RequirementConfig("TrophyNeck", 5, 2));//the 2nd number is upgrade cost per level
            neckCapeConf.AddRequirement(new RequirementConfig("Guck", 8, 3));
            neckCapeConf.AddRequirement(new RequirementConfig("Iron", 1, 1));

            //Sprite and Texture loading
            Sprite NeckIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/neckIcon.png");
            Texture2D NeckTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/neckTexture.png");
            neckCapeConf.Icons = new Sprite[] { NeckIcon };
            neckCapeConf.StyleTex = NeckTex;

            //Make the item as a deer cape mock and assign status effect
            CustomItem NeckCape = new CustomItem("CapeNeck", "CapeDeerHide", neckCapeConf);

            //assign status effects if the config has it enabled
            if (dragoonCapeEffectsEnabled.Value && NeckWaterproof.Value)
            {
                NeckCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = neckCapeStatus;
            }
            //add Poison Resist
            HitData.DamageModPair pRes = default(HitData.DamageModPair);
            pRes.m_modifier = HitData.DamageModifier.Resistant;
            pRes.m_type = HitData.DamageType.Poison;
            NeckCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Add(pRes);

            ItemManager.Instance.AddItem(NeckCape);

            //Cultist Cape-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //Cultist Status Effect
            cultistCapeStatus.name = "$status_cultistcape";
            cultistCapeStatus.m_name = "$status_cultistcape";
            cultistCapeStatus.m_tooltip = "$status_cultistcape_tooltip";
            cultistCapeStatus.m_startMessage = "$status_cultistcape_startmessage";
            cultistCapeStatus.m_skillLevel = Skills.SkillType.BloodMagic;
            cultistCapeStatus.m_skillLevel2 = Skills.SkillType.Axes;
            cultistCapeStatus.m_skillLevelModifier = cultistCapeStatus.m_skillLevelModifier2 = CultistSkill.Value;
            cultistCapeStatus.m_healthRegenMultiplier = CultistRegen.Value;

            //Cultist cape item config
            ItemConfig cultistCapeConf = new ItemConfig();
            cultistCapeConf.Name = "$item_cultistcape";
            cultistCapeConf.Description = "$item_cultistcape_description";
            cultistCapeConf.CraftingStation = "forge";
            cultistCapeConf.AddRequirement(new RequirementConfig("JuteRed", 10, 4));
            cultistCapeConf.AddRequirement(new RequirementConfig("TrophyFenring", 3, 0));
            cultistCapeConf.AddRequirement(new RequirementConfig("Iron", 1, 1));

            //Sprite and Texture loading
            Sprite CultistIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/cultistIcon.png");
            Texture2D CultistTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/cultistTexture.png");
            cultistCapeConf.Icons = new Sprite[] { CultistIcon };
            cultistCapeConf.StyleTex = CultistTex;

            //Make the item as a lox cape mock and assign status effect
            CustomItem CultistCape = new CustomItem("CapeCultist", "CapeLox", cultistCapeConf);
            if (dragoonCapeEffectsEnabled.Value)
            {
                CultistCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = cultistCapeStatus;
            }
            ItemManager.Instance.AddItem(CultistCape);

            //Dvergr cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //dvergr status effect
            dvergrCapeStatus.name = "$status_dvergrcape";
            dvergrCapeStatus.m_name = "$status_dvergrcape";
            dvergrCapeStatus.m_tooltip = "$status_dvergrcape_tooltip";
            dvergrCapeStatus.m_startMessage = "$status_dvergrcape_startmessage";
            dvergrCapeStatus.m_skillLevel = Skills.SkillType.ElementalMagic;
            dvergrCapeStatus.m_skillLevelModifier = 10f;
            dvergrCapeStatus.m_skillLevel2 = Skills.SkillType.Pickaxes;
            dvergrCapeStatus.m_skillLevelModifier2 = 10f;
            dvergrCapeStatus.m_eitrRegenMultiplier = 1.1f;
            dvergrCapeStatus.m_category = "dvergrCape";

            //dvergr item config
            ItemConfig dvergrCapeConf = new ItemConfig();
            dvergrCapeConf.Name = "$item_dvergrcape";
            dvergrCapeConf.Description = "$item_dvergrcape_description";
            dvergrCapeConf.CraftingStation = "piece_magetable";
            dvergrCapeConf.AddRequirement(new RequirementConfig("JuteBlue", 10, 4));
            dvergrCapeConf.AddRequirement(new RequirementConfig("TrophyDvergr", 3, 0));
            dvergrCapeConf.AddRequirement(new RequirementConfig("BlackMetal", 1, 1));

            //Sprite and Texture loading
            Sprite dvergrIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/dvergrIcon.png");
            Texture2D dvergrTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/dvergrTexture.png");
            dvergrCapeConf.Icons = new Sprite[] { dvergrIcon };
            dvergrCapeConf.StyleTex = dvergrTex;

            //Make the item as a lox cape mock and assign status effect
            CustomItem dvergrCape = new CustomItem("CapeDvergr", "CapeLox", dvergrCapeConf);
            if (dragoonCapeEffectsEnabled.Value)
            {
                dvergrCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = dvergrCapeStatus;
            }

            //Fire Res instead of frost
            dvergrCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Clear();
            HitData.DamageModPair fireRes = default(HitData.DamageModPair);
            fireRes.m_modifier = HitData.DamageModifier.Resistant;
            fireRes.m_type = HitData.DamageType.Fire;
            dvergrCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Add(fireRes);

            ItemManager.Instance.AddItem(dvergrCape);

            //Shaman cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //shaman status effect
            shamanCapeStatus.name = "$status_shamancape";
            shamanCapeStatus.m_name = "$status_shamancape";
            shamanCapeStatus.m_tooltip = "$status_shamancape_tooltip\n<color=orange>$status_shamancape_tooltip1</color>";
            shamanCapeStatus.m_startMessage = "$status_shamancape_startmessage";
            //shamanCapeStatus.m_healthRegenMultiplier = 1.05f;//Disabled for balance reasons
            shamanCapeStatus.m_category = "ShamanCape";

            //shaman item config
            ItemConfig shamanCapeConf = new ItemConfig();
            shamanCapeConf.Name = "$item_shamancape";
            shamanCapeConf.Description = "$item_shamancape_description";
            shamanCapeConf.CraftingStation = "piece_artisanstation";
            shamanCapeConf.MinStationLevel = -3;
            shamanCapeConf.AddRequirement(new RequirementConfig("Feathers", 10, 4));
            shamanCapeConf.AddRequirement(new RequirementConfig("TrophyGoblinShaman", 3, 0));
            shamanCapeConf.AddRequirement(new RequirementConfig("BlackMetal", 1, 1));

            //Sprite and Texture loading
            Sprite shamanIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/shamanIcon.png");
            Texture2D shamanTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/shamanTexture.png");
            shamanCapeConf.Icons = new Sprite[] { shamanIcon };
            shamanCapeConf.StyleTex = shamanTex;

            //Make the item as a feather cape mock and assign status effect
            CustomItem shamanCape = new CustomItem("CapeShaman", "CapeFeather", shamanCapeConf);
            if (dragoonCapeEffectsEnabled.Value)
            {
                shamanCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = shamanCapeStatus;
            }
            shamanCape.ItemDrop.m_itemData.m_shared.m_trailStartEffect = null; // This doesn't disable the trail?

            ItemManager.Instance.AddItem(shamanCape);

            //Serpent cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //serpent status effect
            serpentCapeStatus.name = "$status_serpentcape";
            serpentCapeStatus.m_name = "$status_serpentcape";
            serpentCapeStatus.m_tooltip = "$status_serpentcape_tooltip\n$status_serpentcape_tooltip1<color=orange>+" + SerpentDamageMult.Value * 100 + "%</color>";
            serpentCapeStatus.m_startMessage = "$status_serpentcape_startmessage";
            serpentCapeStatus.m_category = "SerpentCape";

            //serpent item config
            ItemConfig serpentCapeConf = new ItemConfig();
            serpentCapeConf.Name = "$item_serpentcape";
            serpentCapeConf.Description = "$item_serpentcape_description";
            serpentCapeConf.CraftingStation = "piece_workbench";
            serpentCapeConf.AddRequirement(new RequirementConfig("SerpentScale", 10, 4));
            serpentCapeConf.AddRequirement(new RequirementConfig("TrophySerpent", 1, 0));
            serpentCapeConf.AddRequirement(new RequirementConfig("Chitin", 3, 2));

            //Sprite and Texture loading
            Sprite serpentIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/serpentIcon.png");
            Texture2D serpentTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/serpentTexture.png");
            serpentCapeConf.Icons = new Sprite[] { serpentIcon };
            serpentCapeConf.StyleTex = serpentTex;

            //Make the item as a feather cape mock and assign status effect
            CustomItem serpentCape = new CustomItem("CapeSerpent", "CapeWolf", serpentCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                serpentCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = serpentCapeStatus;
            }

            //Poison Resistance instead of frost
            serpentCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Clear();
            HitData.DamageModPair poisonRes = default(HitData.DamageModPair);
            poisonRes.m_modifier = HitData.DamageModifier.Resistant;
            poisonRes.m_type = HitData.DamageType.Poison;
            serpentCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Add(poisonRes);

            ItemManager.Instance.AddItem(serpentCape);

            //Knight Cape-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //knight Status Effect
            knightCapeStatus.name = "$status_knightcape";
            knightCapeStatus.m_name = "$status_knightcape";
            knightCapeStatus.m_tooltip = "$status_knightcape_tooltip\n$status_knightcape_tooltip1<color=orange>+" + KnightStaggerRes.Value * 100 + "%</color>";
            knightCapeStatus.m_startMessage = "$status_knightcape_startmessage";
            knightCapeStatus.m_skillLevel = Skills.SkillType.Blocking;
            knightCapeStatus.m_skillLevel2 = Skills.SkillType.Swords;
            knightCapeStatus.m_skillLevelModifier = knightCapeStatus.m_skillLevelModifier2 = KnightSkill.Value;
            knightCapeStatus.m_staminaRegenMultiplier = KnightRegen.Value;
            knightCapeStatus.m_category = "knightCape";

            //knight Item Config
            ItemConfig knightCapeConf = new ItemConfig();
            knightCapeConf.Name = "$item_knightcape";
            knightCapeConf.Description = "$item_knightcape_description";
            knightCapeConf.CraftingStation = "piece_artisanstation";
            knightCapeConf.MinStationLevel = -3;
            knightCapeConf.AddRequirement(new RequirementConfig("CapeLinen", 1, 0));//the 2nd number is upgrade cost per level
            knightCapeConf.AddRequirement(new RequirementConfig("LinenThread", 0, 4));
            knightCapeConf.AddRequirement(new RequirementConfig("Raspberry", 1, 1));
            knightCapeConf.AddRequirement(new RequirementConfig("Blueberries", 1, 1));
            knightCapeConf.AddRequirement(new RequirementConfig("Cloudberry", 1, 1));

            //Sprite and Texture loading, variants are used
            Sprite knightIcon1 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/knightIcon1.png");
            Sprite knightIcon2 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/knightIcon2.png");
            Sprite knightIcon3 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/knightIcon3.png");
            Sprite knightIcon4 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/knightIcon4.png");
            Sprite knightIcon5 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/knightIcon5.png");
            Sprite knightIcon6 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/knightIcon6.png");
            Texture2D knightTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/knightTexture.png");
            knightCapeConf.Icons = new Sprite[] { knightIcon1, knightIcon2, knightIcon3, knightIcon4, knightIcon5, knightIcon6 };
            knightCapeConf.StyleTex = knightTex;

            //Make the item as a linen cape mock and assign status effect
            CustomItem knightCape = new CustomItem("CapeKnight", "CapeLinen", knightCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                knightCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = knightCapeStatus;
            }

            ItemManager.Instance.AddItem(knightCape);

            //Crusader Cape-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //crusader Status Effect
            crusaderCapeStatus.name = "$status_crusadercape";
            crusaderCapeStatus.m_name = "$status_crusadercape";
            crusaderCapeStatus.m_tooltip = "$status_crusadercape_tooltip\n$status_crusadercape_tooltip1<color=orange>+" + CrusaderDamageSpirit.Value * 100f + "%</color>\n$status_crusadercape_tooltip2<color=orange>+" + CrusaderDamageBlunt.Value * 100f + "%</color>";
            crusaderCapeStatus.m_startMessage = "$status_crusadercape_startmessage";
            crusaderCapeStatus.m_skillLevel = Skills.SkillType.Clubs;
            crusaderCapeStatus.m_skillLevel2 = Skills.SkillType.Run;
            crusaderCapeStatus.m_skillLevelModifier = crusaderCapeStatus.m_skillLevelModifier2 = CrusaderSkill.Value;
            crusaderCapeStatus.m_category = "crusaderCape";

            //crusader Item Config
            ItemConfig crusaderCapeConf = new ItemConfig();
            crusaderCapeConf.Name = "$item_crusadercape";
            crusaderCapeConf.Description = "$item_crusadercape_description";
            crusaderCapeConf.CraftingStation = "piece_artisanstation";
            crusaderCapeConf.MinStationLevel = -3;
            crusaderCapeConf.AddRequirement(new RequirementConfig("CapeLinen", 1, 0));//the 2nd number is upgrade cost per level
            crusaderCapeConf.AddRequirement(new RequirementConfig("LinenThread", 0, 4));
            crusaderCapeConf.AddRequirement(new RequirementConfig("Raspberry", 10, 5));
            crusaderCapeConf.AddRequirement(new RequirementConfig("Silver", 10, 5));

            //Sprite and Texture loading, variants are used
            Sprite crusaderIcon1 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/crusaderIcon1.png");
            Sprite crusaderIcon2 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/crusaderIcon2.png");
            Sprite crusaderIcon3 = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/crusaderIcon3.png");
            Texture2D crusaderTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/crusaderTexture.png");
            crusaderCapeConf.Icons = new Sprite[] { crusaderIcon1, crusaderIcon2, crusaderIcon3 };
            crusaderCapeConf.StyleTex = crusaderTex;

            //Make the item as a linen cape mock and assign status effect
            CustomItem crusaderCape = new CustomItem("CapeCrusader", "CapeLinen", crusaderCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                crusaderCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = crusaderCapeStatus;
            }

            ItemManager.Instance.AddItem(crusaderCape);

            //Berserk Cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //berserk status effect
            berserkCapeStatus.name = "$status_berserkcape";
            berserkCapeStatus.m_name = "$status_berserkcape";
            berserkCapeStatus.m_tooltip = "$status_berserkcape_tooltip\n$status_berserkcape_tooltip1<color=orange>+" + BerserkDamageMult.Value * 100f + "%</color>";
            berserkCapeStatus.m_startMessage = "$status_berserkcape_startmessage";
            berserkCapeStatus.m_category = "berserkCape";
            berserkCapeStatus.m_speedModifier = BerserkMoveSpeed.Value;

            //berserk item config
            ItemConfig berserkCapeConf = new ItemConfig();
            berserkCapeConf.Name = "$item_berserkcape";
            berserkCapeConf.Description = "$item_berserkcape_description";
            berserkCapeConf.CraftingStation = "piece_workbench";
            berserkCapeConf.AddRequirement(new RequirementConfig("WolfHairBundle", 10, 3));
            berserkCapeConf.AddRequirement(new RequirementConfig("Mushroom", 20, 4));
            berserkCapeConf.AddRequirement(new RequirementConfig("Obsidian", 10, 3));

            //Sprite and Texture loading
            Sprite berserkIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/berserkIcon.png");
            Texture2D berserkTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/berserkTexture.png");
            berserkCapeConf.Icons = new Sprite[] { berserkIcon };
            berserkCapeConf.StyleTex = berserkTex;

            //Make the item as a feather cape mock and assign status effect
            CustomItem berserkCape = new CustomItem("CapeBerserker", "CapeWolf", berserkCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                berserkCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = berserkCapeStatus;
            }

            //fire res + cold, phys weak
            HitData.DamageModPair pVul = default(HitData.DamageModPair);
            pVul.m_modifier = HitData.DamageModifier.Weak;
            pVul.m_type = HitData.DamageType.Pierce;
            berserkCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Add(pVul);
            HitData.DamageModPair sVul = default(HitData.DamageModPair);
            sVul.m_modifier = HitData.DamageModifier.Weak;
            sVul.m_type = HitData.DamageType.Slash;
            berserkCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Add(sVul);
            HitData.DamageModPair bVul = default(HitData.DamageModPair);
            bVul.m_modifier = HitData.DamageModifier.Weak;
            bVul.m_type = HitData.DamageType.Blunt;
            berserkCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Add(bVul);

            ItemManager.Instance.AddItem(berserkCape);

            //Bush Cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //bush status effect
            bushCapeStatus.name = "$status_bushcape";
            bushCapeStatus.m_name = "$status_bushcape";
            bushCapeStatus.m_tooltip = "$status_bushcape_tooltip\n$status_bushcape_tooltip1<color=orange>+" + (1f - BushNoiseReduction.Value) * 100f + "%</color>\n$status_bushcape_tooltip2<color=orange>+" + BushVelocity.Value * 100f + "%</color>";
            bushCapeStatus.m_startMessage = "$status_bushcape_startmessage";
            bushCapeStatus.m_category = "bushCape";
            bushCapeStatus.m_skillLevel = SkillType.Sneak;
            bushCapeStatus.m_skillLevelModifier = 20f;

            //bush item config
            ItemConfig bushCapeConf = new ItemConfig();
            bushCapeConf.Name = "$item_bushcape";
            bushCapeConf.Description = "$item_bushcape_description";
            bushCapeConf.CraftingStation = "piece_workbench";
            bushCapeConf.AddRequirement(new RequirementConfig("Wood", 30, 5));
            bushCapeConf.AddRequirement(new RequirementConfig("LeatherScraps", 5, 2));
            bushCapeConf.AddRequirement(new RequirementConfig("Flint", 3, 1));

            //Sprite and Texture loading
            Sprite bushIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/bushIcon.png");
            Texture2D bushTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/bushTexture.png");
            bushCapeConf.Icons = new Sprite[] { bushIcon };
            bushCapeConf.StyleTex = bushTex;

            //Make the item as a feather cape mock and assign status effect
            CustomItem bushCape = new CustomItem("CapeBush", "CapeFeather", bushCapeConf);
            if (dragoonCapeEffectsEnabled.Value)
            {
                bushCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = bushCapeStatus;
            }
            //fire vuln, no cold res
            bushCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Clear();
            HitData.DamageModPair fVul = default(HitData.DamageModPair);
            fVul.m_modifier = HitData.DamageModifier.Weak;
            fVul.m_type = HitData.DamageType.Fire;
            bushCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Add(fVul);

            ItemManager.Instance.AddItem(bushCape);

            //Boar Cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //boar status effect
            boarCapeStatus.name = "$status_boarcape";
            boarCapeStatus.m_name = "$status_boarcape";
            boarCapeStatus.m_tooltip = "$status_boarcape_tooltip\n$status_boarcape_tooltip1<color=orange>" + BoarDodgeMult.Value * 100f + "%</color>";
            boarCapeStatus.m_startMessage = "$status_boarcape_startmessage";
            //boarCapeStatus.m_healthRegenMultiplier = 1.05f;
            boarCapeStatus.m_staminaRegenMultiplier = BoarRegen.Value;
            boarCapeStatus.m_skillLevel = SkillType.Clubs;
            boarCapeStatus.m_skillLevelModifier = BoarSkill.Value;
            boarCapeStatus.m_category = "boarCape";

            //boar item config
            ItemConfig boarCapeConf = new ItemConfig();
            boarCapeConf.Name = "$item_boarcape";
            boarCapeConf.Description = "$item_boarcape_description";
            boarCapeConf.CraftingStation = "piece_workbench";
            boarCapeConf.AddRequirement(new RequirementConfig("LeatherScraps", 10, 4));
            boarCapeConf.AddRequirement(new RequirementConfig("TrophyBoar", 3, 0));
            boarCapeConf.AddRequirement(new RequirementConfig("Flint", 1, 1));

            //Sprite and Texture loading
            Sprite boarIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/boarIcon.png");
            Texture2D boarTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/boarTexture.png");
            boarCapeConf.Icons = new Sprite[] { boarIcon };
            boarCapeConf.StyleTex = boarTex;

            //Make the item as a feather cape mock and assign status effect
            CustomItem boarCape = new CustomItem("CapeBoar", "CapeDeerHide", boarCapeConf);
            if (dragoonCapeEffectsEnabled.Value)
            {
                boarCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = boarCapeStatus;
            }
            ItemManager.Instance.AddItem(boarCape);

            //Greydwarf Cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //greydwarf status effect
            dwarfCapeStatus.name = "$status_dwarfcape";
            dwarfCapeStatus.m_name = "$status_dwarfcape";
            dwarfCapeStatus.m_tooltip = "$status_dwarfcape_tooltip\n$status_dwarfcape_tooltip1<color=orange>+" + GreyEitr.Value + "</color>";
            dwarfCapeStatus.m_startMessage = "$status_dwarfcape_startmessage";
            dwarfCapeStatus.m_category = "dwarfCape";
            dwarfCapeStatus.m_skillLevel = SkillType.ElementalMagic;
            dwarfCapeStatus.m_skillLevel2 = SkillType.BloodMagic;
            dwarfCapeStatus.m_skillLevelModifier = dwarfCapeStatus.m_skillLevelModifier2 = GreySkill.Value;

            //greydwarf item config
            ItemConfig dwarfCapeConf = new ItemConfig();
            dwarfCapeConf.Name = "$item_dwarfcape";
            dwarfCapeConf.Description = "$item_dwarfcape_description";
            dwarfCapeConf.CraftingStation = "piece_workbench";
            dwarfCapeConf.AddRequirement(new RequirementConfig("GreydwarfEye", 15, 5));
            dwarfCapeConf.AddRequirement(new RequirementConfig("TrophyGreydwarfShaman", 1, 0));
            dwarfCapeConf.AddRequirement(new RequirementConfig("AncientSeed", 2, 1));

            //Sprite and Texture loading
            Sprite dwarfIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/dwarfIcon.png");
            Texture2D dwarfTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/dwarfTexture.png");
            dwarfCapeConf.Icons = new Sprite[] { dwarfIcon };
            dwarfCapeConf.StyleTex = dwarfTex;

            //Make the item as a feather cape mock and assign status effect
            CustomItem dwarfCape = new CustomItem("CapeGreydwarf", "CapeOdin", dwarfCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                dwarfCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = dwarfCapeStatus;
            }

            dwarfCape.ItemDrop.m_itemData.m_shared.m_dlc = "";

            ItemManager.Instance.AddItem(dwarfCape);

            //Wraith Cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //wraith status effect
            wraithCapeStatus.name = "$status_wraithcape";
            wraithCapeStatus.m_name = "$status_wraithcape";
            wraithCapeStatus.m_tooltip = "$status_wraithcape_tooltip\n$status_wraithcape_tooltip1<color=orange>-" + WraithPenalty.Value * 100f + "%</color>\n<color=orange>$status_wraithcape_tooltip2</color>";
            wraithCapeStatus.m_startMessage = "$status_wraithcape_startmessage";
            wraithCapeStatus.m_category = "wraithCape";
            wraithCapeStatus.m_fallDamageModifier = -2f; // No Fall Damage!
            wraithCapeStatus.m_maxMaxFallSpeed = WraithFall.Value;

            //wraith item config
            ItemConfig wraithCapeConf = new ItemConfig();
            wraithCapeConf.Name = "$item_wraithcape";
            wraithCapeConf.Description = "$item_wraithcape_desc";
            wraithCapeConf.CraftingStation = "forge";
            wraithCapeConf.AddRequirement(new RequirementConfig("Entrails", 15, 5));
            wraithCapeConf.AddRequirement(new RequirementConfig("TrophyWraith", 1, 0));
            wraithCapeConf.AddRequirement(new RequirementConfig("Chain", 4, 2));

            //Sprite and Texture loading
            Sprite wraithIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/wraithIcon.png");
            Texture2D wraithTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/wraithTexture.png");
            wraithCapeConf.Icons = new Sprite[] { wraithIcon };
            wraithCapeConf.StyleTex = wraithTex;

            //Make the item as a feather cape mock and assign status effect
            CustomItem wraithCape = new CustomItem("CapeWraith", "CapeOdin", wraithCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                wraithCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = wraithCapeStatus;
            }

            wraithCape.ItemDrop.m_itemData.m_shared.m_dlc = "";

            ItemManager.Instance.AddItem(wraithCape);

            //Einherjar Cape---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //einherjar status effect
            einherjarCapeStatus.name = "$status_einherjarcape";
            einherjarCapeStatus.m_name = "$status_einherjarcape";
            einherjarCapeStatus.m_startMessage = "$status_einherjarcape_startmessage";
            einherjarCapeStatus.m_category = "einherjarCape";

            if (EinherjarEffect.Value)
            {
                einherjarCapeStatus.m_tooltip = "$status_einherjarcape_tooltip\n$status_einherjarcape_tooltip1<color=orange>+" + EinherjarDamageMult.Value * 100f + "%</color>\n$status_einherjarcape_tooltip2<color=orange>+" + (EinherjarVelocity.Value - 1f) * 100f + "%</color>\n$status_einherjarcape_tooltip3<color=orange>" + (EinherjarCostMult.Value) * 100f + "%</color>\n<color=orange>$status_einherjarcape_tooltip4</color>";
            }
            else
            {
                einherjarCapeStatus.m_tooltip = "$status_einherjarcape_tooltip\n$status_einherjarcape_tooltip1<color=orange>+" + EinherjarDamageMult.Value * 100f + "%</color>\n$status_einherjarcape_tooltip2<color=orange>+" + (EinherjarVelocity.Value - 1f) * 100f + "%</color>";
            }

            //einherjar item config
            ItemConfig einherjarCapeConf = new ItemConfig();
            einherjarCapeConf.Name = "$item_einherjarcape";
            einherjarCapeConf.Description = "$item_einherjarcape_desc";
            einherjarCapeConf.CraftingStation = "piece_workbench";
            einherjarCapeConf.AddRequirement(new RequirementConfig("MushroomYellow", 10, 5));
            einherjarCapeConf.AddRequirement(new RequirementConfig("Blueberries", 10, 5));
            einherjarCapeConf.AddRequirement(new RequirementConfig("Coal", 6, 3));
            einherjarCapeConf.AddRequirement(new RequirementConfig("LeatherScraps", 10, 2));

            //Sprite and Texture loading
            Sprite einherjarIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/einherjarIcon.png");
            Texture2D einherjarTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/einherjarTexture.png");
            einherjarCapeConf.Icons = new Sprite[] { einherjarIcon };
            einherjarCapeConf.StyleTex = einherjarTex;

            //Make the item as a feather cape mock and assign status effect
            CustomItem einherjarCape = new CustomItem("CapeEinherjar", "CapeOdin", einherjarCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                einherjarCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = einherjarCapeStatus;
            }

            einherjarCape.ItemDrop.m_itemData.m_shared.m_dlc = "";

            ItemManager.Instance.AddItem(einherjarCape);

            //Surtling Cape-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            surtlingCapeStatus.name = "$status_surtlingcape";
            surtlingCapeStatus.m_name = "$status_surtlingcape";
            surtlingCapeStatus.m_category = "surtlingCape";
            surtlingCapeStatus.m_tooltip = "$status_surtlingcape_tooltip\n<color=orange>$status_surtlingcape_tooltip1</color>\n$status_surtlingcape_tooltip2<color=orange>+" + SurtlingDamageMult.Value * 100f + "%</color>";
            surtlingCapeStatus.m_startMessage = "$status_surtlingcape_startmessage";

            ItemConfig surtlingCapeConf = new ItemConfig();
            surtlingCapeConf.Name = "$item_surtlingcape";
            surtlingCapeConf.Description = "$item_surtlingcape_desc";
            surtlingCapeConf.CraftingStation = "forge";
            surtlingCapeConf.AddRequirement(new RequirementConfig("Coal", 30, 5));
            surtlingCapeConf.AddRequirement(new RequirementConfig("SurtlingCore", 5, 1));
            surtlingCapeConf.AddRequirement(new RequirementConfig("Coins", 666, 66));
            //surtlingCapeConf.AddRequirement(new RequirementConfig("Flametal", 6, 1));

            //Sprite and Texture loading
            Sprite surtlingIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/surtlingIcon.png");
            Texture2D surtlingTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/surtlingTexture.png");
            surtlingCapeConf.Icons = new Sprite[] { surtlingIcon };
            surtlingCapeConf.StyleTex = surtlingTex;

            CustomItem surtlingCape = new CustomItem("CapeSurtling", "CapeOdin", surtlingCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                surtlingCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = surtlingCapeStatus;
            }

            surtlingCape.ItemDrop.m_itemData.m_shared.m_dlc = "";

            HitData.DamageModPair veryFireRes = default(HitData.DamageModPair);
            veryFireRes.m_modifier = HitData.DamageModifier.VeryResistant;
            veryFireRes.m_type = HitData.DamageType.Fire;
            surtlingCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Add(veryFireRes);

            ItemManager.Instance.AddItem(surtlingCape);


            //Brawler Cape-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            brawlerCapeStatus.name = "$status_brawlercape";
            brawlerCapeStatus.m_name = "$status_brawlercape";
            brawlerCapeStatus.m_category = "brawlerCape";
            brawlerCapeStatus.m_tooltip = "$status_brawlercape_tooltip\n$status_brawlercape_tooltip1<color=orange>+" + HrugnirDamageMult.Value * 100f + "%</color>";
            brawlerCapeStatus.m_startMessage = "$status_brawlercape_startmessage";
            brawlerCapeStatus.m_addMaxCarryWeight = HrugnirCarryWeight.Value;

            ItemConfig brawlerCapeConf = new ItemConfig();
            brawlerCapeConf.Name = "$item_brawlercape";
            brawlerCapeConf.Description = "$item_brawlercape_desc";
            brawlerCapeConf.CraftingStation = "piece_cauldron";
            brawlerCapeConf.AddRequirement(new RequirementConfig("Root", 20, 5));
            brawlerCapeConf.AddRequirement(new RequirementConfig("MeadFrostResist", 5));
            brawlerCapeConf.AddRequirement(new RequirementConfig("Ooze", 12, 4));

            //Sprite and Texture loading
            Sprite brawlerIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/brawlerIcon.png");
            Texture2D brawlerTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/brawlerTexture.png");
            brawlerCapeConf.Icons = new Sprite[] { brawlerIcon };
            brawlerCapeConf.StyleTex = brawlerTex;

            CustomItem brawlerCape = new CustomItem("CapeBrawler", "CapeDeerHide", brawlerCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                brawlerCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = brawlerCapeStatus;
            }

            ItemManager.Instance.AddItem(brawlerCape);

            //Leech Cape-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            leechCapeStatus.name = "$status_leechcape";
            leechCapeStatus.m_name = "$status_leechcape";
            leechCapeStatus.m_category = "leechCape";
            leechCapeStatus.m_tooltip = "$status_leechcape_tooltip\n$status_leechcape_tooltip1<color=orange>+" + LeechLifesteal.Value * 100f + "%</color>";
            leechCapeStatus.m_startMessage = "$status_leechcape_startmessage";

            ItemConfig leechCapeConf = new ItemConfig();
            leechCapeConf.Name = "$item_leechcape";
            leechCapeConf.Description = "$item_leechcape_desc";
            leechCapeConf.CraftingStation = "piece_workbench";
            leechCapeConf.AddRequirement(new RequirementConfig("Bloodbag", 25, 7));
            leechCapeConf.AddRequirement(new RequirementConfig("Iron", 5));
            leechCapeConf.AddRequirement(new RequirementConfig("Root", 8, 4));

            //Sprite and Texture loading
            Sprite leechIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/leechIcon.png");
            Texture2D leechTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/leechTexture.png");
            leechCapeConf.Icons = new Sprite[] { leechIcon };
            leechCapeConf.StyleTex = leechTex;

            CustomItem leechCape = new CustomItem("CapeLeech", "CapeFeather", leechCapeConf);
            leechCape.ItemDrop.m_itemData.m_shared.m_damageModifiers.Clear(); // No Frost Resist for the swampers

            if (dragoonCapeEffectsEnabled.Value)
            {
                leechCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = leechCapeStatus;
            }

            ItemManager.Instance.AddItem(leechCape);


            //Stalker Cape-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            stalkerCapeStatus.name = "$status_stalkerscape";
            stalkerCapeStatus.m_name = "$status_stalkerscape";
            stalkerCapeStatus.m_category = "stalkerCape";
            stalkerCapeStatus.m_tooltip = "$status_stalkerscape_tooltip";
            stalkerCapeStatus.m_startMessage = "$status_stalkerscape_startmessage";
            stalkerCapeStatus.m_staminaRegenMultiplier = nightstalkerRegen.Value;
            stalkerCapeStatus.m_skillLevel = Skills.SkillType.Bows;
            stalkerCapeStatus.m_skillLevelModifier = nightstalkerSkill.Value;

            ItemConfig stalkerCapeConf = new ItemConfig();
            stalkerCapeConf.Name = "$item_stalkerscape";
            stalkerCapeConf.Description = "$item_stalkerscape_desc";
            stalkerCapeConf.CraftingStation = "forge";
            stalkerCapeConf.AddRequirement(new RequirementConfig("WolfHairBundle", 15, 5));
            stalkerCapeConf.AddRequirement(new RequirementConfig("Silver", 5));
            stalkerCapeConf.AddRequirement(new RequirementConfig("Obsidian", 8, 4));

            //Sprite and Texture loading
            Sprite stalkerIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/stalkerIcon.png");
            Texture2D stalkerTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/stalkerTexture.png");
            stalkerCapeConf.Icons = new Sprite[] { stalkerIcon };
            stalkerCapeConf.StyleTex = stalkerTex;

            CustomItem stalkerCape = new CustomItem("CapeStalker", "CapeOdin", stalkerCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                stalkerCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = stalkerCapeStatus;
            }

            stalkerCape.ItemDrop.m_itemData.m_shared.m_dlc = "";

            ItemManager.Instance.AddItem(stalkerCape);


            //Adventurer Cape-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            adventurerCapeStatus.name = "$status_adventurercape";
            adventurerCapeStatus.m_name = "$status_adventurercape";
            adventurerCapeStatus.m_category = "adventurerCape";
            adventurerCapeStatus.m_startMessage = "$status_adventurercape_startmessage";

            string adventurerTooltip = "$status_adventurercape_tooltip";
            string adventurerDesc1 = "\n$status_adventurercape_tooltip1<color=orange>"+AdventurerComfortBonus.Value+"</color>";
            string adventurerDesc2 = "\n<color=orange>$status_adventurercape_tooltip2</color>";

            if (AdventurerComfortBonus.Value > 0)
            {
                adventurerTooltip += adventurerDesc1;
            }

            if (AdventurerEffect.Value)
            {
                adventurerTooltip += adventurerDesc2;
            }

            adventurerCapeStatus.m_tooltip = adventurerTooltip;

            ItemConfig adventurerCapeConf = new ItemConfig();
            adventurerCapeConf.Name = "$item_adventurercape";
            adventurerCapeConf.Description = "$item_adventurercape_desc";
            adventurerCapeConf.CraftingStation = "piece_workbench";
            adventurerCapeConf.AddRequirement(new RequirementConfig("LoxPelt", 15, 5));
            adventurerCapeConf.AddRequirement(new RequirementConfig("Silver", 5));
            adventurerCapeConf.AddRequirement(new RequirementConfig("Guck", 5, 3));

            //Sprite and Texture loading
            Sprite adventurerIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/adventurerIcon.png");
            Texture2D adventurerTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/adventurerTexture.png");
            adventurerCapeConf.Icons = new Sprite[] { adventurerIcon };
            adventurerCapeConf.StyleTex = adventurerTex;

            CustomItem adventurerCape = new CustomItem("CapeAdventurer", "CapeLox", adventurerCapeConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                adventurerCape.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = adventurerCapeStatus;
            }

            ItemManager.Instance.AddItem(adventurerCape);

            //Fall Damage Belt-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            featherBeltStatus.name = "$status_featherbelt";
            featherBeltStatus.m_name = "$status_featherbelt";
            featherBeltStatus.m_tooltip = "$status_featherbelt_tooltip";
            featherBeltStatus.m_fallDamageModifier = -1f;
            featherBeltStatus.m_maxMaxFallSpeed = 5f;

            ItemConfig featherBeltConf = new ItemConfig();
            featherBeltConf.Name = "$item_featherbelt";
            featherBeltConf.Description = "$item_featherbelt_desc";
            featherBeltConf.CraftingStation = "piece_magetable";
            featherBeltConf.AddRequirement(new RequirementConfig("ScaleHide", 10));
            featherBeltConf.AddRequirement(new RequirementConfig("CapeFeather", 1));

            //Sprite and Texture loading
            Sprite featherIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/featherBeltIcon.png");
            Texture2D featherTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/featherBeltTexture.png");
            featherBeltConf.Icons = new Sprite[] { featherIcon };
            featherBeltConf.StyleTex = featherTex;

            CustomItem featherBelt = new CustomItem("BeltFeather", "BeltStrength", featherBeltConf);

            if (dragoonCapeEffectsEnabled.Value)
            {
                featherBelt.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = featherBeltStatus;
            }

            if (featherBeltEnabled.Value)
            {
                ItemManager.Instance.AddItem(featherBelt);
            }

            //Cleanup
            PrefabManager.OnVanillaPrefabsAvailable -= AddClonedItems;

        }
        private void AddArchivedClonedItems()
        {
            //Dragon Breath Staff-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            ItemConfig DragonStaffConf = new ItemConfig();
            DragonStaffConf.Name = "Staff of dragon's breath";
            DragonStaffConf.Description = "A dragon's breath can melt bone and steel alike.";
            DragonStaffConf.CraftingStation = "piece_workbench";
            DragonStaffConf.AddRequirement(new RequirementConfig("DragonTear", 1, 0));//the 2nd number is upgrade cost per level
            DragonStaffConf.AddRequirement(new RequirementConfig("FineWood", 20, 10));
            DragonStaffConf.AddRequirement(new RequirementConfig("Coal", 50, 10));
            DragonStaffConf.AddRequirement(new RequirementConfig("SurtlingCore", 5, 2));

            //Sprite and Texture loading
            //Sprite DragonIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/neckIcon.png");
            //Texture2D DragonTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/neckTexture.png");
            //DragonStaffConf.Icons = new Sprite[] { NeckIcon };
            //DragonStaffConf.StyleTex = DragonTex;

            CustomItem DragonStaff = new CustomItem("StaffDragon", "StaffFireball", DragonStaffConf);
            ItemManager.Instance.AddItem(DragonStaff);

            //mead Belt-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            meadBeltStatus.name = "Mead Buzz";
            meadBeltStatus.m_name = "Mead Buzz";
            meadBeltStatus.m_category = "meadBelt";
            meadBeltStatus.m_tooltip = "A warm feeling makes you ignore your fatigue.";
            meadBeltStatus.m_startMessage = "99 bottles of mead on the wall, 99 bottle of mead";
            meadBeltStatus.m_staminaRegenMultiplier = 1.15f;
            meadBeltStatus.m_addMaxCarryWeight = 50f;
            meadBeltStatus.m_speedModifier = 0.05f;

            ItemConfig meadBeltConf = new ItemConfig();
            meadBeltConf.Name = "Mead Belt";
            meadBeltConf.Description = "Imbues you with the essence of mead, whatever that means!";
            meadBeltConf.CraftingStation = "forge";
            meadBeltConf.AddRequirement(new RequirementConfig("MeadStaminaMinor", 6));
            meadBeltConf.AddRequirement(new RequirementConfig("Iron", 1));
            meadBeltConf.AddRequirement(new RequirementConfig("DeerHide", 20));

            //Sprite and Texture loading
            //Sprite meadIcon = AssetUtils.LoadSpriteFromFile("HappyDragoon-DragoonCapes/Assets/meadBeltIcon.png");
            //Texture2D meadTex = AssetUtils.LoadTexture("HappyDragoon-DragoonCapes/Assets/meadBeltTexture.png");
            //meadBeltConf.Icons = new Sprite[] { meadIcon };
            //meadBeltConf.StyleTex = meadTex;

            CustomItem meadBelt = new CustomItem("BeltMead", "BeltStrength", meadBeltConf);
            meadBelt.ItemDrop.m_itemData.m_shared.m_equipStatusEffect = meadBeltStatus;
            ItemManager.Instance.AddItem(meadBelt);

            //Cleanup
            PrefabManager.OnVanillaPrefabsAvailable -= AddArchivedClonedItems;
        }
        }
}

