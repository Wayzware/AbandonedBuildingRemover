using Game;
using Game.Buildings;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace AbandonedBuildingRemover;
public partial class AbandonedBuildingRemoverSystem : GameSystemBase
{
    private EntityQuery _abandonedBuildingQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        _abandonedBuildingQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All =
            [
                ComponentType.ReadWrite<Abandoned>(),
                ComponentType.ReadWrite<Building>()
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
            EntityManager.AddComponent<Deleted>(entity);
        }
    }
}
