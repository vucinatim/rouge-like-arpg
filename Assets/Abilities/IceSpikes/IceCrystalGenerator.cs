using UnityEngine;

public class IceCrystalGenerator
{
    private int numberOfSides;
    private float baseRadius;
    private float height;
    private Material crystalMaterial;

    /// <summary>
    /// Constructor to set up the generator parameters.
    /// </summary>
    public IceCrystalGenerator(int numberOfSides, float baseRadius, float height, Material crystalMaterial)
    {
        this.numberOfSides = numberOfSides;
        this.baseRadius = baseRadius;
        this.height = height;
        this.crystalMaterial = crystalMaterial;
    }

    /// <summary>
    /// Generates a single ice crystal GameObject with a procedural mesh.
    /// </summary>
    /// <returns>A GameObject representing the ice crystal.</returns>
    public GameObject Generate(Vector3 position, Quaternion rotation)
    {
        // Create a new GameObject for the crystal
        GameObject crystal = new GameObject("IceCrystal");

        // Add components
        MeshFilter meshFilter = crystal.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = crystal.AddComponent<MeshRenderer>();
        meshRenderer.material = crystalMaterial;

        // Generate the crystal mesh
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Vertices
        Vector3[] vertices = new Vector3[numberOfSides + 2];
        vertices[0] = Vector3.zero; // Center of the base
        for (int i = 0; i < numberOfSides; i++)
        {
            float angle = (float)i / numberOfSides * Mathf.PI * 2;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * baseRadius, 0, Mathf.Sin(angle) * baseRadius);
        }
        vertices[numberOfSides + 1] = new Vector3(0, height, 0); // Tip of the crystal

        // Triangles
        int[] triangles = new int[numberOfSides * 6];
        for (int i = 0; i < numberOfSides; i++)
        {
            // Base
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % numberOfSides + 1;

            // Sides
            triangles[numberOfSides * 3 + i * 3] = i + 1;
            triangles[numberOfSides * 3 + i * 3 + 1] = numberOfSides + 1;
            triangles[numberOfSides * 3 + i * 3 + 2] = (i + 1) % numberOfSides + 1;
        }

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Set position and rotation
        crystal.transform.position = position;
        crystal.transform.rotation = rotation;

        return crystal;
    }
}
