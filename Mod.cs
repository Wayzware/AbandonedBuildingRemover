using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace AbandonedBuildingRemover
{
    public class Mod : IMod
    {
        public static ILog Log = LogManager.GetLogger($"{nameof(AbandonedBuildingRemover)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                Log.Info($"Current mod asset at {asset.path}");

            //updateSystem.UpdateAfter<AbandonedBuildingRemoverSystem>(SystemUpdatePhase.Deserialize);
            updateSystem.UpdateAfter<AbandonedBuildingRemoverSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            Log.Info(nameof(OnDispose));
        }
    }
}
