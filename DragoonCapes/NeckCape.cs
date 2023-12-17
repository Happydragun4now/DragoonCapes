using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonCapes
{
    internal class NeckCape
    {
        //Neck Waterproof Patch
        //thanks to RandyKnapp for inspiring this method with epicLoot
        [HarmonyPatch(typeof(SEMan), "AddStatusEffect", new Type[]
        {
        typeof(int),
        typeof(bool),
        typeof(int),
        typeof(float)
        })]
        public static class Waterproof_SEMan_AddStatusEffect_Patch
        {
            public static bool Prefix(SEMan __instance, int nameHash)
            {
                //Wet Can't be applied if you have the NeckCape
                if (nameHash == "Wet".GetHashCode())
                {
                    if (__instance.HaveStatusEffectCategory("NeckCape") || __instance.HaveStatusEffectCategory("surtlingCape"))//surtling cape added here bc lazy
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
