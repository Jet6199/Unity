using UnityEngine;

public class ChickenScatterer : MonoBehaviour
{
    public GameObject maleChickenPrefab;
    public GameObject femaleChickenPrefab;
    public int numberOfMales = 10;
    public int numberOfFemales = 10;
    public float yOffset = 0.5f; // Adjust based on the height of the terrain to avoid sinking into the ground

    void Start()
    {
        Terrain terrain = Terrain.activeTerrain; // Get the active terrain
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPosition = terrain.transform.position;

        ScatterChickens(maleChickenPrefab, numberOfMales, terrainSize, terrainPosition);
        ScatterChickens(femaleChickenPrefab, numberOfFemales, terrainSize, terrainPosition);
    }

    void ScatterChickens(GameObject chickenPrefab, int quantity, Vector3 terrainSize, Vector3 terrainPosition)
    {
        for (int i = 0; i < quantity; i++)
        {
            float x = Random.Range(terrainPosition.x, terrainPosition.x + terrainSize.x);
            float z = Random.Range(terrainPosition.z, terrainPosition.z + terrainSize.z);
            Vector3 position = new Vector3(x, yOffset + terrainPosition.y, z); // Adjust Y position based on terrain Y
            Instantiate(chickenPrefab, position, Quaternion.identity);
        }
    }
}
