using HarmonyLib;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Jotunn.Logger;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static Skills;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class LeechCape
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void Lifesteal_Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (!player.GetSEMan().HaveStatusEffectCategory("leechCape"))
            {
                return;
            }
            if (hit.GetAttacker() == player)
            {
                player.Heal(Math.Max(0f,hit.GetTotalDamage() * DragoonCapes.Instance.LeechLifesteal.Value));
            }
        }
    }

}
