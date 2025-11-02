using UnityEngine;
using UnityEngine.AI;

public class LevelPopulator : MonoBehaviour
{
    public GameObject plane; // Reference to the Plane GameObject
    public GameObject[] spawnPrefabs; // Array of prefabs to spawn
    public int numberOfObjects = 100; // Number of objects to spawn
    public float minScale = 0.8f; // Minimum scale for objects
    public float maxScale = 1.5f; // Maximum scale for objects

    void Start()
    {
        if (plane == null)
        {
            Debug.LogError("Plane not assigned!");
            return;
        }

        PopulatePlane();
    }

    void PopulatePlane()
    {
        // Get the bounds of the plane
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("The plane must have a Renderer component to calculate its bounds.");
            return;
        }

        Bounds planeBounds = planeRenderer.bounds;
        Vector3 min = planeBounds.min;
        Vector3 max = planeBounds.max;

        for (int i = 0; i < numberOfObjects; i++)
        {
            // Randomize position within the plane's bounds
            float xPos = Random.Range(min.x, max.x);
            float zPos = Random.Range(min.z, max.z);
            Vector3 spawnPosition = new Vector3(xPos, plane.transform.position.y, zPos);

            // Randomly select a prefab
            GameObject prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

            // Spawn the object
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Add a NavMeshObstacle component
            NavMeshObstacle obstacle = spawnedObject.AddComponent<NavMeshObstacle>();
            obstacle.carving = true;

            // Randomize scale
            float randomScale = Random.Range(minScale, maxScale);
            spawnedObject.transform.localScale = Vector3.one * randomScale;

            // Parent it to the plane (optional, for organization)
            spawnedObject.transform.SetParent(plane.transform);
        }

        Debug.Log("Plane populated with objects!");
    }
}
