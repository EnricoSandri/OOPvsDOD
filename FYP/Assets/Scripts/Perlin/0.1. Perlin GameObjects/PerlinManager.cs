using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerlinManager : MonoBehaviour
{
    //public GameObjects
    public GameObject cubePrefab;
    //[SerializeField]
     int axisHalfSize;

    public static bool changeData = false;

    //Modifiers in inspector of the PerlinNoise
    [Range(0.1f, 10f)]
    public float strenght1 = 2f;
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

    public List<GameObject> worldCubes = new List<GameObject>();

    //Global Values to check against
    private float height;
    private float perlin1;
    private float perlin2;
    private float perlin3;
    private Vector3 position;
    private Vector3 vertex;

    private float _strength1;
    private float _scale1;
    private float _strength2;
    private float _scale2;
    private float _strength3;
    private float _scale3;

    public GameObject canvas;
    public Text amount; 
    public Slider moveValue;
    private int loopCount = 0;
    private int inputValue;
    public bool IsActive { get; set; }

    private void Start()
    {
        
        //DrawCubes();
    }

    private void Update()
    {
        if (IsActive && loopCount < 1)
        {
            inputValue = int.Parse(amount.text);
            axisHalfSize = inputValue;
            canvas.SetActive(false);
            DrawCubes();
            loopCount++;
        }
        strenght1 = moveValue.value;
        AddPerlin();
        StartCoroutine(routine: DataChangeCheck());
        if (!changeData) return;
        //AddPerlin();
    }

    private void DrawCubes()
    {
        //DRAW THE CUBES IN EACH DIRECTION
        for (int z = -axisHalfSize; z < axisHalfSize; z++)
        {
            for (int x = -axisHalfSize; x < axisHalfSize; x++)
            {
                //position = new Vector3(x, 0, z);
                worldCubes.Add(Instantiate(cubePrefab, new Vector3(x, 0, z), Quaternion.identity));
            }
        }
    }

    private void AddPerlin()
    {
        foreach (GameObject cube in worldCubes)
        {
            vertex = cube.transform.position;

            perlin1 = Mathf.PerlinNoise(vertex.x * scale1, vertex.z * scale1) * strenght1;
            perlin2 = Mathf.PerlinNoise(vertex.x * scale2, vertex.z * scale2) * strenght2;
            perlin3 = Mathf.PerlinNoise(vertex.x * scale3, vertex.z * scale3) * strenght3;
            height = perlin1 + perlin2 + perlin3;

            cube.transform.position = new Vector3(vertex.x, height, vertex.z);
        }
    }

    private IEnumerator DataChangeCheck()
    {
        if (_strength1 != strenght1)
        {
            _strength1 = strenght1;
            changeData = true;
        }
        else if (_strength2 != strenght2)
        {
            _strength2 = strenght2;
            changeData = true;
        }
        else if (_strength3 != strenght3)
        {
            _strength3 = strenght3;
            changeData = true;
        }
        else if (_scale1 != scale1)
        {
            _scale1 = scale1;
            changeData = true;
        }
        else if (_scale2 != scale2)
        {
            _scale2 = scale2;
            changeData = true;
        }
        else if (_scale3 != scale3)
        {
            _scale3 = scale3;
            changeData = true;
        }
        
        else changeData = false;

        yield return null;
    }
}
