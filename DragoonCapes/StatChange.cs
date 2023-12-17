using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class StatChange
    {
        [HarmonyPatch]
        private class PlayerFoodPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
            private static void Player_GetTotalFoodValue_Postfix(ref float eitr, ref float hp, ref float stamina)
            {
                if (Player.m_localPlayer.GetSEMan().HaveStatusEffectCategory("LoxCape"))
                {
                    hp *= DragoonCapes.Instance.LoxHealth.Value;
                }
                //sandwiched in here due to laziness
                else if (Player.m_localPlayer.GetSEMan().HaveStatusEffectCategory("dwarfCape"))
                {
                    eitr += DragoonCapes.Instance.GreyEitr.Value;
                }
                else if (Player.m_localPlayer.GetSEMan().HaveStatusEffectCategory("wraithCape"))
                {
                    hp *= 1f - DragoonCapes.Instance.WraithPenalty.Value;
                    stamina *= 1f - DragoonCapes.Instance.WraithPenalty.Value;
                    eitr *= 1f - DragoonCapes.Instance.WraithPenalty.Value;
                }
            }
        }
    }
}
