using Enemies;
using HarmonyLib;

namespace LadderFoamFix
{
    [HarmonyPatch(typeof(ES_StuckInGlue))]
    internal static class FoamHitboxPatch
    {
        [HarmonyPatch(nameof(ES_StuckInGlue.CommonEnter))]
        [HarmonyPostfix]
        private static void DisableHitbox(ES_StuckInGlue __instance)
        {
            var ai = __instance.m_ai;
            ai.m_behaviour.m_updatebehaviour = float.MaxValue;
            ai.SetNavMeshAgent(false);
        }

        [HarmonyPatch(nameof(ES_StuckInGlue.CommonExit))]
        [HarmonyPostfix]
        private static void EnableHitbox(ES_StuckInGlue __instance)
        {
            var ai = __instance.m_ai;
            ai.m_behaviour.m_updatebehaviour = 0f;
            var state = __instance.m_locomotion.m_currentState.m_stateEnum;
            if (state != ES_StateEnum.Dead && state != ES_StateEnum.Hibernate)
                ai.SetNavMeshAgent(true);
        }
    }
}
