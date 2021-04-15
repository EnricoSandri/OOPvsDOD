using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
public class ECS_BoidsSystem : ComponentSystem
{
    // Reference to the Boid manager instance.
    private ECS_BoidManager manager;

    // Update loop
    protected override void OnUpdate()
    {
        //Check if BoidManager exists, if not creat one.
        if (!manager)
        {
            manager = ECS_BoidManager.instance;
        }
        // Run only if there is a manager.
        if (manager && manager.IsActive)
        {
            //Find and store a reference to all the entities with 
            //BoidComponent and a local to world component by doing a query on the archetype
            EntityQuery boidEntityQuery = 
                GetEntityQuery(ComponentType.ReadOnly<BoidComponent>(),
                               ComponentType.ReadOnly<LocalToWorld>());
           
            //Array containing all the boids next position////////////////////////////////////////////// 
            NativeArray<float4x4> nextBoidPosition =
                new NativeArray<float4x4>(boidEntityQuery.CalculateEntityCount(), Allocator.Temp);

            int boidIndex = 0;

            //Iterate over a set of  entities, this case boids with ref LocalToWorld. (Like a foreach loop)
            Entities.WithAll<BoidComponent>().ForEach((Entity boidEntity, ref LocalToWorld boidLTW) => 
            {
                //Store the boidsEntity position
                float3 boidPosition = boidLTW.Position;
                
                //Reset all the rule values to 0.0.0.
                float3 seperationSum = float3.zero;
                float3 positionSum = float3.zero;
                float3 headingSum = float3.zero;

                int nearbyBoids = 0; // Boids present in the  perception radius.
                
                //iterate over other boids with ref LocalToWorld(Like a foreach loop)
                Entities.WithAll<BoidComponent>().ForEach((Entity otherBoidEntity, ref LocalToWorld otherboidLTW) =>
                {
                   
                    if (boidEntity != otherBoidEntity) // all the other boids apart this.
                    {
                        float otherBoidDistance = math.length(boidPosition - otherboidLTW.Position);// the float distance from this boid to the other.

                        // check if the otherboid is within the boids perception radius. 
                        if (otherBoidDistance < manager.perceptionRadius)
                        {
                            // Get all the values  of boids within the radius

                            // separation => prevents boids from crowding. negative force needed to move away from the other boid.
                            seperationSum += -(otherboidLTW.Position - boidPosition) * (1f / math.max(otherBoidDistance, .0001f));
                            // Get the position of the other boid
                            positionSum += otherboidLTW.Position;
                            // Get the heading of the other boid.
                            headingSum += otherboidLTW.Forward;

                            // add boid to the number of nearby boids.
                            nearbyBoids++;
                        }
                    }
                });
                // Reference to force to apply to the velocity of the boid.
                float3 force = float3.zero;

                // If there is a boid present in the radius,
                //find the average and apply to the force the ruleSums values to this boid.
                if (nearbyBoids > 0)
                {
                    force += (seperationSum / nearbyBoids) * manager.separationMultiplier;
                    force += ((positionSum / nearbyBoids)- boidPosition) * manager.cohesionMultiplier;
                    force += (headingSum / nearbyBoids) * manager.alignmentMultiplier;
                }

                // check for Wall avoidance, if boid is near the wall turn the boid around
                if (math.min
                    (math.min((manager.enclosureSize / 2f) - math.abs(boidPosition.x),(manager.enclosureSize / 2f) - math.abs(boidPosition.y)),
                        (manager.enclosureSize / 2f) - math.abs(boidPosition.z)) < manager.turnAwayFromWallDistance)
                {
                    force += -math.normalize(boidPosition) * manager.wallAvoidanceMultiplier;
                }

                //Store the force values gathered to the velocity of the boid
                float3 boidVelocity = boidLTW.Forward * manager.speed;
                boidVelocity += force * Time.DeltaTime;
                boidVelocity = math.normalize(boidVelocity) * manager.speed;

                //store the next position of the boid at index
                nextBoidPosition[boidIndex] = float4x4.TRS(
                    boidLTW.Position + boidVelocity * Time.DeltaTime,
                    quaternion.LookRotationSafe(boidVelocity, boidLTW.Up),
                    manager.scale
                    );

                boidIndex++;
            });

            boidIndex = 0;
            
            //Apply the next position to the boids.
            Entities.WithAll<BoidComponent>().ForEach((Entity boid, ref LocalToWorld localToWorld) => 
            {
                localToWorld.Value = nextBoidPosition[boidIndex];
                boidIndex++;
            });

           nextBoidPosition.Dispose();
        }
    }
}
