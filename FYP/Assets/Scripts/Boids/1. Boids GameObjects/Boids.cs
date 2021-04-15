using UnityEngine;
public class Boids : MonoBehaviour
{
    // Reference to the Boid manager.
    private BoidManager manager; 

    // Rules for the boids to follow.
    private Vector3 seperationRule, cohesionRule, alignmentRule;
    
    // values to apply to avoid the enclosure walls.
    private Vector3 wallsAvoidanceForce;
    
    // Start is called before the first frame update
    void Start()
    {
        manager = BoidManager.instance; // reference to the boid manager instance.
    }

    // Update is called once per frame
    void Update()
    {
        CalculateRuleValues();
        ApplyRulesAndMultiplier();
    }

    // calculate the rule values for each boid. 
    private void CalculateRuleValues()
    {
        //Reset all the rule values to 0.0.0. 
        Vector3 seperationSum = Vector3.zero;
        Vector3 positionSum = Vector3.zero;
        Vector3 headingSum = Vector3.zero;

        int nearbyBoids = 0; // Boids present in the  perception radius.

        for (int i = 0; i < manager.boids.Count; i++) // iterate through all the boids present in the boids list present in the manager.
        {
            if(this != manager.boids[i])  // all the other boids apart this.
            {
                Vector3 otherBoidPosition = manager.boids[i].transform.position; // save the position of the Other boids.
                float otherBoidDistance = (transform.position - otherBoidPosition).magnitude; // the vector distance from this boid to the other.

                // check if the otherboid is within the boids perception radius. 
                if (otherBoidDistance < manager.perceptionRadius) 
                {
                    // Get all the values  of boids within the radius

                    // separation => prevents boids from crowding. negative force needed to move away from the other boid.
                    seperationSum += -(otherBoidPosition - transform.position) * (1f / Mathf.Max(otherBoidDistance, .0001f));
                    // Get the position of the other boid
                    positionSum += otherBoidPosition;
                    // Get the heading of the other boid.
                    headingSum += manager.boids[i].transform.forward;

                    // add boid to the number of nearby boids.
                    nearbyBoids++;
                }
            }
        }
        // if there is a boid present in the radius, find the average and apply the ruleSums values to this boid.
        if(nearbyBoids > 0)
        {
            seperationRule = seperationSum / nearbyBoids;
            cohesionRule = (positionSum / nearbyBoids) - transform.position;
            alignmentRule = headingSum / nearbyBoids;
        }
        else
        {
            // if there is no nearby boid set all the rules values to 0
            seperationRule = Vector3.zero;
            cohesionRule = Vector3.zero;
            alignmentRule = Vector3.zero;
        }

        // check if the boid is going outside the set enclosure, if so, make it turn around. else dont apply any rule value.
        if (minimumDistanceToBorder(transform.position, manager.enclosureSize) < manager.turnAwayFromWallDistance)
        {
            wallsAvoidanceForce = -transform.position.normalized;
        }
        else
        {
            wallsAvoidanceForce = Vector3.zero;
        }
        
    }
    // apply to the boid all the multipliers values in the manager class of the collected rule values. apply movement to the boid. 
    private void ApplyRulesAndMultiplier()
    {
        // multiply and add all the manager values to the collected rule values
        Vector3 ruleValues =
            seperationRule * manager.separationMultiplier +
            cohesionRule * manager.cohesionMultiplier +
            alignmentRule * manager.alignmentMultiplier +
            wallsAvoidanceForce * manager.wallAvoidanceMultiplier;

        //apply movement to the boid  utilising the ruleValues,
        Vector3 velocity = transform.forward * manager.speed; // set the velocity.
        velocity += ruleValues * Time.deltaTime;              // apply the rule value into time.
        velocity = velocity.normalized * manager.speed;       // normalise.

        transform.position += velocity * Time.deltaTime;       // apply the velocity to the position of the boid into time
        transform.rotation = Quaternion.LookRotation(velocity); // apply the forward and rotation using the velocity.
    }

    // return the distance between the boids and the enclosure walls on all axis of the enclosure. 
    private float minimumDistanceToBorder(Vector3 bPos, float enclosureSize)
    {
        float enclosureHalfSize = enclosureSize / 2f;

        return Mathf.Min(Mathf.Min(
            enclosureHalfSize - Mathf.Abs(bPos.x),
            enclosureHalfSize - Mathf.Abs(bPos.y),
            enclosureHalfSize - Mathf.Abs(bPos.z)
            ));
    }
}
