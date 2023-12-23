using Game.Buildings;
using Game.Simulation;

namespace Wayz.CS2.AbandonedBuildingRemover.Patches;

[HarmonyPatch(typeof(DeathCheckSystem), "OnCreate")]
public static class AbandonedBuildingRemoverSystemInjector
{
    [HarmonyPrefix]
    public static bool OnCreate_Prefix(DeathCheckSystem __instance)
    {
        __instance.World.GetOrCreateSystemManaged<AbandonedBuildingRemoverSystem>();
        __instance.World.GetOrCreateSystemManaged<Game.UpdateSystem>().UpdateAt<AbandonedBuildingRemoverSystem>(Game.SystemUpdatePhase.GameSimulation);
        return true;
    }
}
