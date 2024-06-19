using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class GridSnappedBox : MonoBehaviour
{
    [SerializeField] static public Vector3 grid { get; private set; } = new Vector3(2f, 2.5f, 2f);

    public Vector3Int gridedMin { get; private set; }
    public Vector3Int gridedMax { get; private set; }

    public Vector3 snappedMin { get; private set; }
    public Vector3 snappedMax { get; private set; }

    private bool boundsVisualiser = false;

    public static Vector3 GridedToSnapped(Vector3Int grided)
    {
        return new Vector3(
            grided.x * grid.x,
            grided.y * grid.y,
            grided.z * grid.z
        );
    }

    public static Vector3 SnappedToGrided(Vector3 snapped)
    {
        return new Vector3Int(
            (int)(snapped.x / grid.x),
            (int)(snapped.y / grid.y),
            (int)(snapped.z / grid.z)
        );
    }

    public void SetBoundsVisualiser(Vector3Int _grided_min, Vector3Int _grided_max)
    {
        boundsVisualiser = true;
        if (transform.childCount != 2)
        {
            while (transform.childCount < 2)
            {
                GameObject corner = new GameObject();
                corner.transform.SetParent(transform);
            }
            while (transform.childCount > 2)
            {
                Transform child = transform.GetChild(2);
                child.SetParent(null);
                DestroyImmediate(child.gameObject);
            }
        }

        transform.GetChild(0).transform.position = new Vector3(
            (_grided_min.x * grid.x),
            (_grided_min.y * grid.y),
            (_grided_min.z * grid.z)
        );

        transform.GetChild(1).transform.position = new Vector3(
            (_grided_max.x * grid.x),
            (_grided_max.y * grid.y),
            (_grided_max.z * grid.z)
        );
    }

    private void OnDrawGizmos()
    {
        UpdateCorners();
        DrawCorners();
        DrawEdges();
    }

    private void UpdateCorners()
    {
        transform.position = Vector3.zero;
        if (transform.childCount != 2)
        {
            while (transform.childCount < 2)
            {
                GameObject corner = new GameObject();
                corner.transform.SetParent(transform);
            }
            while (transform.childCount > 2)
            {
                Transform child = transform.GetChild(2);
                child.SetParent(null);
                DestroyImmediate(child.gameObject);
            }
        }

        Transform t_min = transform.GetChild(0);
        Transform t_max = transform.GetChild(1);

        t_min.name = "Min";
        t_max.name = "Max";


        Vector3 t_minSnappedPosition = new Vector3(
            Mathf.Round(t_min.position.x / grid.x) * grid.x,
            Mathf.Round(t_min.position.y / grid.y) * grid.y,
            Mathf.Round(t_min.position.z / grid.z) * grid.z
        );

        Vector3 t_maxSnappedPosition = new Vector3(
            Mathf.Round(t_max.position.x / grid.x) * grid.x,
            Mathf.Round(t_max.position.y / grid.y) * grid.y,
            Mathf.Round(t_max.position.z / grid.z) * grid.z
        );

        t_min.position = t_minSnappedPosition;
        t_max.position = t_maxSnappedPosition;

        snappedMin = t_minSnappedPosition;
        snappedMax = t_maxSnappedPosition;

        Vector3Int t_minGridedPosition = new Vector3Int(
            (int)(t_minSnappedPosition.x / grid.x),
            (int)(t_minSnappedPosition.y / grid.y),
            (int)(t_minSnappedPosition.z / grid.z)
        );

        Vector3Int t_maxGridedPosition = new Vector3Int(
            (int)(t_maxSnappedPosition.x / grid.x),
            (int)(t_maxSnappedPosition.y / grid.y),
            (int)(t_maxSnappedPosition.z / grid.z)
        );

        gridedMin = t_minGridedPosition;
        gridedMax = t_maxGridedPosition;
    }


    private void DrawCorners()
    {
        Gizmos.color = Color.yellow;

        if (!IsValid()) 
            Gizmos.color = Color.red;
        if (boundsVisualiser) 
            return;

        Vector3[] corners = new Vector3[]
        {
            new Vector3(snappedMin.x, snappedMin.y, snappedMin.z),
            new Vector3(snappedMin.x, snappedMin.y, snappedMax.z),
            new Vector3(snappedMax.x, snappedMin.y, snappedMax.z),
            new Vector3(snappedMax.x, snappedMin.y, snappedMin.z),
            new Vector3(snappedMin.x, snappedMax.y, snappedMin.z),
            new Vector3(snappedMin.x, snappedMax.y, snappedMax.z),
            new Vector3(snappedMax.x, snappedMax.y, snappedMax.z),
            new Vector3(snappedMax.x, snappedMax.y, snappedMin.z),
        };

        foreach (Vector3 corner in corners)
        {
            if (corner != null)
                Gizmos.DrawSphere(corner, .25f);
        }
    }

    private void DrawEdges()
    {
        Gizmos.color = Color.yellow;

        if (!IsValid())
            Gizmos.color = Color.red;

        if (boundsVisualiser)
            Gizmos.color = Color.blue;

        Vector3[] corners = new Vector3[]
{
            new Vector3(snappedMin.x, snappedMin.y, snappedMin.z),
            new Vector3(snappedMin.x, snappedMin.y, snappedMax.z),
            new Vector3(snappedMax.x, snappedMin.y, snappedMax.z),
            new Vector3(snappedMax.x, snappedMin.y, snappedMin.z),
            new Vector3(snappedMin.x, snappedMax.y, snappedMin.z),
            new Vector3(snappedMin.x, snappedMax.y, snappedMax.z),
            new Vector3(snappedMax.x, snappedMax.y, snappedMax.z),
            new Vector3(snappedMax.x, snappedMax.y, snappedMin.z),
};

        Tuple<Vector3, Vector3>[] edges = new Tuple<Vector3, Vector3>[]
        {
            new Tuple<Vector3, Vector3>(corners[0], corners[1]),
            new Tuple<Vector3, Vector3>(corners[1], corners[2]),
            new Tuple<Vector3, Vector3>(corners[2], corners[3]),
            new Tuple<Vector3, Vector3>(corners[3], corners[0]),

            new Tuple<Vector3, Vector3>(corners[4], corners[5]),
            new Tuple<Vector3, Vector3>(corners[5], corners[6]),
            new Tuple<Vector3, Vector3>(corners[6], corners[7]),
            new Tuple<Vector3, Vector3>(corners[7], corners[4]),

            new Tuple<Vector3, Vector3>(corners[0], corners[4]),
            new Tuple<Vector3, Vector3>(corners[1], corners[5]),
            new Tuple<Vector3, Vector3>(corners[2], corners[6]),
            new Tuple<Vector3, Vector3>(corners[3], corners[7]),
        };

        foreach (Tuple<Vector3, Vector3> edge in edges)
        {
            Gizmos.DrawLine(edge.Item1, edge.Item2);
        }
    }

    private bool IsValid()
    {
        return !(snappedMin.x == snappedMax.x || snappedMin.y == snappedMax.y || snappedMin.z == snappedMax.z);
    }

    public void FixFlipped()
    {
        Vector3 t_min = transform.GetChild(0).position;
        Vector3 t_max = transform.GetChild(1).position;

        if (snappedMin.x > snappedMax.x)
        {
            float temp = t_min.x;
            t_min.x = t_max.x;
            t_max.x = temp;
        }
        if (snappedMin.y > snappedMax.y)
        {
            float temp = t_min.y;
            t_min.y = t_max.y;
            t_max.y = temp;
        }
        if (snappedMin.z > snappedMax.z)
        {
            float temp = t_min.z;
            t_min.z = t_max.z;
            t_max.z = temp;
        }

        transform.GetChild(0).position = t_min;
        transform.GetChild(1).position = t_max;
        
        UpdateCorners();
    }

    public bool ContainsSnapped(Vector3 point)
    {
        return point.x >= snappedMin.x && point.x < snappedMax.x &&
               point.y >= snappedMin.y && point.y < snappedMax.y &&
               point.z >= snappedMin.z && point.z < snappedMax.z;
    }

    public bool ContainsGrided(Vector3Int point)
    {
        return point.x >= gridedMin.x && point.x < gridedMax.x &&
               point.y >= gridedMin.y && point.y < gridedMax.y &&
               point.z >= gridedMin.z && point.z < gridedMax.z;
    }
}

//public class FloorShapeEditor : MonoBehaviour
//{
//    [SerializeField] private BuildingGenerator buildingGenerator;

//    public Vector3[] corners { get; private set; }

//    private void OnDrawGizmos()
//    {
//        UpdateGizmos();
//        DrawCorners();
//        DrawPaths();
//    }

//    private void UpdateGizmos()
//    {
//        List<Vector3> newCorners = new List<Vector3>();
//        foreach (Transform child in transform)
//        {
//            child.name = "Corner " + newCorners.Count;

//            Vector3 snappedPosition = new Vector3(
//                Mathf.Round(child.position.x / buildingGenerator.wallWidth) * buildingGenerator.wallWidth,
//                Mathf.Round(child.position.y / buildingGenerator.wallHeight) * buildingGenerator.wallHeight,
//                Mathf.Round(child.position.z / buildingGenerator.wallWidth) * buildingGenerator.wallWidth
//            );

//            newCorners.Add(snappedPosition);
//        }
//        corners = newCorners.ToArray();
//    }


//    private void DrawCorners()
//    {
//        Gizmos.color = Color.yellow;
//        foreach (Vector3 corner in corners)
//        {
//            if (corner != null)
//                Gizmos.DrawSphere(corner, .25f);
//        }
//    }

//    private void DrawPaths()
//    {
//        for (int i = 0; i < corners.Length; i++)
//        {
//            Gizmos.color = Color.yellow;
//            int next_i = (i + 1) % corners.Length;
//            if (corners[i] != null && corners[next_i] != null)
//            {
//                Vector3 thisCorner = corners[i];
//                Vector3 nextCorner = corners[next_i];

//                if (thisCorner.x != nextCorner.x && thisCorner.z != nextCorner.z)
//                {
//                    Gizmos.color = Color.red;
//                }

//                Gizmos.DrawLine(thisCorner, nextCorner);
//            }
//        }
//    }
//}
