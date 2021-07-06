using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

public class CollisionSystem : SystemBase
{
    private struct CollisionSystemJob : ICollisionEventsJob
    {

        public BufferFromEntity<CollisionBuffer> collisions;

        public void Execute(CollisionEvent collisionEvent)
        {
            if (collisions.HasComponent(collisionEvent.EntityA))
                collisions[collisionEvent.EntityA].Add(new CollisionBuffer() { entity = collisionEvent.EntityB });

            if (collisions.HasComponent(collisionEvent.EntityB))
                collisions[collisionEvent.EntityB].Add(new CollisionBuffer() { entity = collisionEvent.EntityA });
        }
    }

    private struct TriggerSystemJob : ITriggerEventsJob
    {
        public BufferFromEntity<TriggerBuffer> triggers;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (triggers.HasComponent(triggerEvent.EntityA))
                triggers[triggerEvent.EntityA].Add(new TriggerBuffer() { entity = triggerEvent.EntityB });

            if (triggers.HasComponent(triggerEvent.EntityB))
                triggers[triggerEvent.EntityB].Add(new TriggerBuffer() { entity = triggerEvent.EntityA });
        }
    }


    protected override void OnUpdate()
    {
        var pw = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
        var sw = World.GetExistingSystem<StepPhysicsWorld>().Simulation;

        Entities.ForEach((DynamicBuffer<CollisionBuffer> col) => col.Clear()).Run();

        var collisionSystem = new CollisionSystemJob()
        {
            collisions = GetBufferFromEntity<CollisionBuffer>()
        };

        collisionSystem.Schedule(sw, ref pw, this.Dependency).Complete();

        Entities.ForEach((DynamicBuffer<TriggerBuffer> col) => col.Clear()).Run();

        var triggernSystem = new TriggerSystemJob()
        {
            triggers = GetBufferFromEntity<TriggerBuffer>()
        };

        triggernSystem.Schedule(sw, ref pw, this.Dependency).Complete();
    }

}
