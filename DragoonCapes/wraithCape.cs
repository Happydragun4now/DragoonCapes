using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Skills;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using Steamworks;
using System.Numerics;
using Logger = Jotunn.Logger;

namespace DragoonCapes
{
    internal class wraithCape
    {
        //Sourced from Valheim Legends Rogue Code
        //https://github.com/TorannD/ValheimLegends/blob/main/Class_Rogue.cs

        [HarmonyPatch(typeof(Player), "Update", null)]
        public class AbilityInput_Postfix
        {
            public static void Postfix(Player __instance, ref float ___m_maxAirAltitude, ref Rigidbody ___m_body)
            {
                //Process flying stuff if wearing wraithCape
                if (__instance != null && __instance.GetSEMan().HaveStatusEffectCategory("wraithCape"))
                {
                    Process_Input(__instance, ref ___m_body, ref ___m_maxAirAltitude);
                }
            }
        }

        static int flightTime = 0;
        static UnityEngine.Vector3 currentVel = UnityEngine.Vector3.zero;//currentvel is very sus to be using, used like this just bc thats how the character file works as well
        public static void Process_Input(Player player, ref Rigidbody playerBody, ref float m_maxAirAltitude)
        {
            if (flightTime <= DragoonCapes.Instance.WraithFlightTime.Value && !player.IsDead() && !player.InAttack() && !player.IsEncumbered() && !player.InDodge() && !player.IsKnockedBack())
            {
                //Do Flying Stuff
                flightTime++;
                //Adapted from base game debugfly` code
                //Tried player.isRunning(), ZInput.GetButton("Sprint") Running makes slowe elevation unless sprinting
                float num = (ZInput.GetButton("Run") ? (DragoonCapes.Instance.WraithVel.Value * 2.5f) : (DragoonCapes.Instance.WraithVel.Value));
                //Move Direction?
                UnityEngine.Vector3 b = player.GetMoveDir() * num;

                //Up and Down input
                if (ZInput.GetButton("Jump") || ZInput.GetButton("JoyJump"))
                {
                    b.y = num/2.5f;
                }
                else//no else if crouching to go down because you cant crouch when flying and keycode makes errors
                {
                    b.y = 0f - DragoonCapes.Instance.WraithVel.Value/1.5f;
                }

                //reminder that currentVel is not the same on the character typically uses... For some reason saving it seperately from .velocity makes a difference
                currentVel = UnityEngine.Vector3.Lerp(currentVel, b, 0.5f);
                playerBody.velocity = currentVel;

                playerBody.useGravity = false;
                m_maxAirAltitude = playerBody.transform.position.y;
                //Something else is already handling rotation so no need here
                //playerBody.rotation = UnityEngine.Quaternion.RotateTowards(playerBody.transform.rotation, player.GetLookYaw(), 300f /** dt*/);//how do I get dt, patch on a function that gets passed dt regularly?
                playerBody.angularVelocity = UnityEngine.Vector3.zero;
                player.m_eye.rotation = UnityEngine.Quaternion.LookRotation(player.GetLookDir());
            }
            else if (player.IsOnGround() && ZInput.GetButtonDown("Jump"))
            {
                flightTime = 0;
            }
        }


        //Glitchy horizontal Flight only
        /*
        static int flightTime = 0;
        public static void Process_Input(Player player, ref Rigidbody playerBody, ref float altitude)
        {
            //GetButtonDown vs GetButton - makes it so you can hold space?
            if (ZInput.GetButtonDown("Jump") && !player.IsFlying() && !player.IsDead() && !player.InAttack() && !player.IsEncumbered() && !player.InDodge() && !player.IsKnockedBack())
            {
                player.m_flying = true; //problem is due to spam resetting flying?
            }
            if (player.IsFlying())
            {
                if (flightTime >= DragoonCapes.Instance.WraithFlightTime.Value)
                {
                    player.m_flying = false;
                    Logger.LogInfo("Flight Over!");
                }
                //increase flight time
                flightTime++;
                player.FaceLookDirection();//this works for turning character direction
            }
            else if (player.IsOnGround())
            {
                flightTime = 0;
            }
        }
        */

        //old working flight, very vertical slow horizontal
        /*
        static int flightTime = 0;
        public static void Process_Input(Player player, ref Rigidbody playerBody, ref float altitude)
        {
            //GetButtonDown vs GetButton - makes it so you can hold space?
            if (player.GetSEMan().HaveStatusEffectCategory("wraithCape") && ZInput.GetButton("Jump") && !player.IsDead() && !player.InAttack() && !player.IsEncumbered() && !player.InDodge() && !player.IsKnockedBack())
            {
                if (!player.IsOnGround() && flightTime <= DragoonCapes.Instance.WraithFlightTime.Value)
                {
                    UnityEngine.Vector3 velVec = player.GetVelocity();
                    velVec.y = 0f;
                    flightTime++;
                    Logger.LogInfo("Flight Time: " + flightTime);
                    //Logger.LogInfo("Velocity: " + playerBody.velocity);
                    playerBody.velocity = velVec + new UnityEngine.Vector3(0, DragoonCapes.Instance.WraithAlt.Value, 0f);
                    altitude = 0;
                }
            }
            else if (player.IsOnGround())
            {
                flightTime = 0;
            }
        }
        */
    }
}
