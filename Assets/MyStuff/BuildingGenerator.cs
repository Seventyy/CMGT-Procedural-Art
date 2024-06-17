using Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : Shape
{
    [Header("Modules Constants")]
    [SerializeField] private float wallWidth = 2f;
    [SerializeField] private float wallHeight = 2.5f;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] wallSegments;
    [SerializeField] private GameObject[] wallSeparators;
    [SerializeField] private GameObject[] verandaSegments;
    [SerializeField] private GameObject[] hipSegments;
    [SerializeField] private GameObject[] HipCornerOuters;
    [SerializeField] private GameObject[] HipCornerInners;

    [Header("Shapes")]
    [SerializeField] private Vector2Int[] shapePolygon;
    [SerializeField] private Vector2Int[] verandaEdge;

    protected override void Execute()
    {
        GenerateWalls();
        GenerateVeranda();
        GenerateRoofSmall();
    }

    private void GenerateWalls()
    {
        Tuple<Vector2Int, Vector2Int>[] sides = new Tuple<Vector2Int, Vector2Int>[shapePolygon.Length];

        for (int i = 0; i < shapePolygon.Length; i++)
        {
            sides[i] = new Tuple<Vector2Int, Vector2Int>(shapePolygon[i], shapePolygon[(i + 1) % shapePolygon.Length]);
        }

        foreach (Tuple<Vector2Int, Vector2Int> side in sides)
        {
            Vector2Int unit_start = side.Item1;
            Vector2Int unit_end = side.Item2;

            Vector2Int unit_direction = unit_end - unit_start;

            Vector3 start = new Vector3(unit_start.x * wallWidth, 0, unit_start.y * wallWidth);
            Vector3 end = new Vector3(unit_end.x * wallWidth, 0, unit_end.y * wallWidth);

            Vector3 direction = end - start;
            Vector3 normal = new Vector3(direction.z, 0, -direction.x);

            int segmentCount = (int)unit_direction.magnitude;

            for (int i = 0; i < segmentCount; i++)
            {
                SpawnPrefab(wallSeparators[0], start + i * direction.normalized * wallWidth);
                SpawnPrefab(wallSegments[0], start + (i + .5f) * direction.normalized * wallWidth,
                    Quaternion.LookRotation(normal, Vector3.up));
            }
        }
    }

    private void GenerateVeranda()
    {
        Vector2Int unit_start = verandaEdge[0];
        Vector2Int unit_end = verandaEdge[1];

        Vector2Int unit_direction = unit_end - unit_start;

        Vector3 start = new Vector3(unit_start.x * wallWidth, 0, unit_start.y * wallWidth);
        Vector3 end = new Vector3(unit_end.x * wallWidth, 0, unit_end.y * wallWidth);

        Vector3 direction = end - start;
        Vector3 normal = new Vector3(direction.z, 0, -direction.x);

        int segmentCount = (int)unit_direction.magnitude;

        for (int i = 0; i < segmentCount; i++)
        {
            SpawnPrefab(verandaSegments[0], start + (i + .5f) * direction.normalized * wallWidth,
                Quaternion.LookRotation(normal, Vector3.up));
        }
    }

    private void GenerateRoofSmall()
    {
        Tuple<Vector2Int, Vector2Int>[] sides = new Tuple<Vector2Int, Vector2Int>[shapePolygon.Length];

        for (int i = 0; i < shapePolygon.Length; i++)
        {
            sides[i] = new Tuple<Vector2Int, Vector2Int>(shapePolygon[i], shapePolygon[(i + 1) % shapePolygon.Length]);
        }

        foreach (Tuple<Vector2Int, Vector2Int> side in sides)
        {
            Vector2Int unit_start = side.Item1;
            Vector2Int unit_end = side.Item2;

            Vector2Int unit_direction = unit_end - unit_start;

            Vector3 start = new Vector3(unit_start.x * wallWidth, wallHeight * 1.5f, unit_start.y * wallWidth);
            Vector3 end = new Vector3(unit_end.x * wallWidth, wallHeight * 1.5f, unit_end.y * wallWidth);

            Vector3 direction = end - start;
            Vector3 normal = new Vector3(direction.z, 0, -direction.x).normalized;

            int segmentCount = (int)unit_direction.magnitude;

            for (int i = 0; i < segmentCount; i++)
            {

                SpawnPrefab(hipSegments[0], start + (i + .5f) * direction.normalized * wallWidth - normal * wallWidth / 2,
                    Quaternion.LookRotation(normal, Vector3.up));
            }
        }
    }
}
