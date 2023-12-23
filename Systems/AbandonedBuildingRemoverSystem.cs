using Game;
using Game.Buildings;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace Wayz.CS2.AbandonedBuildingRemover;
public class AbandonedBuildingRemoverSystem : GameSystemBase
{
    private EntityCommandBuffer _commandBuffer;

    private EntityQuery _abandonedBuildingQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        _commandBuffer = base.World.GetOrCreateSystemManaged<EndFrameBarrier>().CreateCommandBuffer();
        _abandonedBuildingQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All =
            [
                ComponentType.ReadWrite<Abandoned>()
            ],
            None =
            [
                ComponentType.ReadOnly<Deleted>(),
                ComponentType.ReadOnly<Temp>()
            ]
        });

        RequireForUpdate(_abandonedBuildingQuery);
    }

    protected override void OnUpdate()
    {

        var abandonedBuildings = _abandonedBuildingQuery.ToEntityArray(Allocator.Temp);
        foreach (var entity in abandonedBuildings)
        {
            _commandBuffer.AddComponent<Deleted>(entity);
        }
    }
}
