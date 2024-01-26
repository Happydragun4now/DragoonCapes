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

    //I think these names are okay because the code side variable names don't get translated?
    private static readonly string[] trollSet = new string[3] { "HelmetTrollLeather", "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs" };
    //private static readonly string[] trollSet = new string[3] { "$item_helmet_trollleather", "$item_chest_trollleather", "$item_legs_trollleather" };

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
        WolfCapeStatus.name = "$status_wolfcape";
        WolfCapeStatus.m_name = "$status_wolfcape";
        WolfCapeStatus.m_tooltip = "$status_wolfcape_desc";
        WolfCapeStatus.m_startMessage = "$status_wolfcape_msg";
        WolfCapeStatus.m_skillLevel = Skills.SkillType.Spears;
        WolfCapeStatus.m_skillLevel2 = Skills.SkillType.Polearms;
        WolfCapeStatus.m_skillLevelModifier = WolfCapeStatus.m_skillLevelModifier2 = DragoonCapes.DragoonCapes.Instance.WolfSkill.Value;
        __instance.m_StatusEffects.Add(WolfCapeStatus);

        DeerCapeStatus.name = "$status_deercape";
        DeerCapeStatus.m_name = "$status_deercape";
        DeerCapeStatus.m_tooltip = "$status_deercape_desc";
        DeerCapeStatus.m_startMessage = "$status_deercape_msg";
        DeerCapeStatus.m_skillLevel = Skills.SkillType.Bows;
        DeerCapeStatus.m_skillLevel2 = Skills.SkillType.Crossbows;
        DeerCapeStatus.m_skillLevelModifier = DeerCapeStatus.m_skillLevelModifier2 = DragoonCapes.DragoonCapes.Instance.DeerSkill.Value;
        DeerCapeStatus.m_speedModifier = DragoonCapes.DragoonCapes.Instance.DeerMoveSpeed.Value;
        __instance.m_StatusEffects.Add(DeerCapeStatus);

        LoxCapeStatus.name = "$status_loxcape";
        LoxCapeStatus.m_name = "$status_loxcape";
        LoxCapeStatus.m_tooltip = "$status_loxcape_desc1\n$status_loxcape_desc2<color=orange>+" + ((DragoonCapes.DragoonCapes.Instance.LoxHealth.Value-1)*100) + "%</color>\n$status_loxcape_desc3<color=orange>" + DragoonCapes.DragoonCapes.Instance.LoxHealing.Value + " $status_loxcape_desc4</color>";
        LoxCapeStatus.m_startMessage = "$status_loxcape_msg";
        //could make this into food healing so it doesn't appear as a different healing tick, food healing handled in function Player -> "UpdateFood"
        LoxCapeStatus.m_tickInterval = DragoonCapes.DragoonCapes.Instance.LoxHealInterval.Value;
        LoxCapeStatus.m_healthPerTick = DragoonCapes.DragoonCapes.Instance.LoxHealing.Value;
        LoxCapeStatus.m_category = "LoxCape";
        __instance.m_StatusEffects.Add(LoxCapeStatus);
        
        //should prob change this - Samurai themed? split arrows and some melee bonus
        LinenCapeStatus.name = "$status_linencape";
        LinenCapeStatus.m_name = "$status_linencape";
        LinenCapeStatus.m_tooltip = "$status_linencape_desc1\n$status_linencape_desc2<color=orange>+" + DragoonCapes.DragoonCapes.Instance.LinenStagger.Value*100+ "%</color>\n$status_linencape_desc3<color=orange>" + DragoonCapes.DragoonCapes.Instance.LinenProjs.Value+ "</color>\n$status_linencape_desc4<color=orange>" + DragoonCapes.DragoonCapes.Instance.LinenDamageMult.Value*100+"%</color>";
        LinenCapeStatus.m_startMessage = "$status_linencape_msg";
        LinenCapeStatus.m_category = "LinenCape";
        __instance.m_StatusEffects.Add(LinenCapeStatus);

        TrollCapeStatus.name = "$status_trollcape";
        TrollCapeStatus.m_name = "$status_trollcape";
        TrollCapeStatus.m_tooltip = "$status_trollcape_desc1\n$status_trollcape_desc2<color=orange>" + DragoonCapes.DragoonCapes.Instance.TrollDamageMult.Value*100f +"%</color>";
        TrollCapeStatus.m_startMessage = "$status_trollcape_msg";
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
