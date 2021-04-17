using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;
using Unity.Jobs;

public class ECS_Job_PerlinManagerSystemBase : MonoBehaviour
{
    //Instance of this class.
    public static ECS_Job_PerlinManagerSystemBase instance;
    // reference to the Entity Manager.
    EntityManager EntityManager;

    //public GameObjects
    public GameObject cubePrefab;
    [SerializeField]
    public int worldHalfSize;

    //Modifiers in inspector of the PerlinNoise
    [Range(0.1f, 10f)]
    public float strenght1 = 1f;
    [Range(0.01f, 1f)]
    public float scale1 = 0.1f;
    [Range(0.1f, 10f)]
    public float strenght2 = 1f;
    [Range(0.01f, 1f)]
    public float scale2 = 0.1f;
    [Range(0.1f, 10f)]
    public float strenght3 = 1f;
    [Range(0.01f, 1f)]
    public float scale3 = 0.1f;

    //PerlinNoise Mofdifiers
    public static float _strenght1;
    public static float _scale1;
    public static float _strenght2;
    public static float _scale2;
    public static float _strenght3;
    public static float _scale3;

    //bool to change material, so it doesnt need to run on update
    public static bool changeData = false;

    private void Awake()
    {
        instance = this; // Set the intstance to this instance.

        // Get the entity manager of this world
        EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // using gameobject convertion utility to convert prefab to entity, could use shadermesh and material?
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cubePrefab, settings);

        //DRAW THE CUBES IN EACH DIRECTION
        for (int z = -worldHalfSize; z < worldHalfSize; z++)
        {
            for (int x = -worldHalfSize; x < worldHalfSize; x++)
            {
                var position = new Vector3(x, 0, z);
                Entity entityInstance;

                entityInstance = EntityManager.Instantiate(cubeEntity);

                EntityManager.SetComponentData(entityInstance, new Translation { Value = position });
                EntityManager.SetComponentData(entityInstance, new ECS_Job_CubeDataSystemBase { initialPosition = position });
            }
        }
    }
}

