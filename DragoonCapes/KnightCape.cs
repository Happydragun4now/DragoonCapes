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
            bool haveStatus = Player.m_localPlayer.GetSEMan().HaveStatusEffectCategory("knightCape");
            if (haveStatus)
            {
                //reduce the damage going to stagger by 25%
                //Logger.LogInfo("Stagger Damage pre-res: " + damage);
                damage *= (1-DragoonCapes.Instance.KnightStaggerRes.Value);
                //Logger.LogInfo("Stagger Damage post-res: " + damage);

            }
        }
    }
}
