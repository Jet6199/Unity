using UnityEngine;

public class ScatterObjects : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject waterPrefab;
    public int numberOfFood = 10;
    public int numberOfWater = 5;
    public Terrain terrain; // Reference to the Terrain object
    public float yOffset = 0.5f; // Adjust based on the height of the terrain to avoid sinking into the ground

    void Start()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPos = terrain.transform.position;

        Scatter(foodPrefab, numberOfFood, terrainSize, terrainPos);
        Scatter(waterPrefab, numberOfWater, terrainSize, terrainPos);
    }

    void Scatter(GameObject prefab, int quantity, Vector3 size, Vector3 position)
    {
        for (int i = 0; i < quantity; i++)
        {
            float x = Random.Range(position.x, position.x + size.x);
            float z = Random.Range(position.z, position.z + size.z);
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + yOffset; // Get the height of the terrain at this point and add yOffset
            Vector3 spawnPosition = new Vector3(x, y, z);
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }
}
