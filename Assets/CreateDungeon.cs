using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDungeon : MonoBehaviour
{
    [SerializeField] private int mapWidth = 50;
    [SerializeField] private int mapDepth = 50;
    [SerializeField] private float mapScale = 2f;
    [SerializeField] private int treeLevel = 6;

    private Leaf root;
    private byte[,] map;
    List<Vector2> corridors = new List<Vector2>();
    [SerializeField] private int ExtraCorridors = 7;
    private GameObject container;

    private void Awake()
    {
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        container = new GameObject("Maze Container");
        root = new Leaf(0, 0, mapWidth, mapDepth, mapScale);
        corridors = new List<Vector2>();
        map = InitializeMap(mapWidth, mapDepth);

        BSP(root, treeLevel);
        AddCorridors();
        AddRandomCorridors(ExtraCorridors);
        DrawMap();
    }

    public void ReGenerateMaze()
    {
        Destroy(container);
        GenerateMaze();
    }

    private byte[,] InitializeMap(int width, int depth)
    {
        byte[,] map = new byte[width, depth];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                map[x, z] = 1;
            }
        }

        return map;
    }

    private void BSP(Leaf root, int sDepth)
    {
        if (root == null) return;
        if (sDepth <= 0)
        {
            root.UpdateMap(map);
            corridors.Add(new Vector2(root.GetXPos() + root.GetWidth() / 2, root.GetZPos() + root.GetDepth() / 2));
            return;
        }

        if (root.Split())
        {
            BSP(root.GetLeftChild(), sDepth - 1);
            BSP(root.GetRightChild(), sDepth - 1);
        }
        else
        {
            root.UpdateMap(map);
            corridors.Add(new Vector2(root.GetXPos() + root.GetWidth() / 2, root.GetZPos() + root.GetDepth() / 2));
        }
    }

    private void AddCorridors()
    {
        if (corridors.Count <= 1) return;
        for (int i = 1; i < corridors.Count; i++)
        {
            if ((int)corridors[i].x == (int)corridors[i - 1].x || (int)corridors[i].y == (int)corridors[i - 1].y)
            {
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i - 1].x, (int)corridors[i - 1].y);
            }
            else
            {
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i].x, (int)corridors[i - 1].y);
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i - 1].x, (int)corridors[i].y);
            }
        }
    }

    private void AddRandomCorridors(int extraCorridors)
    {
        for (int i = 0; i < extraCorridors; i++)
        {
            int startX = Random.Range(root.GetRoomMin(), mapWidth - root.GetRoomMin());
            int startZ = Random.Range(root.GetRoomMin(), mapDepth - root.GetRoomMin());
            int length = Random.Range(root.GetRoomMin(), mapWidth - root.GetRoomMin());

            if (Random.Range(0, 500) < 50)
            {
                line(startX, startZ, length, startZ);
            }
            else
            {
                line(startX, startZ, startX, length);
            }
        }
    }

    //Adapted Bresenham's line algorithm
    //https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
    private void line(int x, int y, int x2, int y2)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            map[x, y] = 0;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

    private void DrawMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapDepth; z++)
            {
                if (map[x, z] == 1)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x * mapScale, 10, z * mapScale);
                    cube.transform.localScale = new Vector3(mapScale, mapScale, mapScale);

                    cube.transform.parent = container.transform;
                }
            }
        }
    }
}
