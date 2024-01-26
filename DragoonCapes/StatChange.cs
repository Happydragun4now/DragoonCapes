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
                //set player, make sure they are real and alive
                Player player = Player.m_localPlayer;
                if (player == null || player.IsDead())
                {
                    return;
                }

                if (player.GetSEMan().HaveStatusEffectCategory("LoxCape"))
                {
                    hp *= DragoonCapes.Instance.LoxHealth.Value;
                }
                //sandwiched in here due to laziness
                else if (player.GetSEMan().HaveStatusEffectCategory("dwarfCape"))
                {
                    eitr += DragoonCapes.Instance.GreyEitr.Value;
                }
                else if (player.GetSEMan().HaveStatusEffectCategory("wraithCape"))
                {
                    hp *= 1f - DragoonCapes.Instance.WraithPenalty.Value;
                    stamina *= 1f - DragoonCapes.Instance.WraithPenalty.Value;
                    eitr *= 1f - DragoonCapes.Instance.WraithPenalty.Value;
                }
            }
        }
    }
}
