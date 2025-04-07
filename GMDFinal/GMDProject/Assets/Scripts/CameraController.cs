using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;      // The player's transform to follow
    public Vector3 offset;        // Offset position between the player and the camera
    public float smoothSpeed = 0.125f;  // Smoothness of the camera movement

    // Update is called once per frame
    void LateUpdate()
    {
        // Desired position based on the player's position + offset
        Vector3 desiredPosition = player.position + offset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}

