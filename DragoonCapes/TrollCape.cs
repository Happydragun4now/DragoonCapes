using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class TrollCape
    {
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (hit.GetAttacker() == player && player.GetSEMan().HaveStatusEffectCategory("TrollCape") && (player.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Knives))
            {
                //Damage can go over 1 if sneak is high enough, wearing troll cape, and using a knife
                hit.m_damage.Modify(Math.Max(1, DragoonCapes.Instance.TrollDamageMult.Value * player.GetSkillLevel(Skills.SkillType.Sneak) / 100f));
            }
        }
    }
}
