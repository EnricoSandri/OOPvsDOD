using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Job_Burst_CubeDataSystemBase : IComponentData
{
    public  float3 initialPosition;
}
