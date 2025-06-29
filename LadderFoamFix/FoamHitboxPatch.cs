using Agents;
using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LadderFoamFix
{
    [HarmonyPatch(typeof(ES_StuckInGlue))]
    internal static class FoamHitboxPatch
    {
        private static readonly Dictionary<IntPtr, Vector3> _startPosCache = new();

        public static void OnCleanup() => _startPosCache.Clear();

        [HarmonyPatch(nameof(ES_StuckInGlue.CommonEnter))]
        [HarmonyPostfix]
        private static void DisableHitbox(ES_StuckInGlue __instance)
        {
            var ai = __instance.m_ai;
            ai.m_behaviour.m_updatebehaviour = float.MaxValue;
            var navAgent = ai.m_navMeshAgent;
            if (navAgent.isOnOffMeshLink)
                _startPosCache[__instance.Pointer] = navAgent.currentOffMeshLinkData.startPos;
            navAgent.enabled = false;
        }

        [HarmonyPatch(nameof(ES_StuckInGlue.CommonExit))]
        [HarmonyPostfix]
        private static void EnableHitbox(ES_StuckInGlue __instance)
        {
            var ai = __instance.m_ai;
            ai.m_behaviour.m_updatebehaviour = 0f;
            var state = __instance.m_locomotion.m_currentState.m_stateEnum;
            if (state != ES_StateEnum.Dead && state != ES_StateEnum.Hibernate)
            {
                var navAgent = ai.m_navMeshAgent;
                navAgent.enabled = true;
                if (_startPosCache.Remove(__instance.Pointer, out var startPos))
                {
                    var owner = __instance.m_enemyAgent;
                    owner.Position = startPos;
                    owner.transform.position = startPos;
                    ai.NavmeshAgentWarp(startPos);
                    ai.NavmeshAgentGoal = startPos;
                }
            }
        }
    }
}
