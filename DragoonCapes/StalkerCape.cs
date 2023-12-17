using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Logger = Jotunn.Logger;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class StalkerCape
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void Damage_Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (hit.GetAttacker() == player && player.GetSEMan().HaveStatusEffectCategory("stalkerCape") && player.GetSEMan().HaveStatusEffect("Cold"))
            {
                hit.m_damage.Modify(Math.Max(1f,1f + DragoonCapes.Instance.nightstalkerDamageMult.Value));
            }
        }

    }
}
