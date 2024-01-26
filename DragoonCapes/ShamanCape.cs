using HarmonyLib;
using MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace DragoonCapes
{
    internal class ShamanCape
    {
        [HarmonyPatch]
        private class ShamanPatch
        {

            //ShamanCape Patch
            //Make Eitr attacks use the Eitr cost in health 
            //needs to lose the black feather effect somehow, and not reset combo when using health.
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Attack), "Start")]
            public static void Attack_ChangeCosts_Prefix(ref Attack __instance, ref ItemDrop.ItemData weapon)
            {
                Player player = Player.m_localPlayer;
                if (player == null || player.IsDead())
                {
                    return;
                }

                if (player.GetSEMan().HaveStatusEffectCategory("ShamanCape") && weapon.m_shared.m_attack.m_attackEitr > player.GetEitr())
                {
                    float curEitr = player.GetEitr();
                    float healthCost = weapon.m_shared.m_attack.m_attackEitr - curEitr;//health cost is the eitr cost - the eitr it will tak
                    __instance.m_attackHealth = healthCost;
                    __instance.m_attackEitr = curEitr-0.1f;//hopefully not emptying Eitr will allow it to cast.
                }
            }
        }
    }
}
