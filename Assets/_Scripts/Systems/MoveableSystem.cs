using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

public class MoveableSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref PhysicsVelocity physicsVelocity, in Movable movable) => {
            var step = movable.direction * movable.speed;

            physicsVelocity.Linear = step;

        }).Schedule();
    }
}
