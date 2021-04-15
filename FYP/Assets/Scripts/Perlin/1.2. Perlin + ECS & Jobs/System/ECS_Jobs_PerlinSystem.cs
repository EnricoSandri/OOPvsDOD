using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
public class ECS_Jobs_PerlinSystem : JobComponentSystem
{
    private ECS_Jobs_PerlinManager perlinManager;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!perlinManager)
        {
            perlinManager = ECS_Jobs_PerlinManager.instance;
        }
        if (perlinManager&& perlinManager.IsActive)
        {
            float strenght1 = perlinManager.strenght1;
            float scale1 = perlinManager.scale1;
            float strenght2 = perlinManager.strenght2;
            float scale2 = perlinManager.scale2;
            float strenght3 = perlinManager.strenght3;
            float scale3 = perlinManager.scale3;
            float moveVal = perlinManager.moveValue.value;
            strenght1 = moveVal;

            var jobHandle = Entities.ForEach((ref Translation position, ref ECS_Jobs_CubeData cubeData) =>{

                    var vertex = cubeData.initialPosition;

                    // Perlin Noise
                    var perlin1 = Mathf.PerlinNoise(vertex.x * scale1, vertex.z * scale1) * strenght1;
                    var perlin2 = Mathf.PerlinNoise(vertex.x * scale2, vertex.z * scale2) * strenght2;
                    var perlin3 = Mathf.PerlinNoise(vertex.x * scale3, vertex.z * scale3) * strenght3;
                    var height = perlin1 + perlin2 + perlin3;

                    position.Value = new Vector3(vertex.x, height, vertex.z);

            }).Schedule(inputDeps);
            jobHandle.Complete();
        }
        return inputDeps;
    }
}
