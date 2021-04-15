using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class ECS_Jobs_Burst_PerlinManager : MonoBehaviour
{
    //Instance of this class.
    public static ECS_Jobs_Burst_PerlinManager instance;
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

    //PerlinNoise Modifiers
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
        instance = this; // Set the instance to this instance.

        // Get the entity manager of this world
        EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // using gameobject conversion utility to convert prefab to entity, could use shadermesh and material?
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cubePrefab, settings);

        //DRAW THE CUBES IN EACH DIRECTION
        for (int z = -worldHalfSize; z <= worldHalfSize-1; z++)
        {
            for (int x = -worldHalfSize; x <= worldHalfSize-1; x++)
            {
                var position = new Vector3(x, 0, z);
                Entity entityInstance;

                entityInstance = EntityManager.Instantiate(cubeEntity);

                EntityManager.SetComponentData(entityInstance, new Translation { Value = position });
                EntityManager.SetComponentData(entityInstance, new ECS_Jobs_Burst_CubeData { initialPosition = position });
            }
        }
    }
}

