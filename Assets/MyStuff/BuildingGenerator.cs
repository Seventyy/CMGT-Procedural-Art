using Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : Shape
{
    [Header("References")]
    [SerializeField] private GameObject GridSnappedBoxes;
    [SerializeField] private GridSnappedBox blondsVisualizer;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] wallSegments;
    [SerializeField] private GameObject[] wallSeparators;
    [SerializeField] private GameObject[] verandaSegments;
    [SerializeField] private GameObject[] floorSeparators;
    [SerializeField] private GameObject[] hipSegments;
    [SerializeField] private GameObject[] HipCornerOuters;
    [SerializeField] private GameObject[] HipCornerInners;

    private GridSnappedBox[] gridSnappedBoxes;

    enum CellType
    {
        Outside,
        Inside,
        Roof,
    }

    private Vector3Int buildSpaceOrigin;
    private CellType[,,] buildSpace; // padded build space

    protected override void Execute()    
    {
        RecalculateSpace();
        BuildWalls();
        BuildWallSeparators();
    }

    public void RecalculateSpace()
    {
        gridSnappedBoxes = GridSnappedBoxes.GetComponentsInChildren<GridSnappedBox>();

        Tuple<Vector3Int, Vector3Int> bounds = CalculateBounds();

        blondsVisualizer.SetBoundsVisualiser(bounds.Item1 - Vector3Int.one, bounds.Item2 + Vector3Int.one);

        buildSpace = new CellType[bounds.Item2.x - bounds.Item1.x + 2, bounds.Item2.y - bounds.Item1.y + 2, bounds.Item2.z - bounds.Item1.z + 2];
        buildSpaceOrigin = bounds.Item1 - Vector3Int.one;

        // zero out
        for (int i = 0; i < buildSpace.GetLength(0); i++)
        {
            for (int j = 0; j < buildSpace.GetLength(1); j++)
            {
                for (int k = 0; k < buildSpace.GetLength(2); k++)
                {
                    buildSpace[i, j, k] = CellType.Outside;
                }
            }
        }

        for (int i = 0; i < buildSpace.GetLength(0); i++)
        {
            for (int j = 0; j < buildSpace.GetLength(1); j++)
            {
                for (int k = 0; k < buildSpace.GetLength(2); k++)
                {
                    foreach(GridSnappedBox gridSnappedBox in gridSnappedBoxes)
                    {
                        if (gridSnappedBox.ContainsGrided(buildSpaceOrigin + new Vector3Int(i, j, k)))
                        {
                            buildSpace[i, j, k] = CellType.Inside;
                        }
                    }
                }
            }
        }
    }

    private Tuple<Vector3Int, Vector3Int> CalculateBounds()
    {
        Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

        foreach (GridSnappedBox gridSnappedBox in gridSnappedBoxes)
        {
            gridSnappedBox.FixFlipped();
            Vector3Int gridedMin = gridSnappedBox.gridedMin;
            Vector3Int gridedMax = gridSnappedBox.gridedMax;

            min = Vector3Int.Min(min, gridedMin);
            max = Vector3Int.Max(max, gridedMax);
        }

        return new Tuple<Vector3Int, Vector3Int>(min, max);
    }

    private void BuildWalls()
    {
        for (int i = 0; i < buildSpace.GetLength(0) - 0; i++)
        {
            for (int j = 0; j < buildSpace.GetLength(1) - 0; j++)
            {
                for (int k = 0; k < buildSpace.GetLength(2) - 0; k++)
                {
                    Vector3Int relativeGridPosition = new Vector3Int(i, j, k);
                    Vector3Int absoluteGridPosition = new Vector3Int(i, j, k) + buildSpaceOrigin;
                    Vector3 absolutePosition = GridSnappedBox.GridedToSnapped(new Vector3Int(i, j, k) + buildSpaceOrigin);

                    if (buildSpace[i,j,k] == CellType.Outside)
                    {
                        if (i + 1 < buildSpace.GetLength(0) &&
                            buildSpace[i + 1, j, k] == CellType.Inside)
                        {
                            SpawnPrefab(wallSegments[0], absolutePosition + new Vector3(GridSnappedBox.grid.x, 0, GridSnappedBox.grid.z / 2), Quaternion.Euler(0, -90, 0));
                            SpawnPrefab(floorSeparators[0], absolutePosition + new Vector3(GridSnappedBox.grid.x, GridSnappedBox.grid.y, GridSnappedBox.grid.z / 2), Quaternion.Euler(0, -90, 0));
                        }
                        if (i - 1 >= 0 &&
                            buildSpace[i - 1, j, k] == CellType.Inside)
                        {
                            SpawnPrefab(wallSegments[0], absolutePosition + new Vector3(0, 0, GridSnappedBox.grid.z / 2), Quaternion.Euler(0, 90, 0));
                            SpawnPrefab(floorSeparators[0], absolutePosition + new Vector3(0, GridSnappedBox.grid.y, GridSnappedBox.grid.z / 2), Quaternion.Euler(0, 90, 0));
                        }
                        if (k + 1 < buildSpace.GetLength(2) &&
                            buildSpace[i, j, k + 1] == CellType.Inside)
                        {
                            SpawnPrefab(wallSegments[0], absolutePosition + new Vector3(GridSnappedBox.grid.x / 2, 0, GridSnappedBox.grid.z), Quaternion.Euler(0, 180, 0));
                            SpawnPrefab(floorSeparators[0], absolutePosition + new Vector3(GridSnappedBox.grid.x / 2, GridSnappedBox.grid.y, GridSnappedBox.grid.z), Quaternion.Euler(0, 180, 0));
                        }
                        if (k - 1 >= 0 &&
                            buildSpace[i, j, k - 1] == CellType.Inside)
                        {
                            SpawnPrefab(wallSegments[0], absolutePosition + new Vector3(GridSnappedBox.grid.x / 2, 0, 0), Quaternion.Euler(0, 0, 0));
                            SpawnPrefab(floorSeparators[0], absolutePosition + new Vector3(GridSnappedBox.grid.x / 2, GridSnappedBox.grid.y, 0), Quaternion.Euler(0, 0, 0));
                        }
                    }
                }
            }
        }
    }

    private void BuildWallSeparators()
    {
        for (int i = 0; i < buildSpace.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < buildSpace.GetLength(1) - 1; j++)
            {
                for (int k = 0; k < buildSpace.GetLength(2) - 1; k++)
                {
                    Vector3Int relativeGridPosition = new Vector3Int(i, j, k);
                    Vector3Int absoluteGridPosition = new Vector3Int(i, j, k) + buildSpaceOrigin;
                    Vector3 absolutePosition = GridSnappedBox.GridedToSnapped(new Vector3Int(i, j, k) + buildSpaceOrigin);

                    // Check if the current cell and its quadrants are inside or outside
                    int cell_sum = 
                        (int)buildSpace[i, j, k] +
                        (int)buildSpace[i + 1, j, k] +
                        (int)buildSpace[i, j, k + 1] +
                        (int)buildSpace[i + 1, j, k + 1];



                    if (cell_sum != 0 && cell_sum != 4)
                    {
                        SpawnPrefab(wallSeparators[0], absolutePosition + new Vector3(GridSnappedBox.grid.x, 0, GridSnappedBox.grid.z), Quaternion.identity);
                    }
                }
            }
        }
    }





    //[UnityEditor.CustomEditor(typeof(BuildingGenerator))]
    //public class BuildingGeneratorEditor : UnityEditor.Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        DrawDefaultInspector();

    //        BuildingGenerator buildingGenerator = (BuildingGenerator)target;
    //        if (GUILayout.Button("Recalculate Space"))
    //        {
    //            buildingGenerator.RecalculateSpace();
    //        }
    //    }
    //}

}





























//[Header("References")]
////[SerializeField] private FloorShapeEditor[] floorShapeEditors;

//[Header("Modules Constants")]
//[SerializeField] public float wallWidth = 2f;
//[SerializeField] public float wallHeight = 2.5f;

//[Header("Prefabs")]
//[SerializeField] private GameObject[] wallSegments;
//[SerializeField] private GameObject[] wallSeparators;
//[SerializeField] private GameObject[] verandaSegments;
//[SerializeField] private GameObject[] floorSeparators;
//[SerializeField] private GameObject[] hipSegments;
//[SerializeField] private GameObject[] HipCornerOuters;
//[SerializeField] private GameObject[] HipCornerInners;

////[Header("Shapes")]
////[SerializeField] private Vector2Int[] shapePolygon;
////[SerializeField] private Vector2Int[] verandaEdge;


////[Header("Other")]
////[SerializeField] private int floorCount = 1;

////public int FloorCount
////{
////    get { return floorCount; }
////    set
////    {
////        floorCount = value;
////        floorShapes.CreateFloors(floorCount);
////    }
////}

//private Vector2Int[][] floorPolygons;

//protected override void Execute()
//{
//    //UpdateFloorPolygons();
//    GenerateWalls();
//    GenerateVeranda();
//    //GenerateRoofSmall();
//}

////private void UpdateFloorPolygons()
////{
////    List<Vector2Int[]> newFloorPolygons = new List<Vector2Int[]>();

////    foreach (FloorShapeEditor floorShapeEditor in floorShapeEditors)
////    {
////        List<Vector2Int> newShapePolygon = new List<Vector2Int>();
////        foreach (Vector3 point in floorShapeEditor.corners)
////        {
////            newShapePolygon.Add(new Vector2Int((int)(point.x / wallWidth), (int)(point.z / wallWidth)));
////        }
////        newFloorPolygons.Add(newShapePolygon.ToArray());
////    }

////    floorPolygons = newFloorPolygons.ToArray();
////}

//private void GenerateWalls()
//{
//    for (int floorIndex = 0; floorIndex < floorPolygons.Length; floorIndex++)
//    {
//        Vector2Int[] floorPolygon = floorPolygons[floorIndex];
//        Tuple<Vector2Int, Vector2Int>[] sides = new Tuple<Vector2Int, Vector2Int>[floorPolygon.Length];

//        for (int i = 0; i < floorPolygon.Length; i++)
//        {
//            sides[i] = new Tuple<Vector2Int, Vector2Int>(floorPolygon[i], floorPolygon[(i + 1) % floorPolygon.Length]);
//        }

//        foreach (Tuple<Vector2Int, Vector2Int> side in sides)
//        {
//            Vector2Int unit_start = side.Item1;
//            Vector2Int unit_end = side.Item2;

//            Vector2Int unit_direction = unit_end - unit_start;

//            Vector3 start = new Vector3(unit_start.x * wallWidth, 0, unit_start.y * wallWidth);
//            Vector3 end = new Vector3(unit_end.x * wallWidth, 0, unit_end.y * wallWidth);

//            Vector3 direction = end - start;
//            Vector3 normal = new Vector3(direction.z, 0, -direction.x);

//            int segmentCount = (int)unit_direction.magnitude;

//            for (int i = 0; i < segmentCount; i++)
//            {
//                SpawnPrefab(wallSeparators[0], start + i * direction.normalized * wallWidth + Vector3.up * floorIndex * wallHeight);
//                SpawnPrefab(wallSegments[0], start + (i + .5f) * direction.normalized * wallWidth + Vector3.up * floorIndex * wallHeight,
//                    Quaternion.LookRotation(normal, Vector3.up));
//            }
//        }
//    }
//}

//private void GenerateVeranda()
//{
//    //Vector2Int unit_start = verandaEdge[0];
//    //Vector2Int unit_end = verandaEdge[1];

//    //Vector2Int unit_direction = unit_end - unit_start;

//    //Vector3 start = new Vector3(unit_start.x * wallWidth, 0, unit_start.y * wallWidth);
//    //Vector3 end = new Vector3(unit_end.x * wallWidth, 0, unit_end.y * wallWidth);

//    //Vector3 direction = end - start;
//    //Vector3 normal = new Vector3(direction.z, 0, -direction.x);

//    //int segmentCount = (int)unit_direction.magnitude;

//    //for (int i = 0; i < segmentCount; i++)
//    //{
//    //    SpawnPrefab(verandaSegments[0], start + (i + .5f) * direction.normalized * wallWidth,
//    //        Quaternion.LookRotation(normal, Vector3.up));
//    //}
//}

//private void GenerateRoofSmall()
//{

//    for (int floorIndex = 0; floorIndex < floorPolygons.Length; floorIndex++)
//    {
//        Vector2Int[] floorPolygon = floorPolygons[floorIndex];
//        Tuple<Vector2Int, Vector2Int>[] sides = new Tuple<Vector2Int, Vector2Int>[floorPolygon.Length];

//        for (int i = 0; i < floorPolygon.Length; i++)
//        {
//            sides[i] = new Tuple<Vector2Int, Vector2Int>(floorPolygon[i], floorPolygon[(i + 1) % floorPolygon.Length]);
//        }

//        foreach (Tuple<Vector2Int, Vector2Int> side in sides)
//        {
//            Vector2Int unit_start = side.Item1;
//            Vector2Int unit_end = side.Item2;

//            Vector2Int unit_direction = unit_end - unit_start;

//            Vector3 start = new Vector3(unit_start.x * wallWidth, wallHeight * 1.5f, unit_start.y * wallWidth);
//            Vector3 end = new Vector3(unit_end.x * wallWidth, wallHeight * 1.5f, unit_end.y * wallWidth);

//            Vector3 direction = end - start;
//            Vector3 normal = new Vector3(direction.z, 0, -direction.x).normalized;

//            int segmentCount = (int)unit_direction.magnitude;

//            for (int i = 0; i < segmentCount; i++)
//            {

//                SpawnPrefab(hipSegments[0],
//                    start + (i + .5f) * direction.normalized * wallWidth - normal * wallWidth / 2 +
//                        Vector3.up * floorIndex * wallHeight,
//                    Quaternion.LookRotation(normal, Vector3.up));
//            }
//        }
//    }
//}

//public class WaveFunctionCollapse
//{
//    private int[,] grid;
//    private int gridSize;
//    private int patternSize;
//    private int[,] patterns;
//    private bool[,] wave;

//    public WaveFunctionCollapse(int[,] grid, int patternSize, int[,] patterns)
//    {
//        this.grid = grid;
//        this.gridSize = grid.GetLength(0);
//        this.patternSize = patternSize;
//        this.patterns = patterns;
//        this.wave = new bool[gridSize, gridSize];
//    }

//    public int[,] Run()
//    {
//        InitializeWave();

//        while (!IsWaveCollapsed())
//        {
//            int[] cell = FindUncollapsedCell();
//            if (cell == null)
//            {
//                throw new Exception("No uncollapsed cell found");
//            }

//            CollapseCell(cell);
//            PropagateConstraints(cell);
//        }

//        return grid;
//    }

//    private void InitializeWave()
//    {
//        for (int i = 0; i < gridSize; i++)
//        {
//            for (int j = 0; j < gridSize; j++)
//            {
//                wave[i, j] = true;
//            }
//        }
//    }

//    private bool IsWaveCollapsed()
//    {
//        for (int i = 0; i < gridSize; i++)
//        {
//            for (int j = 0; j < gridSize; j++)
//            {
//                if (wave[i, j])
//                {
//                    return false;
//                }
//            }
//        }
//        return true;
//    }

//    private int[] FindUncollapsedCell()
//    {
//        for (int i = 0; i < gridSize; i++)
//        {
//            for (int j = 0; j < gridSize; j++)
//            {
//                if (wave[i, j])
//                {
//                    return new int[] { i, j };
//                }
//            }
//        }
//        return null;
//    }

//    private void CollapseCell(int[] cell)
//    {
//        int x = cell[0];
//        int y = cell[1];

//        List<int> possiblePatterns = new List<int>();

//        for (int patternIndex = 0; patternIndex < patterns.GetLength(0); patternIndex++)
//        {
//            bool patternMatches = true;

//            for (int i = 0; i < patternSize; i++)
//            {
//                for (int j = 0; j < patternSize; j++)
//                {
//                    int gridX = x + i - patternSize / 2;
//                    int gridY = y + j - patternSize / 2;

//                    if (gridX < 0 || gridX >= gridSize || gridY < 0 || gridY >= gridSize)
//                    {
//                        continue;
//                    }

//                    if (wave[gridX, gridY] && grid[gridX, gridY] != patterns[patternIndex, i * patternSize + j])
//                    {
//                        patternMatches = false;
//                        break;
//                    }
//                }

//                if (!patternMatches)
//                {
//                    break;
//                }
//            }

//            if (patternMatches)
//            {
//                possiblePatterns.Add(patternIndex);
//            }
//        }

//        if (possiblePatterns.Count == 0)
//        {
//            throw new Exception("No possible pattern found for cell");
//        }

//        int chosenPattern = possiblePatterns[UnityEngine.Random.Range(0, possiblePatterns.Count)];
//        for (int i = 0; i < patternSize; i++)
//        {
//            for (int j = 0; j < patternSize; j++)
//            {
//                int gridX = x + i - patternSize / 2;
//                int gridY = y + j - patternSize / 2;

//                if (gridX < 0 || gridX >= gridSize || gridY < 0 || gridY >= gridSize)
//                {
//                    continue;
//                }

//                if (wave[gridX, gridY])
//                {
//                    wave[gridX, gridY] = false;
//                    grid[gridX, gridY] = patterns[chosenPattern, i * patternSize + j];
//                }
//            }
//        }
//    }

//    private void PropagateConstraints(int[] cell)
//    {
//        int x = cell[0];
//        int y = cell[1];

//        for (int i = 0; i < patternSize; i++)
//        {
//            for (int j = 0; j < patternSize; j++)
//            {
//                int gridX = x + i - patternSize / 2;
//                int gridY = y + j - patternSize / 2;

//                if (gridX < 0 || gridX >= gridSize || gridY < 0 || gridY >= gridSize)
//                {
//                    continue;
//                }

//                if (!wave[gridX, gridY])
//                {
//                    continue;
//                }

//                for (int patternIndex = 0; patternIndex < patterns.GetLength(0); patternIndex++)
//                {
//                    if (grid[gridX, gridY] == patterns[patternIndex, i * patternSize + j])
//                    {
//                        bool patternMatches = true;

//                        for (int k = 0; k < patternSize; k++)
//                        {
//                            if (k == i)
//                            {
//                                continue;
//                            }

//                            int neighborGridX = gridX + k - patternSize / 2;

//                            if (neighborGridX < 0 || neighborGridX >= gridSize)
//                            {
//                                continue;
//                            }

//                            if (wave[neighborGridX, gridY] && grid[neighborGridX, gridY] != patterns[patternIndex, k * patternSize + j])
//                            {
//                                patternMatches = false;
//                                break;
//                            }
//                        }

//                        if (patternMatches)
//                        {
//                            for (int k = 0; k < patternSize; k++)
//                            {
//                                if (k == j)
//                                {
//                                    continue;
//                                }

//                                int neighborGridY = gridY + k - patternSize / 2;

//                                if (neighborGridY < 0 || neighborGridY >= gridSize)
//                                {
//                                    continue;
//                                }

//                                if (wave[gridX, neighborGridY] && grid[gridX, neighborGridY] != patterns[patternIndex, i * patternSize + k])
//                                {
//                                    patternMatches = false;
//                                    break;
//                                }
//                            }
//                        }

//                        if (!patternMatches)
//                        {
//                            wave[gridX, gridY] = false;
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
