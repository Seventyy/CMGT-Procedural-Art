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

    [SerializeField] private Vector2Int[] shapePolygon;

    [SerializeField] private GameObject[] wallSegments;
    [SerializeField] private GameObject[] wallSeparators;


    protected override void Execute()
    {
        GenerateWalls();

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
}
