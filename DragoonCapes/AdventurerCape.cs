using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Jotunn.Logger;
using System.Threading;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class AdventurerCape
    {
        //+X Comfort Level
        //Type[] needed to seperate ambiguous methods
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SE_Rested), "CalculateComfortLevel", new Type[] { typeof(Player) })]
        private static void MoreComfort_Postfix(ref int __result, ref Player player)
        {
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("adventurerCape"))
            {
                return;
            }
            __result += DragoonCapes.Instance.AdventurerComfortBonus.Value;
        }
        //Remove Rested on Dequip
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Humanoid), "UnequipItem", null)]
        private static void Unrest_Postfix(ref ItemDrop.ItemData item)
        {
            Player player = Player.m_localPlayer;
            if (item == null || player == null || player.IsDead() || item.m_shared.m_equipStatusEffect == null /*|| !player.GetSEMan().HaveStatusEffectCategory("adventurerCape")*/)
            {
                return;
            }
            if (item.m_shared.m_equipStatusEffect.m_category == "adventurerCape" && DragoonCapes.Instance.AdventurerComfortBonus.Value > 0 && player.GetSEMan().HaveStatusEffect("Rested".GetHashCode()))
            {
                //player.GetSEMan().RemoveStatusEffect("Rested".GetHashCode());
                //Remove comfort level added x60 seconds from the rested buff when the cape is removed
                player.GetSEMan().RemoveStatusEffect("Resting".GetHashCode());
                player.GetSEMan().GetStatusEffect("Rested".GetHashCode()).m_time += DragoonCapes.Instance.AdventurerComfortBonus.Value * 60f;
            }
        }

        //Sleep Restriction Dodging
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bed), "CheckExposure")]
        private static void SleepRoof_Postfix(ref bool __result)
        {
            //if wearing adventurerCape, never check exposure?
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("adventurerCape"))
            {
                return;
            }
            if (DragoonCapes.Instance.AdventurerEffect.Value)
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
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("adventurerCape"))
            {
                return;
            }
            if (DragoonCapes.Instance.AdventurerEffect.Value)
            {
                __result = true;
            }

        }
    }
}
