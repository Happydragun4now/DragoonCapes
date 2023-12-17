using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class BerserkCape
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void Damage_Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (hit.GetAttacker() == player && player.GetSEMan().HaveStatusEffectCategory("berserkCape") && !hit.m_ranged /*&& player.GetCurrentWeapon().IsTwoHanded()*/)
            {
                float missingHealth = 1f - player.GetHealthPercentage();
                //Logger.LogInfo("Missing Health: " + missingHealth);
                float dmgMod = 1f + (missingHealth * DragoonCapes.Instance.BerserkMoveSpeed.Value);
                //Logger.LogInfo("Damage mod: " + dmgMod);
                hit.m_damage.Modify(dmgMod);
            }
        }
    }
}
