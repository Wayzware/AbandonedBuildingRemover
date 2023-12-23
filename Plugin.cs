namespace Wayz.CS2.AbandonedBuildingRemover;

[BepInPlugin("Wayz.CS2.AbandonedBuildingRemover", "Abandoned Building Remover", "0.0.1")]
#pragma warning disable BepInEx002
public class AbandonedBuildingRemoverMod : BaseUnityPlugin
#pragma warning restore BepInEx002
{
    public static ManualLogSource GameLogger = null!;

    private void Awake()
    {
        GameLogger = Logger;
        var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID + "_Cities2Harmony");

        var patchedMethods = harmony.GetPatchedMethods();

        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} {MyPluginInfo.PLUGIN_VERSION} is loaded! Patched methods: " + patchedMethods.Count());

        foreach (var patchedMethod in patchedMethods)
        {
            Logger.LogInfo($"Patched method: {patchedMethod.Module.Name}:{patchedMethod.Name}");
        }
    }
}