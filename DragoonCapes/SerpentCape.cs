using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DragoonCapes
{
    internal class SerpentCape
    {
        [HarmonyPatch]
        private class SerpentPatch
        {
            [HarmonyPatch(typeof(Character), "Damage")]
            private static void Prefix(HitData hit)
            {
                Player player = Player.m_localPlayer;
                if (player == null || player.IsDead() || !player.GetSEMan().HaveStatusEffectCategory("SerpentCape"))
                {
                    return;
                }
                if (hit.GetAttacker() == player)
                {
                   hit.m_damage.m_poison += hit.GetTotalDamage() * DragoonCapes.Instance.SerpentDamageMult.Value;
                }
            }
        }
    }
}
