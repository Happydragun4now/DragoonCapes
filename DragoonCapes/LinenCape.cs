using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Jotunn.Logger;

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
                //Checking for null values is really important here
                //Make sure player is alive, and exists
                //Make sure the weapon is existant, and the player is wearing the cape
                Player player = Player.m_localPlayer;
                if (player == null || player.IsDead() || __instance?.GetWeapon() == null || !(__instance.GetWeapon()?.GetDamage()).HasValue || !player.GetSEMan().HaveStatusEffectCategory("LinenCape"))
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
                if ((bow && bowskill) || (twohand && magic))
                {
                    if (__instance.m_projectileAccuracy < 3f)
                    {
                        __instance.m_projectileAccuracy = 3f;
                    }
                    //Only Apply for things without multishot, also only do the first round of the burst or else it scales exponentially
                    if (__instance.m_projectileBurstsFired == 0)//OLD: __instance.m_projectiles == 1
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
                //Checking for null values is really important here
                //Make sure player is alive, and exists
                //Make sure the weapon is existant, and the player is wearing the cape
                Player player = Player.m_localPlayer;
                if (player == null || player.IsDead() || __instance?.GetWeapon() == null || !(__instance.GetWeapon()?.GetDamage()).HasValue || !player.GetSEMan().HaveStatusEffectCategory("LinenCape"))
                {
                    return;
                }
                ItemDrop.ItemData.ItemType? itemType = __instance.GetWeapon()?.m_shared.m_itemType;
                Skills.SkillType? skillType = __instance.GetWeapon()?.m_shared.m_skillType;
                bool weapon = itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon || itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon;
                bool melee = (skillType == Skills.SkillType.Swords || skillType == Skills.SkillType.Polearms || skillType == Skills.SkillType.Axes || skillType == Skills.SkillType.Spears || skillType == Skills.SkillType.Clubs || skillType == Skills.SkillType.Knives);
                if (weapon && melee)
                {
                    __instance.m_staggerMultiplier += DragoonCapes.Instance.LinenStagger.Value;
                }
            }
        } 
    }
}
