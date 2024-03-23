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
        //Increase Projectile Velocity
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Attack), "FireProjectileBurst")]
        public static void Attack_FireProjectileBurst_Prefix(Attack __instance)
        {
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("einherjarCape"))
            {
                return;
            }
            Skills.SkillType? skillType = __instance.GetWeapon()?.m_shared.m_skillType;
            if (skillType == Skills.SkillType.Spears)
            {
                __instance.m_projectileVel *= DragoonCapes.Instance.EinherjarVelocity.Value;
            }
        }
        //Add lightning damage
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void LightningSpear_Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("einherjarCape"))
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

        //Don't spawn a spear from the projectile when it lands
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Projectile), "Setup")]
        public static void Proj_Setup_Prefix(Projectile __instance)
        {
            //this might not play well in MP
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("einherjarCape"))
            {
                return;
            }
            if (DragoonCapes.Instance.EinherjarEffect.Value)
            {
                __instance.m_respawnItemOnHit = false;
            }
        }
        //Don't remove spear from your hand
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Attack), "OnAttackTrigger")]
        public static void AttackTrigger_Prefix(Attack __instance)
        {
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("einherjarCape"))
            {
                return;
            }
            if (DragoonCapes.Instance.EinherjarEffect.Value && __instance.GetWeapon()?.m_shared.m_skillType == Skills.SkillType.Spears && __instance.m_consumeItem == true)
            {
                //if its a *thrown spear attack and they have the cape on, don't remove spear from inventory
                __instance.m_consumeItem = false;
            }
        }
        //Stamina Cost Patch
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Attack), "Start")]
        public static void Attack_ChangeCosts_Prefix(ref Attack __instance, ref ItemDrop.ItemData weapon)
        {
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("einherjarCape"))
            {
                return;
            }
            //Logger.LogInfo("Attack Start Detected");
            //Logger.LogInfo("Flag1: "+ DragoonCapes.Instance.EinherjarEffect.Value);
            //Logger.LogInfo("Flag2: "+ player.GetSEMan().HaveStatusEffectCategory("einherjarCape"));
            //Logger.LogInfo("Flag3: "+ (weapon.m_shared.m_skillType == Skills.SkillType.Spears));
            if (DragoonCapes.Instance.EinherjarEffect.Value && weapon.m_shared.m_skillType == Skills.SkillType.Spears && __instance.m_attackProjectile != null)
            {
                //Logger.LogInfo("Stamina Cost Changed, before: " + __instance.m_attackStamina);
                __instance.m_attackStamina *= DragoonCapes.Instance.EinherjarCostMult.Value;
                //Logger.LogInfo("Stamina Cost Changed, after: " + __instance.m_attackStamina);
            }
        }
    }

}
