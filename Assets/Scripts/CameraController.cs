using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 30f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 10, -10);
    [SerializeField] private float maxDistance = 20f;

    private List<Transform> players = new List<Transform>();
    private Camera cam;
    private int prevCount = 0;

    private void Start()
    {
        cam = GetComponent<Camera>();
        FindPlayers();
        prevCount = players.Count;
    }

    private void LateUpdate()
    {
        if(PlayerMovement.Players.Count != prevCount)
        {
            FindPlayers();
            prevCount = players.Count;
        }

        if (players.Count == 0) return;
        
        MoveCamera();
        AdjustZoom();
    }

    private void FindPlayers()
    {
        players = new List<Transform>();
        foreach (var player in PlayerMovement.Players)
        { 
            players.Add(player.transform);
        }
    }

    private void MoveCamera()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 desiredPosition = centerPoint + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }

    private void AdjustZoom()
    {
        float greatestDistance = GetGreatestDistance();

        float desiredZoom = Mathf.Lerp(minZoom, maxZoom, greatestDistance / maxDistance);

        if (cam.orthographic) cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, desiredZoom, zoomSpeed * Time.deltaTime);
        else cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredZoom, zoomSpeed * Time.deltaTime);
    }

    private Vector3 GetCenterPoint()
    {
        if (players.Count == 1) return players[0].position;

        Vector3 totalPosition = Vector3.zero;
        foreach (var player in players)
        {
            totalPosition += player.position;
        }
        return totalPosition / players.Count;
    }

    private float GetGreatestDistance()
    {
        if (players.Count <= 1)
            return 0f;

        Vector3 minPosition = players[0].position;
        Vector3 maxPosition = players[0].position;

        foreach (var player in players)
        {
            Vector3 position = player.position;
            minPosition = Vector3.Min(minPosition, position);
            maxPosition = Vector3.Max(maxPosition, position);
        }

        Vector3 difference = maxPosition - minPosition;
        return Mathf.Max(difference.x, difference.z);
    }
    
}
