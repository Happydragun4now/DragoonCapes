using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using static ItemDrop.ItemData;
using static Skills;
using static UnityEngine.GridBrushBase;
using Logger = Jotunn.Logger;
using DragoonCapes;

[HarmonyPatch(typeof(ObjectDB))]
internal static class PatchObjectDB
{

    private static SE_Stats TrollCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
    private static SE_Stats LoxCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
    private static SE_Stats WolfCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
    private static SE_Stats DeerCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();
    private static SE_Stats LinenCapeStatus = ScriptableObject.CreateInstance<SE_Stats>();

    private static readonly string[] trollSet = new string[3] { "HelmetTrollLeather", "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs" };

    //this is the patch for the vanilla items!
    [HarmonyPatch("Awake")]
    [HarmonyPrefix]
    public static void Show_Postfix(ObjectDB __instance)
    {

        if (SceneManager.GetActiveScene().name != "main")
        {
            return;
        }
        //If the config value was set as false, skip all the vanilla status stuff
        if (!DragoonCapes.DragoonCapes.Instance.vanillaCapeEffectsEnabled.Value)
        {
            return;
        }

        //Make the Status Effect for each cape
        WolfCapeStatus.name = "Wolf's Fury";
        WolfCapeStatus.m_name = "Wolf's Fury";
        WolfCapeStatus.m_tooltip = "I am not a wolf in sheep’s clothing, I’m a wolf in wolf’s clothing.";
        WolfCapeStatus.m_startMessage = "AWWWWWOOOOOOOOOOOOO";
        WolfCapeStatus.m_skillLevel = Skills.SkillType.Spears;
        WolfCapeStatus.m_skillLevel2 = Skills.SkillType.Polearms;
        WolfCapeStatus.m_skillLevelModifier = WolfCapeStatus.m_skillLevelModifier2 = DragoonCapes.DragoonCapes.Instance.WolfSkill.Value;
        __instance.m_StatusEffects.Add(WolfCapeStatus);

        DeerCapeStatus.name = "Deer's Grace";
        DeerCapeStatus.m_name = "Deer's Grace";
        DeerCapeStatus.m_tooltip = "An army of lions commanded by a deer will never be an army of lions";
        DeerCapeStatus.m_startMessage = "Oh Deery Me, this cape will have the ladies fawning over you.";
        DeerCapeStatus.m_skillLevel = Skills.SkillType.Bows;
        DeerCapeStatus.m_skillLevel2 = Skills.SkillType.Crossbows;
        DeerCapeStatus.m_skillLevelModifier = DeerCapeStatus.m_skillLevelModifier2 = DragoonCapes.DragoonCapes.Instance.DeerSkill.Value;
        DeerCapeStatus.m_speedModifier = DragoonCapes.DragoonCapes.Instance.DeerMoveSpeed.Value;
        __instance.m_StatusEffects.Add(DeerCapeStatus);

        LoxCapeStatus.name = "Lox's Haven";
        LoxCapeStatus.m_name = "Lox's Haven";
        LoxCapeStatus.m_tooltip = "The mightly lox's blood strengthens you.\nHealth: <color=orange>+"+ ((DragoonCapes.DragoonCapes.Instance.LoxHealth.Value-1)*100) +"%</color>\nHeal: <color=orange>"+ DragoonCapes.DragoonCapes.Instance.LoxHealing.Value + " hp/tick</color>";
        LoxCapeStatus.m_startMessage = "Waddya call a lox in a tar pit? Stuck!";
        //could make this into food healing so it doesn't appear as a different healing tick, food healing handled in function Player -> "UpdateFood"
        LoxCapeStatus.m_tickInterval = DragoonCapes.DragoonCapes.Instance.LoxHealInterval.Value;
        LoxCapeStatus.m_healthPerTick = DragoonCapes.DragoonCapes.Instance.LoxHealing.Value;
        LoxCapeStatus.m_category = "LoxCape";
        __instance.m_StatusEffects.Add(LoxCapeStatus);
        
        //should prob change this - Samurai themed? split arrows and some melee bonus
        LinenCapeStatus.name = "Traveler's Humility";
        LinenCapeStatus.m_name = "Traveler's Humility";
        LinenCapeStatus.m_tooltip = "You have picked up many tricks through your travels.\nMelee Stagger: <color=orange>+"+DragoonCapes.DragoonCapes.Instance.LinenStagger.Value*100+"%</color>\nProjectile Multiplier: <color=orange>"+DragoonCapes.DragoonCapes.Instance.LinenProjs.Value+ "</color>\nProjectile Damage: <color=orange>"+DragoonCapes.DragoonCapes.Instance.LinenDamageMult.Value*100+"%</color>";
        LinenCapeStatus.m_startMessage = "Be wary my son, for evil finds a way.";
        LinenCapeStatus.m_category = "LinenCape";
        __instance.m_StatusEffects.Add(LinenCapeStatus);

        TrollCapeStatus.name = "Troll's Ambush";
        TrollCapeStatus.m_name = "Troll's Ambush";
        TrollCapeStatus.m_tooltip = "Trolls evolved to blend in with the night sky, now that advantage is yours. Your knife damage will scale with your sneak skill \nKnife damage at maximum sneak: <color=orange>" + DragoonCapes.DragoonCapes.Instance.TrollDamageMult.Value*100f +"%</color>";
        TrollCapeStatus.m_startMessage = "Trololololololo";
        TrollCapeStatus.m_category = "TrollCape";
        //TrollCapeStatus.m_staminaRegenMultiplier = 1.25f;
        //TrollCapeStatus.m_addMaxCarryWeight = 30f;
        __instance.m_StatusEffects.Add(TrollCapeStatus);

        foreach (GameObject item in __instance.m_items)
        {
            if (item.name == "CapeDeerHide")
            {
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_equipStatusEffect = DeerCapeStatus;
            }
            else if (item.name == "CapeWolf")
            {
                //set armor and armor per level values on the item according to the config values
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_armor = DragoonCapes.DragoonCapes.Instance.WolfArmor.Value;
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_armorPerLevel = DragoonCapes.DragoonCapes.Instance.WolfArmorPerLevel.Value;
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_equipStatusEffect = WolfCapeStatus;
            }
            else if (item.name == "CapeLox")
            {
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_equipStatusEffect = LoxCapeStatus;
            }
            else if (item.name == "CapeLinen")
            {
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_equipStatusEffect = LinenCapeStatus;
            }
            else if (item.name == "CapeTrollHide")
            {
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_equipStatusEffect = TrollCapeStatus;
                //thanks to GoldenRevolver for this part that makes the troll set 3 pieces
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_setName = string.Empty;
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_setSize = 0;
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_setStatusEffect = null;
            }
            else if (trollSet.Contains(item.name))
            {
                item.GetComponent<ItemDrop>().m_itemData.m_shared.m_setSize = 3;
            }
        }
    }
}
