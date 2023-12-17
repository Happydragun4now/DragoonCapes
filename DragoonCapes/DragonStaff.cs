using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonCapes
{
    internal class DragonStaff
    {
        /*
        [HarmonyPatch]
        private class DragonPatch
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
                string WeaponName = __instance.GetWeapon()?.m_shared.m_name;
                //splitshot if bow or staff:
                if (WeaponName == "Staff of dragon's breath")
                {
                    __instance.m_loopingAttack = true;
                    __instance.m_damageMultiplier *= 0.13f;
                    __instance.m_projectiles *= 5;
                    __instance.m_projectileVel *= 0.4f;
                    __instance.m_projectileAccuracy = 7f;

                }
            }
        } 
        */
    }
}
