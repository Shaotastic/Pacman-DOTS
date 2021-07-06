using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Enemy : IComponentData
{
    public float3 previousCell;

    public float3 nextCell;
    
}
