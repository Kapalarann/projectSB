using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConeVisualizer : MonoBehaviour
{
    public float range = 5f;
    public float angle = 45f;
    public int segments = 20;
    public Material coneMaterial;

    [Header("References")]
    private Mesh coneMesh;
    public PlayerMovement movement;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = coneMesh = new Mesh();
        GetComponent<MeshRenderer>().material = coneMaterial;
        GenerateConeMesh();
    }

    private void Update()
    {
        Vector3 moveDir = movement.movementValue;
        if (moveDir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }

    void GenerateConeMesh()
    {
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float step = angle * 2f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float theta = Mathf.Deg2Rad * (-angle + step * i);
            vertices[i + 1] = new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta)) * range;
        }

        for (int i = 0; i < segments; i++)
        {
            int baseIdx = i * 3;
            triangles[baseIdx] = 0;
            triangles[baseIdx + 1] = i + 1;
            triangles[baseIdx + 2] = i + 2;
        }

        coneMesh.Clear();
        coneMesh.vertices = vertices;
        coneMesh.triangles = triangles;
        coneMesh.RecalculateNormals();
    }
}
