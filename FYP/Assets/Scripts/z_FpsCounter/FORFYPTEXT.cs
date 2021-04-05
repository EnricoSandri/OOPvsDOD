//using Unity.Jobs;
//using Unity.Burst;
//using Unity.Entities;
//using Unity.Transforms;
//using Unity.Mathematics;
//using Unity.Collections;


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


