using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Areas;
using Game.Net;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace AbandonedBuildingRemover
{
    public partial class AbandonedBuildingRemoverSystem : GameSystemBase
    {
        private EntityQuery _abandonedBuildingQuery;
        private EndFrameBarrier _endFrameBarrier;

        protected override void OnCreate()
        {
            base.OnCreate();

            _abandonedBuildingQuery = GetEntityQuery(new EntityQueryDesc
            {
                All  = new[] { ComponentType.ReadOnly<Abandoned>() },
                None = new[] { ComponentType.ReadOnly<Deleted>(), ComponentType.ReadOnly<Temp>() }
            });

            _endFrameBarrier = World.GetOrCreateSystemManaged<EndFrameBarrier>();

            RequireForUpdate(_abandonedBuildingQuery);
        }

        protected override void OnUpdate()
        {
#if USE_BURST
            var job = new AbandonedBuildingRemoverJob
            {
                m_entityTypeHandle    = SystemAPI.GetEntityTypeHandle(),
                m_entityCommandBuffer = _endFrameBarrier.CreateCommandBuffer(),
                m_abandonedChunks     = _abandonedBuildingQuery.ToArchetypeChunkListAsync(World.UpdateAllocator.ToAllocator, out var deps),
                m_subAreaLookup       = GetBufferLookup<SubArea>(true),
                m_subNetLookup        = GetBufferLookup<SubNet>(true),
                m_subLaneLookup       = GetBufferLookup<SubLane>(true)
            };

            var handle = job.Schedule(JobHandle.CombineDependencies(Dependency, deps));
            _endFrameBarrier.AddJobHandleForProducer(handle);
            Dependency = handle;
#else
            var abandoned = _abandonedBuildingQuery.ToEntityArray(Allocator.Temp);
            try
            {
                foreach (var entity in abandoned)
                {
                    // Delete child areas
                    if (EntityManager.TryGetBuffer<SubArea>(entity, out var subAreas))
                    {
                        foreach (var entry in subAreas)
                            EntityManager.AddComponent<Deleted>(entry.m_Area);
                    }

                    // Delete child nets
                    if (EntityManager.TryGetBuffer<SubNet>(entity, out var subNets))
                    {
                        foreach (var entry in subNets)
                            EntityManager.AddComponent<Deleted>(entry.m_SubNet);
                    }

                    // Delete child lanes
                    if (EntityManager.TryGetBuffer<SubLane>(entity, out var subLanes))
                    {
                        foreach (var entry in subLanes)
                            EntityManager.AddComponent<Deleted>(entry.m_SubLane);
                    }

                    // Finally delete the building entity itself
                    EntityManager.AddComponent<Deleted>(entity);
                }
            }
            finally
            {
                if (abandoned.IsCreated) abandoned.Dispose(); // <-- fix leak
            }
#endif
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
            => phase == SystemUpdatePhase.GameSimulation ? 16 : 1;

#if USE_BURST
        [BurstCompile]
        private struct AbandonedBuildingRemoverJob : IJob
        {
            public EntityCommandBuffer m_entityCommandBuffer;
            public NativeList<ArchetypeChunk> m_abandonedChunks;

            [ReadOnly] public EntityTypeHandle m_entityTypeHandle;
            [ReadOnly] public BufferLookup<SubArea> m_subAreaLookup;
            [ReadOnly] public BufferLookup<SubNet>  m_subNetLookup;
            [ReadOnly] public BufferLookup<SubLane> m_subLaneLookup;

            public void Execute()
            {
                for (int i = 0; i < m_abandonedChunks.Length; i++)
                {
                    var chunk = m_abandonedChunks[i];
                    var entities = chunk.GetNativeArray(m_entityTypeHandle);

                    for (int j = 0; j < entities.Length; j++)
                    {
                        var entity = entities[j];

                        if (m_subAreaLookup.TryGetBuffer(entity, out var subAreas))
                            foreach (var e in subAreas) m_entityCommandBuffer.AddComponent<Deleted>(e.m_Area);

                        if (m_subLaneLookup.TryGetBuffer(entity, out var subLanes))
                            foreach (var e in subLanes) m_entityCommandBuffer.AddComponent<Deleted>(e.m_SubLane);

                        if (m_subNetLookup.TryGetBuffer(entity, out var subNets))
                            foreach (var e in subNets) m_entityCommandBuffer.AddComponent<Deleted>(e.m_SubNet);

                        m_entityCommandBuffer.AddComponent<Deleted>(entity);
                    }
                }
            }
        }
#endif
    }
}
