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
            bool weaponType3 = hit.m_toolTier == 0;//fists are tier 0, but are claws as well?
            /*
            bool weaponType1 = player.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Spears;
            bool weaponType2 = player.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Polearms;
            bool weaponType3 = player.GetCurrentWeapon() == null;//need something that works for empty handed but not fist weapons so that thrown spear still works
            */
            if (hit.GetAttacker() == player && (weaponType1 || weaponType2 || weaponType3))
            {
                hit.m_damage.m_lightning += hit.GetTotalDamage() * DragoonCapes.Instance.EinherjarDamageMult.Value;
            }
        }
        /*
        //return on throw somehow IDK
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Projectile), "SpawnOnHit")]
        public static void Proj_SpawnOnHit_Prefix(Projectile __instance, GameObject go)
        {
            Logger.LogInfo("RespawnItemonHit: " + __instance.m_respawnItemOnHit);
            __instance.m_respawnItemOnHit = false;
            Logger.LogInfo("RespawnItemonHit2: " + __instance.m_respawnItemOnHit);

            if (go == null)
            {
                Logger.LogInfo("gameobject was null");
                return;
            }
            Player player = Player.m_localPlayer;
            bool haveStatus = player.GetSEMan().HaveStatusEffectCategory("einherjarCape");
            if (haveStatus)
            {
                Logger.LogInfo("Spear weapon and Einherjar Cape");
                //player.Pickup(go, true, false);//this was spawning extra spears on the ground
            }
        }
        */
    }

}
