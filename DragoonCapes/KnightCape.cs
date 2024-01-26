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
    internal class KnightCape
    {
        //Stagger Resist when equipped
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "AddStaggerDamage")]
        private static void ChangeStaggerRes_Prefix(ref float damage)
        {
            Player player = Player.m_localPlayer;
            if (player == null || player.IsDead())
            {
                return;
            }

            if (player.GetSEMan().HaveStatusEffectCategory("knightCape"))
            {
                //reduced added stagger damage when you are getting hit
                damage *= (1-DragoonCapes.Instance.KnightStaggerRes.Value);
            }
        }
    }
}
