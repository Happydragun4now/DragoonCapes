using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ItemSets;
using static Skills;
using Logger = Jotunn.Logger;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class StalkerCape
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Skills), "GetSkillFactor")]
        private static void BowSkill_postfix(ref Skills __instance, ref SkillType skillType, ref float __result)
        {
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("stalkerCape"))
            {
                return;
            }
            //This works but shows the bow skill during the daytime
            if (skillType == Skills.SkillType.Bows)
            {
                if (EnvMan.IsNight())
                {
                    __result = Mathf.Clamp(__instance.GetSkillLevel(skillType)/100f, 0f, (100f+DragoonCapes.Instance.nightstalkerSkill.Value)/100f);
                }
                else
                {
                    float newSkill = __instance.GetSkillLevel(skillType) - DragoonCapes.Instance.nightstalkerSkill.Value;
                    __result = Mathf.Clamp(newSkill / 100f, 0f, (100f + DragoonCapes.Instance.nightstalkerSkill.Value) / 100f);
                }
                //__result += DragoonCapes.Instance.nightstalkerSkillFactor.Value;//increases the returned bow skill factor by the config value in a way it can go over 100 bow skill
                //Logger.LogInfo("Returned Bow Skill: " + __result);
            }
        }

    }
}
