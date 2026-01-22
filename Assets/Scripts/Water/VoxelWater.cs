using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelWater : MonoBehaviour
{
    [Header("Grid Settings")]
    // Keep these reasonable! 32x32x32 is approx 32k voxels. 
    // 64x64x64 is 260k voxels (very heavy).
    public int width = 32;
    public int height = 32;
    public int depth = 32;
    public float cellSize = 1.0f;

    [Header("Physics Settings")]
    [Range(0.1f, 1.0f)] public float flowSpeed = 1.0f;
    public float tickRate = 0.05f; // Updates physics every 0.05s
    public LayerMask obstacleLayer;

    // Voxel Data
    private float[,,] liquid;
    private float[,,] nextLiquid;
    private bool[,,] solids;

    // Mesh Data
    private Mesh mesh;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private float timer;
    private bool isDirty = true; // Does the mesh need an update?

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        // 1. Tick System: Don't run physics every single frame
        timer += Time.deltaTime;
        if (timer >= tickRate)
        {
            SimulatePhysics();
            timer = 0;
        }

        // 2. Dirty Flag: Only rebuild mesh if something actually changed
        if (isDirty)
        {
            GenerateMesh();
            isDirty = false;
        }
    }

    public void Initialize()
    {
        liquid = new float[width, height, depth];
        nextLiquid = new float[width, height, depth];
        solids = new bool[width, height, depth];

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        BakeObstacles();
    }

    public void BakeObstacles()
    {
        Vector3 origin = transform.position;
        float halfSize = cellSize / 2f;

        // Optimization: Don't use CheckBox if grid is huge, but for <64 it's okay
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 center = origin + new Vector3(x * cellSize + halfSize, y * cellSize + halfSize, z * cellSize + halfSize);
                    solids[x, y, z] = Physics.CheckBox(center, Vector3.one * (halfSize - 0.05f), Quaternion.identity, obstacleLayer);
                }
            }
        }
        Debug.Log($"Initialized {width}x{height}x{depth} grid.");
    }

    public void AddWater(Vector3 worldPosition, float amount)
    {
        Vector3 localPos = worldPosition - transform.position;
        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int y = Mathf.FloorToInt(localPos.y / cellSize);
        int z = Mathf.FloorToInt(localPos.z / cellSize);

        if (IsInside(x, y, z) && !solids[x, y, z])
        {
            liquid[x, y, z] += amount;
            if (liquid[x, y, z] > 1.5f) liquid[x, y, z] = 1.5f;

            // Important: Tell the system we need to redraw
            isDirty = true;
        }
    }

    private void SimulatePhysics()
    {
        bool liquidMoved = false; // Track if anything actually happened

        System.Array.Copy(liquid, nextLiquid, liquid.Length);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    float currentWater = liquid[x, y, z];
                    if (currentWater <= 0.001f) continue;

                    // 1. Over-pressure (Up)
                    if (currentWater > 1.0f)
                    {
                        float excess = currentWater - 1.0f;
                        if (IsInside(x, y + 1, z) && !solids[x, y + 1, z])
                        {
                            nextLiquid[x, y + 1, z] += excess;
                            nextLiquid[x, y, z] -= excess;
                            currentWater = 1.0f;
                            liquidMoved = true;
                        }
                    }

                    // 2. Gravity (Down)
                    if (IsInside(x, y - 1, z) && !solids[x, y - 1, z])
                    {
                        float spaceBelow = 1.0f - liquid[x, y - 1, z];
                        if (spaceBelow > 0)
                        {
                            float flow = Mathf.Min(currentWater, spaceBelow);
                            nextLiquid[x, y - 1, z] += flow;
                            nextLiquid[x, y, z] -= flow;
                            liquidMoved = true;
                            continue;
                        }
                    }

                    // 3. Dispersion (Sides)
                    if (nextLiquid[x, y, z] > 0)
                    {
                        if (DistributeToNeighbors(x, y, z, nextLiquid[x, y, z]))
                        {
                            liquidMoved = true;
                        }
                    }
                }
            }
        }

        System.Array.Copy(nextLiquid, liquid, liquid.Length);

        // Only trigger a mesh rebuild if liquid actually moved
        if (liquidMoved) isDirty = true;
    }

    private bool DistributeToNeighbors(int x, int y, int z, float amount)
    {
        bool moved = false;
        int[][] dirs = { new int[] { 1, 0 }, new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 0, -1 } };
        float flow = amount * 0.25f * flowSpeed;

        foreach (var dir in dirs)
        {
            int nx = x + dir[0];
            int nz = z + dir[1];

            if (IsInside(nx, y, nz) && !solids[nx, y, nz])
            {
                if (liquid[nx, y, nz] < amount)
                {
                    float diff = amount - liquid[nx, y, nz];
                    float transfer = Mathf.Min(flow, diff / 2f);

                    if (transfer > 0.001f) // Optimization: Ignore tiny movements
                    {
                        nextLiquid[nx, y, nz] += transfer;
                        nextLiquid[x, y, z] -= transfer;
                        moved = true;
                    }
                }
            }
        }
        return moved;
    }

    private void GenerateMesh()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        int vertIndex = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    float level = liquid[x, y, z];
                    if (level <= 0.01f) continue;

                    Vector3 pos = new Vector3(x, y, z) * cellSize;
                    bool hasWaterAbove = IsInside(x, y + 1, z) && liquid[x, y + 1, z] > 0.01f;
                    float drawHeight = hasWaterAbove ? 1.0f : Mathf.Clamp01(level);
                    bool isSolidAbove = IsInside(x, y + 1, z) && solids[x, y + 1, z];

                    // TOP FACE
                    if (!hasWaterAbove && !isSolidAbove)
                    {
                        AddFace(pos + new Vector3(0, drawHeight, 0), pos + new Vector3(0, drawHeight, 1),
                                pos + new Vector3(1, drawHeight, 1), pos + new Vector3(1, drawHeight, 0), ref vertIndex);
                    }

                    // SIDES (Pass optimization data)
                    CheckAndDrawFace(x, y, z, x - 1, y, z, pos, 0, drawHeight, ref vertIndex);
                    CheckAndDrawFace(x, y, z, x + 1, y, z, pos, 1, drawHeight, ref vertIndex);
                    CheckAndDrawFace(x, y, z, x, y, z - 1, pos, 2, drawHeight, ref vertIndex);
                    CheckAndDrawFace(x, y, z, x, y, z + 1, pos, 3, drawHeight, ref vertIndex);

                    // BOTTOM FACE
                    if (IsInside(x, y - 1, z) && liquid[x, y - 1, z] <= 0.01f && !solids[x, y - 1, z])
                    {
                        AddFace(pos + new Vector3(0, 0, 1), pos + new Vector3(0, 0, 0),
                                pos + new Vector3(1, 0, 0), pos + new Vector3(1, 0, 1), ref vertIndex);
                    }
                }
            }
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
    }

    // Helper functions remain mostly the same
    private void CheckAndDrawFace(int x, int y, int z, int nx, int ny, int nz, Vector3 pos, int faceDir, float height, ref int idx)
    {
        bool draw = true;
        if (IsInside(nx, ny, nz))
        {
            if (solids[nx, ny, nz]) draw = false;
            else if (liquid[nx, ny, nz] > 0.99f) draw = false;
            else if (liquid[nx, ny, nz] > 0.01f) draw = false;
        }

        if (draw)
        {
            float h = height;
            Vector3 p0 = Vector3.zero, p1 = Vector3.zero, p2 = Vector3.zero, p3 = Vector3.zero;
            switch (faceDir)
            {
                case 0: p0 = pos; p1 = pos + Vector3.forward; p2 = pos + new Vector3(0, h, 1); p3 = pos + new Vector3(0, h, 0); break;
                case 1: p0 = pos + new Vector3(1, 0, 1); p1 = pos + Vector3.right; p2 = pos + new Vector3(1, h, 0); p3 = pos + new Vector3(1, h, 1); break;
                case 2: p0 = pos + Vector3.right; p1 = pos; p2 = pos + new Vector3(0, h, 0); p3 = pos + new Vector3(1, h, 0); break;
                case 3: p0 = pos + Vector3.forward; p1 = pos + new Vector3(1, 0, 1); p2 = pos + new Vector3(1, h, 1); p3 = pos + new Vector3(0, h, 1); break;
            }
            AddFace(p0, p1, p2, p3, ref idx);
        }
    }

    private void AddFace(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, ref int idx)
    {
        vertices.Add(v0); vertices.Add(v1); vertices.Add(v2); vertices.Add(v3);
        uvs.Add(new Vector2(0, 0)); uvs.Add(new Vector2(1, 0)); uvs.Add(new Vector2(1, 1)); uvs.Add(new Vector2(0, 1));
        triangles.Add(idx); triangles.Add(idx + 1); triangles.Add(idx + 2);
        triangles.Add(idx); triangles.Add(idx + 2); triangles.Add(idx + 3);
        idx += 4;
    }

    private bool IsInside(int x, int y, int z) => x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth;
}