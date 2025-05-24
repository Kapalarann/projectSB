using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Gen Settings")]
    public int nodeCount = 15;
    public Vector2 mapSize = new(20, 20);
    public float extraEdgePercent = 0f;

    [Header("Connection Constraints")]
    [SerializeField] private float minDistanceBetweenNodes = 3f;
    [SerializeField] private float maxDistanceBetweenNodes = 10f;

    [Header("References")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject roadPrefab;

    private List<Node> nodes = new();
    private List<Edge> triangulatedEdges = new();
    private List<Edge> mstEdges = new();
    private GameObject[] edges = new GameObject[99];

    void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ClearMap();
            GenerateMap();
        }
    }

    void ClearMap()
    {
        foreach (Node node in nodes)
        {
            Destroy(node.obj);
        }
        foreach (GameObject edge in edges)
        {
            Destroy(edge);
        }
        nodes.Clear();
        triangulatedEdges.Clear();
        mstEdges.Clear();
    }

    void GenerateMap()
    {
        GeneratePoints();
        GenerateAllEdges();
        ComputeMST();
        AddExtraEdges(extraEdgePercent);
        DrawGraph();
    }

    void GeneratePoints()
    {
        nodes.Clear();

        // 1. Add the first node at this object's transform position
        Vector2 origin2D = new Vector2(transform.position.x, transform.position.z); // Assume Y is up
        nodes.Add(new Node(origin2D));

        int attempts = 0;
        int maxAttempts = 1000;

        while (nodes.Count < nodeCount && attempts < maxAttempts)
        {
            Vector2 candidate = new Vector2(
                Random.Range(-mapSize.x, mapSize.x),
                Random.Range(-mapSize.y, mapSize.y)
            );

            bool tooClose = false;
            bool hasNeighbor = false;

            foreach (var existing in nodes)
            {
                float distance = Vector2.Distance(existing.position, candidate);

                if (distance < minDistanceBetweenNodes)
                {
                    tooClose = true;
                    break;
                }

                if (distance <= maxDistanceBetweenNodes)
                {
                    hasNeighbor = true;
                }
            }

            // Now always skip if too close to any, and require at least one neighbor (except the first, already added)
            if (!tooClose && hasNeighbor)
            {
                nodes.Add(new Node(candidate));
            }

            attempts++;
        }

        if (nodes.Count < nodeCount)
            Debug.LogWarning($"Only generated {nodes.Count} nodes after {attempts} attempts. Try adjusting map size or distance limits.");
    }



    void GenerateAllEdges()
    {
        triangulatedEdges.Clear();

        // Fully connect each point (naive Delaunay-like)
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = i + 1; j < nodes.Count; j++)
            {
                float dist = Vector2.Distance(nodes[i].position, nodes[j].position);
                triangulatedEdges.Add(new Edge(nodes[i], nodes[j], dist));
            }
        }

        // Sort by distance to simulate natural graph behavior
        triangulatedEdges.Sort((a, b) => a.distance.CompareTo(b.distance));
    }

    void ComputeMST()
    {
        mstEdges.Clear();
        DisjointSet<Node> ds = new();

        foreach (var node in nodes)
            ds.MakeSet(node);

        foreach (var edge in triangulatedEdges)
        {
            if (ds.Find(edge.a) != ds.Find(edge.b))
            {
                mstEdges.Add(edge);
                ds.Union(edge.a, edge.b);
            }

            if (mstEdges.Count >= nodes.Count - 1)
                break;
        }
    }

    void AddExtraEdges(float percentage)
    {
        int extras = Mathf.FloorToInt(triangulatedEdges.Count * percentage);
        int added = 0;

        foreach (var edge in triangulatedEdges)
        {
            if (mstEdges.Contains(edge)) continue;
            mstEdges.Add(edge);
            added++;

            if (added >= extras)
                break;
        }
    }

    void DrawGraph()
    {
        foreach (var node in nodes)
        {
            var go = Instantiate(nodePrefab, new Vector3(node.position.x, 0, node.position.y), Quaternion.identity, transform);
            node.obj = go;
        }

        int i = 0;
        foreach (var edge in mstEdges)
        {
            Vector3 start = new Vector3(edge.a.position.x, 0, edge.a.position.y);
            Vector3 end = new Vector3(edge.b.position.x, 0, edge.b.position.y);
            Vector3 direction = end - start;
            float distance = direction.magnitude;
            Vector3 midPoint = start + direction * 0.5f;

            GameObject road = Instantiate(roadPrefab, midPoint, Quaternion.identity, transform);
            road.transform.forward = direction.normalized;
            road.transform.localScale = new Vector3(
                road.transform.localScale.x,
                road.transform.localScale.y,
                distance / 10
            );

            edges[i] = road;
            i++;
        }
    }

    public class Node
    {
        public Vector2 position;
        public GameObject obj;
        public Node(Vector2 pos) { position = pos; }
    }

    public class Edge
    {
        public Node a, b;
        public float distance;
        public Edge(Node a, Node b, float d) { this.a = a; this.b = b; distance = d; }
    }

    public class DisjointSet<T>
    {
        Dictionary<T, T> parent = new();

        public void MakeSet(T item) => parent[item] = item;

        public T Find(T item)
        {
            if (!parent.ContainsKey(item)) return item;
            if (!EqualityComparer<T>.Default.Equals(parent[item], item))
                parent[item] = Find(parent[item]);
            return parent[item];
        }

        public void Union(T a, T b)
        {
            T rootA = Find(a);
            T rootB = Find(b);
            if (!EqualityComparer<T>.Default.Equals(rootA, rootB))
                parent[rootB] = rootA;
        }
    }
}