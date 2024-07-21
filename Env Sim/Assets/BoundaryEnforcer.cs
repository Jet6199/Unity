using UnityEngine;

public class BoundaryEnforcer : MonoBehaviour
{
    private Terrain terrain; // Now a private variable, set automatically
    private Vector3 terrainSize;
    private Vector3 terrainPosition;

    void Start()
    {
        // Automatically get the active terrain
        terrain = Terrain.activeTerrain;

        if (terrain == null)
        {
            Debug.LogError("BoundaryEnforcer: No active terrain found in the scene!");
            return;
        }

        // Initialize terrain size and position from the active terrain
        terrainSize = terrain.terrainData.size;
        terrainPosition = terrain.transform.position;
    }

    void Update()
    {
        if (terrain == null) return; // Safety check to prevent errors if terrain isn't found

        // Clamp the object's position within the bounds of the terrain
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, terrainPosition.x, terrainPosition.x + terrainSize.x);
        position.z = Mathf.Clamp(position.z, terrainPosition.z, terrainPosition.z + terrainSize.z);
        
        // Adjust the y position to ensure it doesn't go below the terrain surface
        float terrainHeight = terrain.SampleHeight(transform.position) + terrainPosition.y;
        if (position.y < terrainHeight)
        {
            position.y = terrainHeight;
        }

        transform.position = position;
    }
}
