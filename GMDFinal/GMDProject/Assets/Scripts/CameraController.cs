using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;  // The player's transform to follow
    public Vector3 offset = new Vector3(0f, 0f, -10f);  // Offset from the player
    public float smoothSpeed = 0.125f;  // Smoothness factor

    void Start()
    {
        // Ensure camera is linked to the player when the game starts
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                Debug.LogError("Player not found at Start. Camera cannot follow.");
                return;
            }
        }
    }

    void Update()
    {
        if (player == null)
        {
            return; // If no player, exit the method
        }

        // Update camera position based on player position + offset
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}