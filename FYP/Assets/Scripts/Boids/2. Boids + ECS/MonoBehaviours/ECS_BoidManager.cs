using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class ECS_BoidManager : MonoBehaviour
{
    //Instance of this class.
    public static ECS_BoidManager instance;

    //Set the amount of boids and the boids shared mesh and material in the Inspector 
    [Header("Boid Settings")]
    [SerializeField] private int amountOfBoids;
    [SerializeField] private Mesh boidSharedMesh;
    [SerializeField] private Material boidSharedMaterial;

    //Boid values
    [Header("Boid Values")]
    public float speed;              // Speed of each boid.
    public float perceptionRadius;  // Boids perceptionRadious to check for other nearby boids.

    // Customisable Rule Multipliers.
    [Header("Rules Multipliers")]
    [Tooltip("Each boid attempts to maintain a reasonable amount of distance between itself and any nearby boids, to prevent overcrowding.")]
    public float separationMultiplier;
    [Tooltip("Every boid attempts to move towards the average position of other nearby boids")]
    public float cohesionMultiplier;
    [Tooltip("Boids try to change their position so that it corresponds with the average alignment of other nearby boids.")]
    public float alignmentMultiplier;

    // Boids behaviour values for enclosure .
    [Header("Enclosure Values")]
    public float enclosureSize; public float wallAvoidanceMultiplier; public float turnAwayFromWallDistance;

    public float3 scale =  new float3(.1f, .1f, .3f);

   // private Quaternion randomRotation;
   // private Vector3 randomPosition;

    private void Awake()
    {
        instance = this; // Set the intstance to this instance.

        // Get the entity manager of this world
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        /*
            Create the Archetype with entity and its neccesary components
            - BOID COMPONENT
            - MESH
            - MESH RENDERER
            - TRANSFORM
        */
        EntityArchetype boidEntityArchetype = entityManager.CreateArchetype(
            typeof(BoidComponent),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld)
            );
        
        //Array cointaining the boid entities. Temp allocator, fastest and lifespan of a frame or less. 
        NativeArray<Entity> boidArray = new NativeArray<Entity>(amountOfBoids, Allocator.Temp); // TO DISPOSE

        // Create the boid entities, from the boidArray.
        entityManager.CreateEntity(boidEntityArchetype, boidArray);

        /* Set component values such as:
            random position and rotation,
            Mesh and  Mesh Renderer
            for each boid in present in the boidArray
        */
        for (int i = 0; i < boidArray.Length; i++)
        {
            //Set the position and rotation
            entityManager.SetComponentData(boidArray[i], new LocalToWorld {

                //Access the TRASFORM of the entity. TRS(translation,rotation,scale) 
                Value = float4x4.TRS(
                        RandomisePositions(),       // Radomise the spawn location for each boid.
                        RandomiseRotation(),       // Randomise the spawn rotation for each boid.
                        scale)
            });

            //Set the RenderMesh values, RenderMesh (Mesh, Material)
            entityManager.SetSharedComponentData(boidArray[i], new RenderMesh { 
               mesh = boidSharedMesh,        // Set Mesh
               material = boidSharedMaterial, // Set Material
            });
        }

        //DISPOSE ARRAY
        boidArray.Dispose();
    }

    //Randomise the spawn position values within the enclosure.
    private float3 RandomisePositions()
    {
        return new float3(
               UnityEngine.Random.Range(-enclosureSize / 2f, enclosureSize / 2f),
               UnityEngine.Random.Range(-enclosureSize / 2f, enclosureSize / 2f),
               UnityEngine.Random.Range(-enclosureSize / 2f, enclosureSize / 2f)
               );
    }

    //Randomise the spawn rotation values within the enclosure.
    private quaternion RandomiseRotation()
    {
        return quaternion.Euler(
                        UnityEngine.Random.Range(-360f, 360f),
                        UnityEngine.Random.Range(-360f, 360f),
                        UnityEngine.Random.Range(-360f, 360f)
                        );
    }

    // Visual representation of the enclosure in the scene.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(                  // Draw the Cube with the center being x0.y0.z0 and size of enclosure.
            Vector3.zero,
            new Vector3(enclosureSize, enclosureSize, enclosureSize));
    }
}
