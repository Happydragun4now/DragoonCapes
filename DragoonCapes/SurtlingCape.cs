using HarmonyLib;
using Jotunn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Logger = Jotunn.Logger;

namespace DragoonCapes
{
    [HarmonyPatch]
    internal class SurtlingCape
    {
        private static float burnTimer = 0f;
        private static float damageInterval = 1f;
        //Update the Burn
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SE_Burning), "UpdateStatusEffect")]//Start the fire on equip and never let it expire?!
        public static void BurningUpdate_Prefix(ref SE_Burning __instance, float dt)
        {
            //only do this prefix function stuff if the player has the surtlingBelt on
            Player player = Player.m_localPlayer;
            //Logger.LogInfo("__instance vs player burning status:\n" + __instance + "\n" + player.GetSEMan().GetStatusEffect("Burning".GetHashCode()));
            if (player != null && __instance == player.GetSEMan().GetStatusEffect("Burning".GetHashCode()) && player.GetSEMan().HaveStatusEffectCategory("surtlingCape"))
            {
                //Time-gate the burn to not happen every tick
                burnTimer += dt;
                if ((burnTimer >= damageInterval))
                {
                    burnTimer = 0f;
                    //fire dmg based on currHP
                    __instance.AddFireDamage(Mathf.Max(0.015f * player.GetHealth(), 0.3f));
                    //Logger.LogInfo("Added Fire Damage: " + Mathf.Max(0.02f * player.GetHealth(), 0.6f));
                }
                return;
            }
        }

        //Damage Bonus when burning
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "Damage")]
        private static void FireDamage_Prefix(HitData hit)
        {
            Player player = Player.m_localPlayer;
            if (hit.GetAttacker() == player && player.GetSEMan().HaveStatusEffectCategory("surtlingCape") && player.GetSEMan().HaveStatusEffect("Burning".GetHashCode()))
            {  
                hit.m_damage.m_fire += hit.GetTotalDamage() * DragoonCapes.Instance.SurtlingDamageMult.Value;
            }
        }

        //Patches that reapply fire if it is somehow removed
        //RemoveStatusEffect doesnt work bc the devs made 2 functions with the same name
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SEMan), "RemoveAllStatusEffects")]
        private static void ClearAllStatus_Prefix()
        {
            Player player = Player.m_localPlayer;
            if (player.GetSEMan().HaveStatusEffectCategory("surtlingCape") && !player.GetSEMan().HaveStatusEffect("Burning".GetHashCode()))
            {
                //Logger.LogInfo("Dealing surtling bonus fire damage!");
                HitData selfBurn = new HitData();
                selfBurn.m_damage.m_fire = 100f + player.GetBodyArmor();//this needs to be high enough to evade armor, therefore its the armor value!
                player.Damage(selfBurn);
            }
        }
        //Tailored to work with magicplugin healing staff lol
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "UpdateStats", new Type[] { typeof(float) })]
        public static void UpdateStats_Postfix(Player __instance)
        {
            //If you have the cape on and aren't burning fix that
            if (!__instance.GetSEMan().HaveStatusEffect("healstaff_cooldown") && __instance.GetSEMan().HaveStatusEffectCategory("surtlingCape") && !__instance.GetSEMan().HaveStatusEffect("Burning"))
            {
                HitData selfBurn = new HitData();
                selfBurn.m_damage.m_fire = 100f + __instance.GetBodyArmor();//this needs to be high enough to evade armor, therefore its the armor value!
                __instance.Damage(selfBurn);
            }
        }

        /*
        //Start the burn when equipped, not needed due to the new above function
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Humanoid), "EquipItem")]
        private static void BurnStart_Postfix(ItemDrop.ItemData item)
        {
            //Logger.LogInfo("Equipped Item: "+ item.m_shared.m_name);
            if (item.m_shared.m_name == "Surtling Cape")
            {
                Player player = Player.m_localPlayer;
                try { player.GetSEMan().RemoveStatusEffect(player.GetSEMan().GetStatusEffect("Wet".GetHashCode())); }
                catch { }
                HitData selfBurn = new HitData();
                selfBurn.m_damage.m_fire = 100f + player.GetBodyArmor();//this needs to be high enough to evade armor, therefore its the armor value!
                player.Damage(selfBurn);

                //Logger.LogInfo("Burn started with: " + selfBurn.m_damage.m_fire + " Dmg");
            }
        }
        */
    }
}
