using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ECS_Jobs_Burst_CubeData : IComponentData
{
    public  float3 initialPosition;
}
