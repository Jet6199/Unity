using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target; // The chicken's transform
    public float height = 10f; // Height of the camera above the target
    public float followSpeed = 2f; // Speed at which the camera follows the target

    private Vector3 offset; // Offset position to the target

    void Start()
    {
        // Set initial offset. Adjust the x and z values to center or adjust camera position relative to the target
        offset = new Vector3(0, height, 0);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Update the position of the camera to follow the chicken from above
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Ensure the camera always looks at the chicken
        transform.LookAt(target);
    }
}
