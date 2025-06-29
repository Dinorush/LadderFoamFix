using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace LadderFoamFix
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.0.1")]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    internal sealed class EntryPoint : BasePlugin
    {
        public const string MODNAME = "LadderFoamFix";

        public override void Load()
        {
            new Harmony(MODNAME).PatchAll();
            GTFO.API.LevelAPI.OnLevelCleanup += FoamHitboxPatch.OnCleanup;
            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}