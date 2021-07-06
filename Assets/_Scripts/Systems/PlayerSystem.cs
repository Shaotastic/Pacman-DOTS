using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Entities.WithAll<Player>().ForEach((ref Movable mov, in Player player) => {
            mov.direction = new float3(x, 0, y);
        }).Run();
    }
}
