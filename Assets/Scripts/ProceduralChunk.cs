using UnityEngine;

[ExecuteAlways] // Update in edit mode
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralChunk : MonoBehaviour
{
    [Header("Terrain Settings")]
    private int resolution;     // Number of segments
    private float width;       // Total width in world units
    private float baseHeight;   // Baseline Y
    private float amplitude;    // Height variation
    private float noiseScale; // Controls noise frequency
    private int seed;
    private int offset;

    [Header("Mesh Settings")]
    public float bottomY; // Y position of the bottom of the terrain
    private Vector2[] points; // Terrain points (Ground)
    // private float offset; // offset / seed
    private MeshFilter meshFilter;
    private EdgeCollider2D edgeCollider;
    

    private void Start()
    {
       
    }

    public void Initialize(ChunkSettings chunkSettings){
        this.resolution = chunkSettings.resolution;
        this.width = chunkSettings.width;
        this.baseHeight = chunkSettings.baseHeight;
        this.amplitude = chunkSettings.amplitude;
        this.noiseScale = chunkSettings.noiseScale;
        this.seed = chunkSettings.seed;

        if (Camera.main != null)
            bottomY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        else
            bottomY = -5f; // Default fallback for editor preview

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
        if (resolution < 1)
            resolution = 1;

        points = new Vector2[resolution + 1];
        System.Random rng = new System.Random(seed);
        offset = rng.Next(-10000, 10000);

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            float x = t * width;
            float y = baseHeight + (Mathf.PerlinNoise(x * noiseScale + offset, 0f) - 0.5f) * 2f * amplitude;
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

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            meshFilter = GetComponent<MeshFilter>();
            GeneratePoints();
            GenerateMesh();
        }
    }
#endif
    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "TerrainMesh";

        if (points == null || points.Length == 0) { return; }

        int vertCount = points.Length * 2;
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[(points.Length - 1) * 6];
        Vector2[] uvs = new Vector2[vertCount];

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

        // Convert Vector2[] points to Vector2[] for EdgeCollider2D (same type)
        // Just assign directly since points is already Vector2[]
        Debug.Log("UpdateCollider");
        edgeCollider.points = points;
    }

    private void OnDrawGizmos()
    {
        VisualizePoints();
    }

    //------------ Getter and Setter ------------
    public float GetWidth() { return width; }
}