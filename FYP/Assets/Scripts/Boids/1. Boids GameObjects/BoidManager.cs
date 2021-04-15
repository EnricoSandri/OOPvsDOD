using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    /// <summary>
    /// This class is responsible for the managment of the boids and the ensclosure in which the boids fly...
    /// https://zklinger.com/unity-boids/
    /// </summary>

    //Instance of this class.
    public static BoidManager instance;

    //Set the amount of boids and the boids prefab in the Inspector 
    [Header("Boid Settings")]
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private int amountOfBoids;
    
    //Boid values
    [Header("Boid Values")]
    public float speed;              // Speed of each boid.
    public float perceptionRadius;  // Boids perceptionRadius to check for other nearby boids.

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

    //List holding all the boids.
    public  List<Boids> boids;
    private Quaternion randomRotation;
    private Vector3 randomPosition;
    private void Awake()
    {
        instance = this; // Set the instance to this instance.

        boids.Clear(); // Make sure the list of boids is empty.

        // Spawn the # boids in the enclosure at random positions and rotations. 
        for (int i = 0; i < amountOfBoids; i++)  //Iteration through the number of boids set by the user.
        {
            RandomisePositions();       // Randomise the spawn location for each boid.
            RandomiseRotation();        // Randomise the spawn rotation for each boid.
            InstatiateAndAddToList();   // Instantiation of the boids and applying the randomised values.
        }
    }
    
    //Randomise the spawn position values within the enclosure.
    private void RandomisePositions()
    {
         randomPosition = new Vector3(

                Random.Range(-enclosureSize / 2f, enclosureSize / 2f),
                Random.Range(-enclosureSize / 2f, enclosureSize / 2f),
                Random.Range(-enclosureSize / 2f, enclosureSize / 2f)
                );
    }

    //Randomise the spawn rotation values within the enclosure.
    private void RandomiseRotation()
    {
         randomRotation = Quaternion.Euler(
                        Random.Range(0f, 360f),
                        Random.Range(0f, 360f),
                        Random.Range(0f, 360f)
                        );
    }

    // Instantiate the boids and add them to the boids list holder.
    private void InstatiateAndAddToList()
    {
        Boids newboid = Instantiate(boidPrefab, randomPosition, randomRotation).GetComponent<Boids>();
        boids.Add(newboid);
    }

   // public List<Boids> BoidCollection() { return boids; } // method which returns the boids collection. 

    // Visual representation of the enclosure in the scene.
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.cyan; 
        
        Gizmos.DrawWireCube(                  // Draw the Cube with the center being x0.y0.z0 and size of enclosure.
            Vector3.zero, 
            new Vector3(enclosureSize, enclosureSize, enclosureSize)); 
    }
}
