using Unity.Entities;

[GenerateAuthoringComponent]
public struct CollisionBuffer : IBufferElementData
{
    public Entity entity;
}
