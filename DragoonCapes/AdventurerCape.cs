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
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "GetTotalFoodValue", null)]
        private static void Rested_Postfix()
        {
            Player player = Player.m_localPlayer;

            //Logger.LogInfo("Adventurer Cape: " + player.GetSEMan().HaveStatusEffectCategory("adventurerCape"));
            if (player.GetSEMan().HaveStatusEffectCategory("adventurerCape") && !player.GetSEMan().HaveStatusEffect("Rested"))
            {
                player.GetSEMan().AddStatusEffect("Rested".GetHashCode());
            }
        }
    }
}
