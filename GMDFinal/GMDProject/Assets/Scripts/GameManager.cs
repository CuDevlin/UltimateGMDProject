using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject playerPrefab;   // Reference to the player prefab
    public GameObject cameraPrefab;   // Reference to the camera prefab
    public Transform playerSpawnPoint; // The spawn point for the player

    private GameObject currentPlayer;
    private GameObject currentCamera;  // To reference the created camera

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartGame()
    {
        // Ensure no camera exists from previous game sessions.
        if (currentCamera != null)
        {
            Destroy(currentCamera);  // Destroy the existing camera if it exists
        }

        // Spawn player at the spawn point
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        }

        // Instantiate the camera and set it to follow the player
        if (cameraPrefab != null)
        {
            currentCamera = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity);  // Position the camera initially at (0,0,0)

            // Link camera to player
            CameraController camController = currentCamera.GetComponent<CameraController>();
            if (camController != null && currentPlayer != null)
            {
                camController.player = currentPlayer.transform;  // Set the camera to follow the player
            }
            else
            {
                Debug.LogError("CameraController or player not found during camera setup.");
            }
        }

        // Enable player controls
        if (currentPlayer != null)
        {
            PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableControls(true);  // Enable player controls
            }
        }

        // Enable enemy spawning
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.SetSpawningEnabled(true);
        }
    }

    public void ResetGame()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        var projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (var proj in projectiles)
        {
            Destroy(proj);
        }

        // Clear enemies
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.ClearEnemies();
            spawner.SetSpawningEnabled(false);
        }

        // Destroy existing camera before creating a new one
        if (currentCamera != null)
        {
            Destroy(currentCamera);
        }

        // Show main menu again
        UIHandler.instance.ShowMainMenu();
    }
}