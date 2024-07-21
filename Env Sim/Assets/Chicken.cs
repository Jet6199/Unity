using UnityEngine;

public class Chicken : MonoBehaviour
{
    public float baseMoveSpeed = 5f; // Base speed to be randomized
    public float moveSpeed; // Actual movement speed
    public float baseHunger = 100f; // Base maximum hunger
    public float hunger; // Current hunger level
    public float baseThirst = 100f; // Base maximum thirst
    public float thirst; // Current thirst level
    public float hungerDecreaseRate; // Rate of hunger decrease per second
    public float thirstDecreaseRate; // Rate of thirst decrease per second
    public float detectionRadius; // Radius to detect food and water
    public GameObject detectionSphere; // GameObject that visualizes the detection radius

    private GameObject target; // Current target object (food or water)
    private GameObject targetChicken;
    private Vector3 randomDirection; // Current direction of random wandering
    public float wanderTimer; // Timer to control direction change in wandering

    private bool isFindingFood = false;
    private bool isFindingWater = false;
    private bool isFindingMate = false;

    public enum Gender { Male, Female }
    public Gender gender; // Gender of the chicken

    public GameObject chickenPrefab; // Prefab used to instantiate baby chickens
    public float reproductionCooldown = 20f; // Time between reproductions
    public float timeSinceLastReproduction = 0f; // Timer to track reproduction cooldown

    void Start()
    {
        moveSpeed = baseMoveSpeed * Random.Range(0.8f, 1.2f); // Randomize movement speed
        hunger = baseHunger; // Start with random hunger level
        thirst = baseThirst; // Start with random thirst level
        wanderTimer = Random.Range(5f, 15f); // Randomize initial wander timer
        hungerDecreaseRate = Random.Range(1f, 2f);
        thirstDecreaseRate = Random.Range(1.5f, 3.5f);
        detectionRadius = Random.Range(5f, 20f);

        timeSinceLastReproduction = reproductionCooldown; // Start with the ability to reproduce immediately

    UpdateDetectionSphere();

        UpdateDetectionSphere();
    }

    void Update()
    {
        hunger -= hungerDecreaseRate * Time.deltaTime;
        thirst -= thirstDecreaseRate * Time.deltaTime;
        timeSinceLastReproduction += Time.deltaTime; // Increment reproduction timer

        // Update wandering behavior
        wanderTimer -= Time.deltaTime;
        

        // Check for targets
        if (target == null || Vector3.Distance(transform.position, target.transform.position) > detectionRadius)
        {
            FindNewTarget();
        }

        if (target != null)
            MoveToTarget();
        else
            WanderRandomly();

        if (hunger <= 0 || thirst <= 0)
        {
            Die();
        }

        Chicken targetChicken = FindNearestChicken();
        if (targetChicken != null && 
            targetChicken.gender != gender && 
            timeSinceLastReproduction > reproductionCooldown &&
            Vector3.Distance(transform.position, targetChicken.transform.position) <= detectionRadius) // Check if the target is within the detection radius
        {
            Debug.Log("Attempting to reproduce...");
            Reproduce(targetChicken);
            targetChicken = null; // You might not need to nullify this here unless it's for a specific reason
        }


        else
        {
            if (timeSinceLastReproduction < reproductionCooldown) {
                Debug.Log("Reproduction cooldown not finished: " + timeSinceLastReproduction);
            }
            if (target == null) {
                Debug.Log("No target found.");
            } else if (targetChicken.gender == gender) {
                Debug.Log(gender);
                Debug.Log(targetChicken.gender);
                Debug.Log("Same gender, cannot reproduce.");
            }
        }
        if (target == null && wanderTimer <= 0)
        {
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * moveSpeed;
            wanderTimer = Random.Range(5f, 15f); // Reset timer with a new random value
        }
        UpdateDetectionSphere();
    }

    void WanderRandomly()
    {
        transform.position += randomDirection * Time.deltaTime;
        transform.LookAt(transform.position + randomDirection);
    }

    void FindNewTarget()
    {
        if (hunger <= 50f && !isFindingFood)
        {
            target = FindNearestTaggedObject("Food");
            isFindingFood = true;
            isFindingWater = false;
            Debug.Log("Finding Food");
        }
        else if (thirst <= 60f && !isFindingWater)
        {
            target = FindNearestTaggedObject("Water");
            isFindingWater = true;
            isFindingFood = false;
            Debug.Log("Finding Water");
        }
        // Only consider other chickens as potential reproduction targets
        else if (timeSinceLastReproduction >= reproductionCooldown)
        {
            target = FindNearestTaggedObject("Chicken");
            isFindingMate = true;
            isFindingWater = false;
            isFindingFood = false;
            Debug.Log("Finding Mate");
        }
    }
    
    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.transform.position) < 1f)
        {
            if (isFindingFood)
            {
                Eat();
            }
            else if (isFindingWater)
            {
                Drink();
            }
        }
    }

    void Reproduce(Chicken mate)
    {
        float distance = Vector3.Distance(transform.position, mate.transform.position);
        Debug.Log("Distance to mate: " + distance);
        if (distance < detectionRadius)
        {
            Vector3 spawnPosition = (transform.position + mate.transform.position) / 2;
            GameObject babyChicken = Instantiate(chickenPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Baby chicken created at " + spawnPosition);

            Chicken babyScript = babyChicken.GetComponent<Chicken>();
            if (babyScript != null)
            {
                babyScript.moveSpeed = (moveSpeed + mate.moveSpeed) / 2 * 0.5f;
                babyScript.hungerDecreaseRate = (hungerDecreaseRate + mate.hungerDecreaseRate) / 2;
                babyScript.thirstDecreaseRate = (thirstDecreaseRate + mate.thirstDecreaseRate) / 2;
                babyScript.detectionRadius = (detectionRadius + mate.detectionRadius) / 2 * 0.5f;

                timeSinceLastReproduction = 0;
                mate.timeSinceLastReproduction = 0;
                isFindingMate = false;
                babyScript = null;
            }
            else
            {
                Debug.LogError("Failed to assign baby script properties.");
            }
        }
        else
        {
            Debug.Log("Mate not within reproduction distance.");
        }
    }



    void Eat()
    {
        Debug.Log("Eating food.");
        hunger = 100f;
        isFindingFood = false;
        target = null;
    }

    void Drink()
    {
        Debug.Log("Drinking water.");
        thirst = 100f;
        isFindingWater = false;
        target = null;
    }

    GameObject FindNearestTaggedObject(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject obj in taggedObjects)
        {
            float dist = Vector3.Distance(obj.transform.position, currentPos);
            if (dist < minDistance && dist <= detectionRadius)
            {
                nearest = obj;
                minDistance = dist;
            }
        }
        return nearest;
    }

    Chicken FindNearestChicken()
    {
        GameObject[] taggedChickens = GameObject.FindGameObjectsWithTag("Chicken");
        Chicken nearestChicken = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject chickenObj in taggedChickens)
        {
            // Skip the current chicken to avoid detecting itself
            if (chickenObj == this.gameObject) {
                continue;
            }

            Chicken chicken = chickenObj.GetComponent<Chicken>();
            if (chicken != null)
            {
                float dist = Vector3.Distance(chicken.transform.position, currentPos);
                if (dist < minDistance && dist <= detectionRadius)
                {
                    nearestChicken = chicken;
                    minDistance = dist;
                }
            }
        }
        return nearestChicken;
    }


    void UpdateDetectionSphere()
    {
        if (detectionSphere != null)
        {
            detectionSphere.transform.localScale = Vector3.one * detectionRadius * 2; // Adjust the scale according to the detection radius
        }
    }

    void Die()
    {
        Debug.Log("Chicken has died.");
        gameObject.SetActive(false);
    }
}
