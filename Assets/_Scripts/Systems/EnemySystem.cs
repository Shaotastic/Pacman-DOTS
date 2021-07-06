using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class EnemySystem : SystemBase
{
    private Random rand = new Random(5342);
    protected override void OnUpdate()
    {
        //Unity.Mathematics.Random rand = new Unity.Mathematics.Random();
        var raycaster = new MovementRayCast() { pw =  World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld };
        rand.NextInt();
        var rngTemp = rand;

        Entities.ForEach((ref Movable mov, ref Enemy enemy, in Translation translation) =>
        {
            if(math.distance(translation.Value, enemy.previousCell) > 0.9f)
            {
                enemy.previousCell.x = (float)System.Math.Round(translation.Value.x * 2, System.MidpointRounding.AwayFromZero) / 2;
                enemy.previousCell.y = 0;
                enemy.previousCell.z = (float)System.Math.Round(translation.Value.z * 2, System.MidpointRounding.AwayFromZero) / 2;

                var validDirection = new NativeList<float3>(Allocator.Temp);

                if(!raycaster.CheckRay(translation.Value, new float3(0, 0, -1), mov.direction))
                    validDirection.Add(new float3(0, 0, -1));

                if (!raycaster.CheckRay(translation.Value, new float3(0, 0, 1), mov.direction))
                    validDirection.Add(new float3(0, 0, 1));

                if (!raycaster.CheckRay(translation.Value, new float3(-1, 0, 0), mov.direction))
                    validDirection.Add(new float3(-1, 0, 0));

                if (!raycaster.CheckRay(translation.Value, new float3(1, 0, 0), mov.direction))
                    validDirection.Add(new float3(1, 0, 0));

                mov.direction = validDirection[rngTemp.NextInt(validDirection.Length)];

                enemy.nextCell = translation.Value + (mov.direction * 0.9f);

                validDirection.Dispose();
            }

        }).ScheduleParallel();
    }

    private struct MovementRayCast
    {
        [ReadOnly] public PhysicsWorld pw;

        public bool CheckRay(float3 pos, float3 direction, float3 currentDirection)
        {
            if (direction.Equals(-currentDirection))
                return true;

            var ray = new RaycastInput()
            {
                Start = pos,
                End = pos + (direction * 0.9f),
                Filter = new CollisionFilter()
                {
                    BelongsTo = 1u << 1,
                    CollidesWith = 1u << 2,
                    GroupIndex = 0
                }
            };
            
            return pw.CastRay(ray);
        }
    }
}
