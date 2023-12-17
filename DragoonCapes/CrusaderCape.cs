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
    internal class CrusaderCape
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void Spirit_Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            //Logger.LogInfo("Have crusader status: "+player.GetSEMan().HaveStatusEffectCategory("crusaderCape"));
            if (hit.GetAttacker() == player && player.GetSEMan().HaveStatusEffectCategory("crusaderCape"))
            {
                //Logger.LogInfo("Adding Spirit Damage");
                float initialTotal = hit.GetTotalDamage();
                hit.m_damage.m_spirit += initialTotal * DragoonCapes.Instance.CrusaderDamageSpirit.Value;
                hit.m_damage.m_blunt += initialTotal * DragoonCapes.Instance.CrusaderDamageBlunt.Value;
            }
        }

    }
}
