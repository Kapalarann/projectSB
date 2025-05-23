using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Node Settings")]
    public int nodeCount = 10;
    public float nodeMinDistance = 3f;
    public GameObject nodePrefab;

    [Header("Line Settings")]
    public GameObject linePrefab;

    [Header("Map Size")]
    public Vector2 mapMin = new Vector2(-10, -10);
    public Vector2 mapMax = new Vector2(10, 10);

    private List<MapNode> nodes = new();
    private List<(MapNode, MapNode)> connections = new();

    private void Start()
    {
        GenerateNodes();
        GenerateConnections();
        AddExtraConnections(Mathf.FloorToInt(nodeCount * 0.3f));
        SpawnVisuals();
    }

    void GenerateNodes()
    {
        int maxAttempts = 100;

        for (int i = 0; i < nodeCount; i++)
        {
            Vector2 pos;
            int attempts = 0;
            bool valid;

            do
            {
                pos = new Vector2(Random.Range(mapMin.x, mapMax.x), Random.Range(mapMin.y, mapMax.y));
                valid = true;

                foreach (var other in nodes)
                {
                    if (Vector2.Distance(other.position, pos) < nodeMinDistance)
                    {
                        valid = false;
                        break;
                    }
                }

                attempts++;
            } while (!valid && attempts < maxAttempts);

            if (valid)
            {
                nodes.Add(new MapNode { position = pos });
            }
        }
    }

    void GenerateConnections()
    {
        HashSet<MapNode> connected = new();
        connected.Add(nodes[0]);

        while (connected.Count < nodes.Count)
        {
            float bestDist = float.MaxValue;
            MapNode bestA = null;
            MapNode bestB = null;

            foreach (var a in connected)
            {
                foreach (var b in nodes)
                {
                    if (connected.Contains(b)) continue;

                    float dist = Vector2.Distance(a.position, b.position);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestA = a;
                        bestB = b;
                    }
                }
            }

            if (bestA != null && bestB != null)
            {
                bestA.neighbors.Add(bestB);
                bestB.neighbors.Add(bestA);
                connections.Add((bestA, bestB));
                connected.Add(bestB);
            }
        }
    }

    void AddExtraConnections(int count)
    {
        int attempts = 0;
        int added = 0;
        int maxAttempts = 100;

        while (added < count && attempts < maxAttempts)
        {
            var a = nodes[Random.Range(0, nodes.Count)];
            var b = nodes[Random.Range(0, nodes.Count)];

            if (a != b && !a.neighbors.Contains(b))
            {
                a.neighbors.Add(b);
                b.neighbors.Add(a);
                connections.Add((a, b));
                added++;
            }

            attempts++;
        }
    }

    void SpawnVisuals()
    {
        foreach (var node in nodes)
        {
            Vector3 pos3D = new Vector3(node.position.x, 0, node.position.y);
            node.orbGO = Instantiate(nodePrefab, pos3D, Quaternion.identity, transform);
        }

        foreach (var (a, b) in connections)
        {
            GameObject lineGO = Instantiate(linePrefab, transform);
            LineRenderer lr = lineGO.GetComponent<LineRenderer>();

            if (lr)
            {
                Vector3 p1 = new Vector3(a.position.x, 0, a.position.y);
                Vector3 p2 = new Vector3(b.position.x, 0, b.position.y);

                lr.positionCount = 2;
                lr.SetPosition(0, p1);
                lr.SetPosition(1, p2);
            }
        }
    }

    public class MapNode
    {
        public Vector2 position;
        public GameObject orbGO;
        public List<MapNode> neighbors = new();
    }
}