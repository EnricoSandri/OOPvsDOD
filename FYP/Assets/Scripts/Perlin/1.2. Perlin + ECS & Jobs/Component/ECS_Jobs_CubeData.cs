using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ECS_Jobs_CubeData : IComponentData
{
    public  float3 initialPosition;
}
