using UnityEngine;

public class Chicken : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float hunger = 100f;
    public float thirst = 100f;
    public float hungerDecreaseRate = 1f; // per second
    public float thirstDecreaseRate = 1.5f; // per second
    public float detectionRadius = 10f; // Detection radius for finding food and water
    public GameObject detectionSphere; // Reference to the detection sphere GameObject

    private GameObject target;
    private Vector3 randomDirection;
    private float wanderTimer = 5f; // Time to change direction
    private bool isFindingFood = false;
    private bool isFindingWater = false;

    void Start()
    {
        UpdateDetectionSphere();
    }

    void Update()
    {
        hunger -= hungerDecreaseRate * Time.deltaTime;
        thirst -= thirstDecreaseRate * Time.deltaTime;

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

        UpdateDetectionSphere();
    }

    void UpdateDetectionSphere()
    {
        if (detectionSphere != null)
        {
            detectionSphere.transform.localScale = Vector3.one * detectionRadius * 2; // Scale the sphere to the correct radius
        }
    }

    void WanderRandomly()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            wanderTimer = 5f;
        }

        transform.position += randomDirection * moveSpeed * Time.deltaTime;
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

    void Die()
    {
        Debug.Log("Chicken has died.");
        gameObject.SetActive(false);
    }
}
