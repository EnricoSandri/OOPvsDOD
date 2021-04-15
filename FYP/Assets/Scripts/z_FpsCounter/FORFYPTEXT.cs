/*
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;


//    public struct Job : IJob
//    {
//        [ReadOnly] public NativeArray<float> inputs;
//        [WriteOnly] public NativeArray<float> outputs;
//        public void Execute()
//        {
//            for(int i = 0 ; i < inputs.Length; i++) {
//                outputs[i] = inputs[i];
//            }
//        }
//    }
    
//    public struct Job2 : IJobParallelFor
//    {
//        [ReadOnly] public NativeArray<float> input;
//        [WriteOnly] public NativeArray<float> output;
//        public void Execute()
//        {
//            for(int i = 0 ; i < input.Length; i++) {
//                output[i] = input[i];
//            }
//        }
//    }
class  MyClass : MonoBehaviour
{
    private void Start()
    {
        //1
        EntityManager  entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //2
        Entity entity = entityManager.CreateEntity();
    }
    
    
  

}
public struct MoveForward : IComponentData
{
    public float speed;
}


{
    public struct Movement : IComponentData 
    {
        public float speed ;
    }
    public struct Translation : IComponentData 
    {
        public float3 Value ; 
    }
    public class MovementSystem : ComponentSystem 
    {
        public NativeArray<Translation> translation; 
        public NativeArray<Movement> movement;
        protected override void OnUpdate ( ) 
        {
            for ( int i = 0; i < translation.Length; i++) 
            {
              float3 position = translation.Value[i]; 
              position.x += movement[i].speed;
              translation.Value[i] = position; 
            }
        } 
    }
    



}
public class MovementComponent : MonoBehaviour
{
    private float speed;
    void Update()
    {
        Vector3 position = transform.position;
        position.x += speed;
        transform.position = position;
    }
}

public struct Job : IJobParallelFor 
{
    [ReadOnly] public NativeArray<float> input; 
    [WriteOnly] public NativeArray<float > output;
    public void Execute(int index)
    {
        for (int i = 0; i < input.Length; i++) 
        {
            output [ i ] = input [ i ] ; 
        } 
    }
}

public struct parallelJob
{
    [ReadOnly] public NativeArray<float> input; 
    [WriteOnly] public NativeArray<float > output;


}


// Create input job data
var src = new NativeArray<float>(1000, Allocator.TempJob);
// Create output job data
var dst = new NativeArray<float>(1000, Allocator.TempJob);
var parallelDst = new NativeArray<float>(1000, Allocator.TempJob);

// Create the jobs and fill data
var job = new Job
{
    input = src, output = dst
};
var parallelJob = new parallelJob
{
    input = src,
    output = parallelDst
};

// Schedule the jobs
var handle = job.Schedule();
var parallelHandle = parallelJob.Schedule(src.Length, 64);

// Wait for the j bs to complete
handle.Complete(); 
parallelHandle.Complete();

// Free the memory in the native containers
src. Dispose(); 
dst. Dispose(); 
parallelDst.Dispose();
*/
