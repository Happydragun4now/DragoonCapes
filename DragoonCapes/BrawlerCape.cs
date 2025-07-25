﻿using BepInEx.Logging;
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
    internal class BrawlerCape
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void JotunnPunch_Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || hit == null || !player.GetSEMan().HaveStatusEffectCategory("brawlerCape"))
            {
                return;
            }
            SEMan playerStatus = player.GetSEMan();
            if (hit.GetAttacker() == player && (player.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Unarmed))
            {
                //Logger.LogInfo("Pre-Buff Damage: " + hit.m_damage.GetTotalDamage());
                hit.m_damage.Modify(1f + DragoonCapes.Instance.HrugnirDamageMult.Value);
            }
            /*
            if (playerStatus.HaveStatusEffectCategory("healthpotion") || playerStatus.HaveStatusEffectCategory("staminapotion") || playerStatus.HaveStatusEffectCategory("eitrpotion") || playerStatus.HaveStatusEffect("Potion_frostresist".GetHashCode()) || playerStatus.HaveStatusEffect("Potion_poisonresist".GetHashCode()) || playerStatus.HaveStatusEffect("Potion_tasty".GetHashCode()))
            {
                if (hit.GetAttacker() == player && (player.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Unarmed))
                {
                    //Logger.LogInfo("Pre-Buff Damage: " + hit.m_damage.GetTotalDamage());
                    hit.m_damage.Modify(1f+DragoonCapes.Instance.HrugnirDamageMult.Value);
                }
            }
            */
        }
    }
}
