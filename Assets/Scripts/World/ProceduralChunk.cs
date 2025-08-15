using UnityEngine;

[ExecuteAlways] // Update in edit mode
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralChunk : MonoBehaviour
{
    [Header("Terrain Settings")]
    [SerializeField] private ChunkSettings chunkSettings; // ScriptableObject holding chunk info
    private float startingBaseHeight;
    private int seed;
    private int chunkIndex;

    [Header("Mesh Settings")]
    public float bottomY; // Y position of the bottom of the terrain
    private Vector2[] points; // Terrain points (Ground)
    private MeshFilter meshFilter; // Vizual
    private EdgeCollider2D edgeCollider;
    

    private void Start()
    {
       
    }

    public void Initialize(ChunkSettings chunkSettings, float startingBaseHeight, int seed, int chunkIndex)
    {

        this.chunkSettings = chunkSettings;
        this.chunkIndex = chunkIndex;
        this.seed = seed;
        
        this.startingBaseHeight = startingBaseHeight;

        if (Camera.main != null)
            bottomY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        else
            bottomY = -5f; // Default fallback

        meshFilter = GetComponent<MeshFilter>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        
        GeneratePoints();
        GenerateMesh();
    }

    private void OnValidate()
    {
           
    }

    // Generates terrain points using Perlin noise
    private void GeneratePoints()
    {
        if (chunkSettings.resolution < 1)
            chunkSettings.resolution = 1;

        // Sum +1 to include the last edge
        points = new Vector2[chunkSettings.resolution + 1];
        float offset = chunkIndex * seed;


        // Generate each point
        for (int i = 0; i <= chunkSettings.resolution; i++)
        {
            float t = i / (float)chunkSettings.resolution;
            float x = t * chunkSettings.width; // X position

            float y;
            if (i == 0)
            {
                // First edge uses the baseHeight exactly for smooth connection to last chunk
                y = startingBaseHeight;
            }
            else
            {
                // - 0.5f) * 2f -> Convert from [0,1] to [-1,1] range
                y = chunkSettings.baseHeight + (Mathf.PerlinNoise(x * chunkSettings.noiseScale + offset, 0f) - 0.5f) * 2f * chunkSettings.amplitude;
            }

            points[i] = new Vector2(x, y);
        }

        UpdateCollider();
    }

    // Visualizes points using Gizmos
    private void VisualizePoints()
    {
        if (points == null || points.Length == 0)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(points[i]);
            Gizmos.DrawSphere(worldPos, 0.05f);

            if (i < points.Length - 1)
            {
                Vector3 nextWorldPos = transform.TransformPoint(points[i + 1]);
                Gizmos.DrawLine(worldPos, nextWorldPos);
            }
        }
    }

    public float GetRightEdgeHeight()
    {
        if (points == null || points.Length == 0)
            return chunkSettings.baseHeight;

        return points[points.Length - 1].y;
    }

    // Generates the mesh
    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "TerrainMesh";

        if (points == null || points.Length == 0) { return; }


        int vertCount = points.Length * 2; // Top and bottom
        Vector3[] vertices = new Vector3[vertCount];

        int[] triangles = new int[(points.Length - 1) * 6]; // 2 triangles per segment (quad)
        Vector2[] uvs = new Vector2[vertCount]; // Mesh texture

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 topPoint = points[i];
            vertices[i] = new Vector3(topPoint.x, topPoint.y, 0f);
            vertices[i + points.Length] = new Vector3(topPoint.x, bottomY, 0f);

            float u = i / (float)(points.Length - 1);
            uvs[i] = new Vector2(u, 1);
            uvs[i + points.Length] = new Vector2(u, 0);
        }

        int triIndex = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            int topLeft = i;
            int topRight = i + 1;
            int bottomLeft = i + points.Length;
            int bottomRight = i + points.Length + 1;

            triangles[triIndex++] = topLeft;
            triangles[triIndex++] = topRight;
            triangles[triIndex++] = bottomLeft;

            triangles[triIndex++] = bottomLeft;
            triangles[triIndex++] = topRight;
            triangles[triIndex++] = bottomRight;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
    }

    private void UpdateCollider()
    {
       if (edgeCollider == null || points == null || points.Length == 0)
        return;

        // Assign points to the collider
        edgeCollider.points = points;
    }

    private void OnDrawGizmos()
    {
        VisualizePoints();
    }

    //------------ Getter and Setter ------------
    public float GetWidth() { return chunkSettings.width; }
}