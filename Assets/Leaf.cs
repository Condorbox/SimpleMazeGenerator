using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf
{
    private int xPos;
    private int zPos;
    private int width;
    private int depth;
    private float scale;

    private int roomMin = 5;
    private int minWallSize = 1;
    private int maxWallSize = 3;

    private Leaf leftChild;
    private Leaf rightChild;

    public Leaf(int x, int z, int width, int depth, float scale)
    {
        this.xPos = x;
        this.zPos = z;
        this.width = width;
        this.depth = depth;
        this.scale = scale;
    }

    public bool Split()
    {
        if (width <= roomMin || depth <= roomMin)
        {
            return false;
        }

        bool splitHorizontal = Random.Range(0, 100) > 50;
        if (width > depth && width / depth >= 1.2)
        {
            splitHorizontal = false;
        }
        else if (depth > width && depth / width >= 1.2)
        {
            splitHorizontal = true;
        }

        int max = (splitHorizontal ? depth : width) - roomMin;
        if (max <= roomMin)
        {
            return false;
        }

        int childCut = Random.Range(roomMin, max);
        if (splitHorizontal)
        {
            leftChild = new Leaf(xPos, zPos, width, childCut, scale);
            rightChild = new Leaf(xPos, zPos + childCut, width, depth - childCut, scale);
        }
        else
        {
            leftChild = new Leaf(xPos, zPos, childCut, depth, scale);
            rightChild = new Leaf(xPos + childCut, zPos, width - childCut, depth, scale);
        }

        return true;
    }

    public void UpdateMap(byte[,] map)
    {
        int wallSize = Random.Range(minWallSize, maxWallSize);
        for (int x = xPos + wallSize; x < width + xPos - wallSize; x++)
        {
            for (int z = zPos + wallSize; z < depth + zPos - wallSize; z++)
            {
                map[x, z] = 0;
            }
        }
    }

    public Leaf GetLeftChild()
    {
        return leftChild;
    }

    public Leaf GetRightChild()
    {
        return rightChild;
    }

    public int GetXPos()
    {
        return xPos;
    }

    public int GetZPos()
    {
        return zPos;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetDepth()
    {
        return depth;
    }

    public int GetRoomMin()
    {
        return roomMin;
    }
}
