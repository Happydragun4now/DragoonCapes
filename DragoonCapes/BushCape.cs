using HarmonyLib;
using JetBrains.Annotations;
using Jotunn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = Jotunn.Logger;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class BushCape
    {
        /*
        //Draw Speed is too OP here, but this is how its implemented for future reference
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "GetSkillFactor")]
        private static void Postfix(Player __instance, Skills.SkillType skill, ref float __result)
        {
            if ((skill == Skills.SkillType.Bows || skill == Skills.SkillType.Crossbows) && __instance.GetSEMan().HaveStatusEffectCategory("bushCape"))
            {
                //10% Speed with bows/crossbows
                Logger.LogInfo("Pre-Buff Skill factor: " + __result);
                __result *= 1.1f;
                Logger.LogInfo("Post-Buff Skill factor: " + __result);
            }

        }
        */

        //Patch the firing bow function to change velocity, and noise values
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Attack), "FireProjectileBurst")]
        public static void Attack_FireProjectileBurst_Prefix(Attack __instance)
        {
            //Checking for null values is really important here
            //Make sure player is alive, and exists
            //Make sure the weapon is existant, and the player is wearing the cape
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || __instance?.GetWeapon() == null || !player.GetSEMan().HaveStatusEffectCategory("bushCape"))
            {
                return;
            }
            //Get the skill the weapon uses
            Skills.SkillType? skillType = __instance.GetWeapon()?.m_shared.m_skillType;
            if (skillType == Skills.SkillType.Bows || skillType == Skills.SkillType.Crossbows)
            {
                __instance.m_projectileVel *= (1f + DragoonCapes.Instance.BushVelocity.Value);
                __instance.m_attackStartNoise *= DragoonCapes.Instance.BushNoiseReduction.Value;
                __instance.m_attackHitNoise *= DragoonCapes.Instance.BushNoiseReduction.Value;
            }
        }
    }
}
