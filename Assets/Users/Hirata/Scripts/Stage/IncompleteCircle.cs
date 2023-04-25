using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncompleteCircle : MonoBehaviour
{
    public Transform playerTransform;
    public Transform circleCenter;
    public float circleRadius;
    public MeshFilter circleMesh;

    private void Update()
    {
        // Check if player is inside the circle
        if (IsPlayerInsideCircle())
        {
            Debug.Log("Player is inside the circle");
            // Perform some action when player is inside the circle
        }
        else
        {
            Debug.Log("Player is outside the circle");
            // Perform some action when player is outside the circle
        }
    }

    private bool IsPlayerInsideCircle()
    {
        // Convert circle mesh to world space
        Mesh circleWorldMesh = circleMesh.mesh;
        circleWorldMesh.RecalculateBounds();
        circleWorldMesh.RecalculateNormals();
        circleWorldMesh.RecalculateTangents();

        Matrix4x4 transformMatrix = circleCenter.localToWorldMatrix;

        Vector3[] worldVertices = circleWorldMesh.vertices;
        for (int i = 0; i < worldVertices.Length; i++)
        {
            worldVertices[i] = transformMatrix.MultiplyPoint(worldVertices[i]);
        }

        // Check if player is inside any of the triangle faces of the circle mesh
        int[] triangles = circleWorldMesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v1 = worldVertices[triangles[i]];
            Vector3 v2 = worldVertices[triangles[i + 1]];
            Vector3 v3 = worldVertices[triangles[i + 2]];

            // Calculate normal vector of the triangle face
            Vector3 faceNormal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

            // Check if player is on the opposite side of the face normal vector
            Vector3 playerToCenter = circleCenter.position - playerTransform.position;
            if (Vector3.Dot(faceNormal, playerToCenter) > 0)
            {
                continue;
            }

            // Calculate distance from player to the plane of the triangle face
            float distance = Vector3.Dot(faceNormal, v1 - playerTransform.position) / faceNormal.magnitude;

            // Check if player is inside the circle
            if (distance <= circleRadius)
            {
                return true;
            }
        }

        return false;
    }
}
