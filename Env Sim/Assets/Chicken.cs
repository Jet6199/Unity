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
    private Vector3 randomDirection; // Current direction of random wandering
    public float wanderTimer; // Timer to control direction change in wandering

    private bool isFindingFood = false;
    private bool isFindingWater = false;

    public enum Gender { Male, Female }
    public Gender gender; // Gender of the chicken

    public GameObject chickenPrefab; // Prefab used to instantiate baby chickens
    public float reproductionCooldown = 20f; // Time between reproductions
    public float timeSinceLastReproduction = 0f; // Timer to track reproduction cooldown

    void Start()
    {
        moveSpeed = baseMoveSpeed * Random.Range(0.8f, 1.2f); // Randomize movement speed
        hunger = Random.Range(baseHunger * 0.5f, baseHunger); // Start with random hunger level
        thirst = Random.Range(baseThirst * 0.5f, baseThirst); // Start with random thirst level
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
        if (wanderTimer <= 0)
        {
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * moveSpeed;
            wanderTimer = Random.Range(5f, 15f); // Reset timer with a new random value
        }

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
        if (timeSinceLastReproduction >= reproductionCooldown && target != null && target.GetComponent<Chicken>().gender != gender)
        {
            Reproduce(target.GetComponent<Chicken>());
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
        }
        else if (thirst <= 60f && !isFindingWater)
        {
            target = FindNearestTaggedObject("Water");
            isFindingWater = true;
            isFindingFood = false;
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
        if (Vector3.Distance(transform.position, mate.transform.position) < detectionRadius)
        {
            GameObject babyChicken = Instantiate(chickenPrefab, (transform.position + mate.transform.position) / 2, Quaternion.identity);
            Chicken babyScript = babyChicken.GetComponent<Chicken>();

            // Average out the parents' properties for the baby
            babyScript.moveSpeed = (moveSpeed + mate.moveSpeed) / 2 * 0.5f; // Baby moves slower
            babyScript.hungerDecreaseRate = (hungerDecreaseRate + mate.hungerDecreaseRate) / 2;
            babyScript.thirstDecreaseRate = (thirstDecreaseRate + mate.thirstDecreaseRate) / 2;
            babyScript.detectionRadius = detectionRadius * 0.5f; // Smaller detection radius for the baby

            timeSinceLastReproduction = 0; // Reset reproduction timer
            mate.timeSinceLastReproduction = 0; // Reset mate's timer as well
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
