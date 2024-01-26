using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Jotunn.Logger;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class AdventurerCape
    {
        //Rested AF
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "GetTotalFoodValue", null)]
        private static void Rested_Postfix()
        {
            Player player = Player.m_localPlayer;

            //Logger.LogInfo("Adventurer Cape: " + player.GetSEMan().HaveStatusEffectCategory("adventurerCape"));
            if (DragoonCapes.Instance.AdventurerEffect1.Value && player.GetSEMan().HaveStatusEffectCategory("adventurerCape") && !player.GetSEMan().HaveStatusEffect("Rested"))
            {
                player.GetSEMan().AddStatusEffect("Rested".GetHashCode());
            }
        }
        //really it should remove rested on dequip.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Humanoid), "UnequipItem", null)]
        private static void Unrest_Postfix(ref ItemDrop.ItemData item)
        {
            Player player = Player.m_localPlayer;
            if (item == null || player == null || item.m_shared.m_equipStatusEffect == null)
            {
                return;
            }
            //Logger.LogInfo("Adventurer Cape: " + player.GetSEMan().HaveStatusEffectCategory("adventurerCape"));
            if (DragoonCapes.Instance.AdventurerEffect1.Value && item.m_shared.m_equipStatusEffect.m_category == "adventurerCape" && player.GetSEMan().HaveStatusEffect("Rested"))
            {
                player.GetSEMan().RemoveStatusEffect("Rested".GetHashCode());
            }
        }

        //Sleep Restriction Dodging
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bed), "CheckExposure")]
        private static void SleepRoof_Postfix(ref bool __result)
        {
            //if wearing adventurerCape, never check exposure?
            Player player = Player.m_localPlayer;
            if (DragoonCapes.Instance.AdventurerEffect2.Value && player.GetSEMan().HaveStatusEffectCategory("adventurerCape"))
            {
                __result = true;
            }

        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bed), "CheckFire")]
        private static void SleepFire_Prefix(ref bool __result)
        {
            //if wearing adventurerCape, never check exposure?
            Player player = Player.m_localPlayer;
            if (DragoonCapes.Instance.AdventurerEffect2.Value && player.GetSEMan().HaveStatusEffectCategory("adventurerCape"))
            {
                __result = true;
            }

        }
    }
}
