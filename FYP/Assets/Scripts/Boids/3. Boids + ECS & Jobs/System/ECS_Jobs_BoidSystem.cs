using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public class ECS_Jobs_BoidSystem : JobComponentSystem
{
    // Reference to the Boid manager instance.
    private Ecs_Jobs_BoidManager manager;

    // JobHandel Update
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Check if BoidManager exsists, if not creat one.
        if (!manager)
        {
            manager = Ecs_Jobs_BoidManager.instance;
        }
        // Run only if there is a manager.
        if (manager)
        {
            //Find and store a refrence to all the entities with 
            //BoidComponent and a local to world component by doing a query on the archetype
            EntityQuery boidEntityQuery =
                GetEntityQuery(ComponentType.ReadOnly<ECS_Jobs_BoidComponent>(),
                               ComponentType.ReadOnly<LocalToWorld>());
            
            //Array Containing the entites from query // to dispose.
            NativeArray<Entity> entities = boidEntityQuery.ToEntityArray(Allocator.TempJob);
            //Array containing the LocalToWorld of the entitier
            NativeArray<LocalToWorld> LTW_Array = boidEntityQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

            // Job arrays, deallocated after job completion. 
            NativeArray<BoidEntityWithLTW> boidEntityWithLTW = new NativeArray<BoidEntityWithLTW>(entities.Length, Allocator.TempJob);
            //Array containg all the boids next position
            NativeArray<float4x4> nextBoidPositionArray = new NativeArray<float4x4>(entities.Length, Allocator.TempJob);

            // Itarate through the entites, for each entitie in the array assign the valuse ion the struct.
            for (int i = 0; i < entities.Length; i++)
            {
                boidEntityWithLTW[i] = new BoidEntityWithLTW
                {
                    BoidEntity = entities[i],
                    LTW = LTW_Array[i]
                };
            }

            entities.Dispose();
            LTW_Array.Dispose();

            //Do the Jobs.
            BoidEntityJob boidEntityJob = new BoidEntityJob
            {
                otherBoidEntities = boidEntityWithLTW,
                nextBoidPosition = nextBoidPositionArray,
                speed = manager.speed,
                deltaTime = Time.DeltaTime,
                scale = manager.scale,
                perceptionRadius = manager.perceptionRadius,
                separationMultiplier = manager.separationMultiplier,
                cohesionMultiplier = manager.cohesionMultiplier,
                alignmentMultiplier = manager.alignmentMultiplier,
                enclosureSize = manager.enclosureSize,
                wallAvoidanceMultiplier = manager.wallAvoidanceMultiplier,
                turnAwayFromWallDistance = manager.turnAwayFromWallDistance,

            };

            BoidEntityMovementJob boidEntityMovementJob = new BoidEntityMovementJob
            {
                nextBoidPosition = nextBoidPositionArray
            };

            JobHandle boidJobHandle = boidEntityJob.Schedule(this, inputDeps);
            return boidEntityMovementJob.Schedule(this, boidJobHandle);
        }
        else { return inputDeps; }
    }

    
    private struct BoidEntityWithLTW
    {
        public Entity BoidEntity;
        public LocalToWorld LTW;
    }

    //// THE JOBS//// 

    //https://www.red-gate.com/simple-talk/dotnet/c-programming/introducing-the-unity-job-system/

    //Job on enetites with boidcomponent 
    [RequireComponentTag(typeof(ECS_Jobs_BoidComponent))]
    //Job for each enetity with reference LTW 
    private struct BoidEntityJob : IJobForEachWithEntity<LocalToWorld>
    {
        // Job required varibles 
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<BoidEntityWithLTW> otherBoidEntities; // Array for other boids with LTW component.
        [WriteOnly] public NativeArray<float4x4> nextBoidPosition; // Array for next positon. 

        [ReadOnly] public float speed;
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float3 scale;
        [ReadOnly] public float perceptionRadius;
        [ReadOnly] public float separationMultiplier;
        [ReadOnly] public float cohesionMultiplier;
        [ReadOnly] public float alignmentMultiplier;
        [ReadOnly] public float enclosureSize;
        [ReadOnly] public float wallAvoidanceMultiplier;
        [ReadOnly] public float turnAwayFromWallDistance;

        //Job execute function, all job logic in here. (IJobForEachWithEntity works like a forloop)
        public void Execute(Entity boidEntity, int boidIndex, [ReadOnly] ref LocalToWorld boidLTW)
        {
            //Store the boidsEntity position
            float3 boidPosition = boidLTW.Position;

            //Reset all the rule values to 0.0.0.
            float3 seperationSum = float3.zero;
            float3 positionSum = float3.zero;
            float3 headingSum = float3.zero;

            int nearbyBoids = 0; // Boids present in the  preseption radius.

            //Itarate through the otherboidentities array 
            for (int otherBoidIndex = 0; otherBoidIndex < otherBoidEntities.Length; otherBoidIndex++)
            {
                if(boidEntity != otherBoidEntities[otherBoidIndex].BoidEntity) // all the other boids apart this.
                {
                    //Reference to other boids position.
                    float3 otherBoidLTW = otherBoidEntities[otherBoidIndex].LTW.Position; //LTW
                    //calculate the float distance from this boid to the other.
                    float otherBoidDistance = math.length(boidPosition - otherBoidLTW);

                    // check if the otherboid is within the boids perception radius. 
                    if (otherBoidDistance < perceptionRadius) // Get all the values  of boids within the radius
                    {
                        // seperation => prevents boids from crowding. negative force needed to move away from the other boid.
                        seperationSum += -(otherBoidLTW - boidPosition) * (1f / math.max(otherBoidDistance, .0001f));
                        // Get the position of the other boid
                        positionSum += otherBoidLTW;
                        // Get the heading of the other boid.
                        headingSum += otherBoidEntities[otherBoidIndex].LTW.Forward;

                        // add boid to the number of nearby boids.
                        nearbyBoids++;
                    }
                }
            }
            // Reference to force to apply to the velocity of the boid.
            float3 force = float3.zero;

            // If there is a boid present in the radius,
            //find the average and apply to the force the ruleSums values to this boid.
            if (nearbyBoids > 0)
            {
                force += (seperationSum / nearbyBoids) * separationMultiplier;
                force += ((positionSum / nearbyBoids) - boidPosition) * cohesionMultiplier;
                force += (headingSum / nearbyBoids) * alignmentMultiplier;
            }

            // check for Wall avoidance, if boid is near the wall turn the boid around
            if (math.min
                (math.min((enclosureSize / 2f) - math.abs(boidPosition.x), (enclosureSize / 2f) - math.abs(boidPosition.y)),
                    (enclosureSize / 2f) - math.abs(boidPosition.z)) < turnAwayFromWallDistance)
            {
                force += -math.normalize(boidPosition) * wallAvoidanceMultiplier;
            }


            //Store the force values gathered to the velocity of the boid
            float3 boidVelocity = boidLTW.Forward * speed;
            boidVelocity += force * deltaTime;
            boidVelocity = math.normalize(boidVelocity) * speed;

            //store the next postition of the boid at index
            nextBoidPosition[boidIndex] = float4x4.TRS(
                boidLTW.Position + boidVelocity * deltaTime,
                quaternion.LookRotationSafe(boidVelocity, boidLTW.Up),
                scale
                );
        }
    }

    //Apply the next position to the boids.
    //Job on enetites with boidcomponent 
    [RequireComponentTag(typeof(ECS_Jobs_BoidComponent))]
    //Job for each enetity with reference LTW 
    private struct BoidEntityMovementJob : IJobForEachWithEntity<LocalToWorld>
    {
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<float4x4> nextBoidPosition;
       
        public void Execute(Entity boid, int boidIndex, [WriteOnly] ref LocalToWorld LTW)
        {
            LTW.Value = nextBoidPosition[boidIndex];
        }
    }
}
