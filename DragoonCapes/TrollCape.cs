﻿using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class TrollCape
    {
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("TrollCape"))
            {
                //Jotunn.Logger.LogInfo("Player NOT Wearing Troll Cape");
                return;
            }   
            if (hit.GetAttacker() == player && (player.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Knives))
            {
                //Damage can go over 1 if sneak is high enough, wearing troll cape, and using a knife
                //Jotunn.Logger.LogInfo("Troll Cape Damage Modifier: " + Math.Max(1, DragoonCapes.Instance.TrollDamageMult.Value * player.GetSkillLevel(Skills.SkillType.Sneak) / 100f));
                hit.m_damage.Modify(Math.Max(1, DragoonCapes.Instance.TrollDamageMult.Value * player.GetSkillLevel(Skills.SkillType.Sneak) / 100f));
            }
        }
    }
}
