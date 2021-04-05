using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ECS_Job_CubeDataSystemBase : IComponentData
{
    public  float3 initialPosition;
}
