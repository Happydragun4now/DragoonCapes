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
            SEMan playerStatus = player.GetSEMan();

            if (player == null || player.IsDead() || hit == null || !player.GetSEMan().HaveStatusEffectCategory("brawlerCape"))
            {
                return;
            }
            if (playerStatus.HaveStatusEffectCategory("healthpotion") || playerStatus.HaveStatusEffectCategory("staminapotion") || playerStatus.HaveStatusEffectCategory("eitrpotion") || playerStatus.HaveStatusEffect("Potion_frostresist") || playerStatus.HaveStatusEffect("Potion_poisonresist") || playerStatus.HaveStatusEffect("Potion_tasty"))
            {
                if (hit.GetAttacker() == player && (player.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Unarmed))
                {
                    //Logger.LogInfo("Pre-Buff Damage: " + hit.m_damage.GetTotalDamage());
                    hit.m_damage.Modify(1f+DragoonCapes.Instance.HrugnirDamageMult.Value);
                }
            }
            
        }
    }
}
