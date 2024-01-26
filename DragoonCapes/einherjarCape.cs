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
using TMPro;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class einherjarCape
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Attack), "FireProjectileBurst")]
        public static void Attack_FireProjectileBurst_Prefix(Attack __instance)
        {
            Player player = Player.m_localPlayer;
            bool haveStatus = player.GetSEMan().HaveStatusEffectCategory("einherjarCape");
            if (haveStatus)
            {
                Skills.SkillType? skillType = __instance.GetWeapon()?.m_shared.m_skillType;
                if (skillType == Skills.SkillType.Spears)
                {
                    __instance.m_projectileVel *= DragoonCapes.Instance.EinherjarVelocity.Value;
                }
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void LightningSpear_Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (!player.GetSEMan().HaveStatusEffectCategory("einherjarCape"))
            {
                return;
            }
            bool weaponType1 = hit.m_skill == Skills.SkillType.Spears;
            bool weaponType2 = hit.m_skill == Skills.SkillType.Polearms;
            //bool weaponType3 = hit.m_toolTier == 0;//fists are tier 0, makes spear throws add dmg! this has some side effects though.
            if (hit.GetAttacker() == player && (weaponType1 || weaponType2))
            {
                hit.m_damage.m_lightning += hit.GetTotalDamage() * DragoonCapes.Instance.EinherjarDamageMult.Value;
            }
        }

        //infinite throw patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Projectile), "Setup")]
        public static void Proj_Setup_Prefix(Projectile __instance)
        {
            //this might not play well in MP
            Player player = Player.m_localPlayer;
            if (DragoonCapes.Instance.EinherjarEffect.Value && player.GetSEMan().HaveStatusEffectCategory("einherjarCape"))
            {
                //Don't spawn the item drop on hit if einherjar cape
                __instance.m_respawnItemOnHit = false;
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Attack), "OnAttackTrigger")]
        public static void Attack_Postfix(Attack __instance)
        {
            //this might not play well in MP
            Player player = Player.m_localPlayer;
            if (DragoonCapes.Instance.EinherjarEffect.Value && player.GetSEMan().HaveStatusEffectCategory("einherjarCape") && __instance.GetWeapon()?.m_shared.m_skillType == Skills.SkillType.Spears)
            {
                //if its a spear attack and they have the cape on, don't consume the cape
                __instance.m_consumeItem = false;
            }
        }
    }

}
