using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonCapes
{
    //NOTE TO SELF: IF THIS MAKES ENEMIES ATTACKS SPLIT, IT NEEDS TO CHECK IF __instance.attacker = player first!
    internal class LinenCape
    {
        [HarmonyPatch]
        private class LinenPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Attack), "FireProjectileBurst")]
            public static void Attack_FireProjectileBurst_Prefix(Attack __instance)
            {
                //if not a weapon, return
                if (__instance?.GetWeapon() == null || !(__instance.GetWeapon()?.GetDamage()).HasValue)
                {
                    return;
                }
                //if not a cape that effects this, return
                Player player = Player.m_localPlayer;
                bool haveStatus = player.GetSEMan().HaveStatusEffectCategory("LinenCape");
                if (!haveStatus)
                {
                    return;
                }
                //setup booleans
                ItemDrop.ItemData.ItemType? itemType = __instance.GetWeapon()?.m_shared.m_itemType;
                Skills.SkillType? skillType = __instance.GetWeapon()?.m_shared.m_skillType;
                bool twohand = itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon;
                bool magic = skillType == Skills.SkillType.ElementalMagic;
                bool bow = itemType == ItemDrop.ItemData.ItemType.Bow;
                bool bowskill = (skillType == Skills.SkillType.Bows || skillType == Skills.SkillType.Crossbows);
                //Linen Cape
                //splitshot if bow or staff:
                if (haveStatus && ((bow && bowskill) || (twohand && magic)))
                {
                    if (__instance.m_projectileAccuracy < 3f)
                    {
                        __instance.m_projectileAccuracy = 3f;
                    }
                    //Only Apply for things without multishot, also stops looping attacks like frost staff from breaking
                    if (__instance.m_projectiles == 1)
                    {
                        __instance.m_damageMultiplier *= DragoonCapes.Instance.LinenDamageMult.Value;
                        __instance.m_projectiles *= DragoonCapes.Instance.LinenProjs.Value;
                    }
                }
            }
            //Should be patch for Linen +25% stagger, currently just breaks stagger when done
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Attack), "DoMeleeAttack")]
            public static void Attack_DoMeleeAttack_Prefix(Attack __instance)
            {
                if (__instance?.GetWeapon() == null || !(__instance.GetWeapon()?.GetDamage()).HasValue)
                {
                    return;
                }
                Player player = Player.m_localPlayer;
                bool haveStatus = player.GetSEMan().HaveStatusEffectCategory("LinenCape");
                if (!haveStatus)
                {
                    return;
                }
                ItemDrop.ItemData.ItemType? itemType = __instance.GetWeapon()?.m_shared.m_itemType;
                Skills.SkillType? skillType = __instance.GetWeapon()?.m_shared.m_skillType;
                bool weapon = itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon || itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon;
                bool melee = (skillType == Skills.SkillType.Swords || skillType == Skills.SkillType.Polearms || skillType == Skills.SkillType.Axes || skillType == Skills.SkillType.Spears || skillType == Skills.SkillType.Clubs || skillType == Skills.SkillType.Knives);
                if (haveStatus && weapon && melee)
                {
                    __instance.m_staggerMultiplier += DragoonCapes.Instance.LinenStagger.Value;
                }
            }
        } 
    }
}
