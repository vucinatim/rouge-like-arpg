using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 8, -8); // Offset from the player
    private float lagFactor = 0.15f; // Leeway factor for camera lag
    private Transform player; // Reference to the player's transform
    private Vector3 velocity = Vector3.zero; // Used for smoothing

    public void AssignCameraToPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        // Calculate the target position
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera towards the target position with "lag"
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, lagFactor);
    }
}
