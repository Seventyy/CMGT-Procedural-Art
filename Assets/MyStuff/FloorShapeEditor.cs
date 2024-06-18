using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class FloorShapeEditor : MonoBehaviour
{
    [SerializeField] private BuildingGenerator buildingGenerator;

    public Vector3[] corners { get; private set; }

    private void OnDrawGizmos()
    {
        UpdateGizmos();
        DrawCorners();
        DrawPaths();
    }

    private void UpdateGizmos()
    {
        List<Vector3> newCorners = new List<Vector3>();
        foreach (Transform child in transform)
        {
            child.name = "Corner " + newCorners.Count;

            Vector3 snappedPosition = new Vector3(
                Mathf.Round(child.position.x / buildingGenerator.wallWidth) * buildingGenerator.wallWidth,
                Mathf.Round(child.position.y / buildingGenerator.wallHeight) * buildingGenerator.wallHeight,
                Mathf.Round(child.position.z / buildingGenerator.wallWidth) * buildingGenerator.wallWidth
            );

            newCorners.Add(snappedPosition);
        }
        corners = newCorners.ToArray();
    }


    private void DrawCorners()
    {
        Gizmos.color = Color.yellow;
        foreach (Vector3 corner in corners)
        {
            if (corner != null)
                Gizmos.DrawSphere(corner, .25f);
        }
    }

    private void DrawPaths()
    {
        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.color = Color.yellow;
            int next_i = (i + 1) % corners.Length;
            if (corners[i] != null && corners[next_i] != null)
            {
                Vector3 thisCorner = corners[i];
                Vector3 nextCorner = corners[next_i];

                if (thisCorner.x != nextCorner.x && thisCorner.z != nextCorner.z)
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawLine(thisCorner, nextCorner);
            }
        }
    }
}
