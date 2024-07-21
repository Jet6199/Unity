using UnityEngine;

public class CamerAdjust : MonoBehaviour
{
    public Terrain terrain;

    void Start()
    {
        if (terrain)
        {
            Vector3 terrainSize = terrain.terrainData.size;
            Vector3 terrainPosition = terrain.transform.position;

            // Calculate center of the terrain
            Vector3 centerPosition = terrainPosition + terrainSize / 2;

            // Set camera position above the center
            transform.position = new Vector3(centerPosition.x, transform.position.y, centerPosition.z);

            // Adjust orthographic size based on the larger dimension of the terrain
            GetComponent<Camera>().orthographicSize = Mathf.Max(terrainSize.x, terrainSize.z) / 2;
        }

        // Ensure the camera is looking straight down
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
