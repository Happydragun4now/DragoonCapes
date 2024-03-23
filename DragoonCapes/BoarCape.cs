using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class BoarCape
    {
        private static float baseStaminaUse = 10f;

        [HarmonyPatch(typeof(Player), "Dodge")]
        private static void Prefix(ref float ___m_dodgeStaminaUsage, ref SEMan ___m_seman)
        {
            if (___m_seman == null)
            {
                return;
            }
            if (___m_seman.HaveStatusEffectCategory("boarCape"))
            {
                ___m_dodgeStaminaUsage = baseStaminaUse * Math.Max(0.01f, 1f - DragoonCapes.Instance.BoarDodgeMult.Value);
            }
            else if (___m_dodgeStaminaUsage != baseStaminaUse)
            {
                ___m_dodgeStaminaUsage = baseStaminaUse;
            }
        }
    }
}
