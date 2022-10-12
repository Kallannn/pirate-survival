using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathGrid
{
    private int width;
    private int height;
    private float cellSize;
    public PathNode[,] nodeArray;


    public PathGrid(int width, int height, float cellSize, float marginOfObstacleIgnorance = 0f)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        nodeArray = new PathNode[width,height];

        for(int x = 0; x < nodeArray.GetLength(0); x++)
        {
            for(int y = 0; y < nodeArray.GetLength(1); y++)
            {
                Vector3 position = GridCell_To_WorldPosition(x, y);
                var obj = new PathNode(x, y, position);

                CheckForObsacle(obj, marginOfObstacleIgnorance);

                nodeArray[x,y] = obj;

            }
        }

        stopWatch.Stop();

        UnityEngine.Debug.Log($"Finished mapping {height * width} path modules in {stopWatch.ElapsedMilliseconds}ms");
    }

    public Vector3 GridCell_To_WorldPosition(int x, int y)
    {
        Vector3 gridOffset = new Vector3(width * cellSize / 2, height * cellSize / 2) * -1;
        Vector3 moduleOffset = new Vector3(cellSize, cellSize) / 2;
        return new Vector3(x, y) * cellSize + gridOffset + moduleOffset;
    }

    public PathNode WorldPosition_To_GridCell(Vector3 position)
    {
        Vector3 gridOffset = new Vector3(width * cellSize / 2, height * cellSize / 2) * -1;
        Vector3 moduleOffset = new Vector3(cellSize, cellSize) / 2;

        int nodeX = (int)((position.x - gridOffset.x) / cellSize);
        int nodeY = (int)((position.y - gridOffset.y) / cellSize);

        if(nodeX > width-1)
        {
            string errorTxt = ($"The position {position} resulted in coordinate [{nodeX},{nodeY}] which is out of the array on the X.");
            throw new System.Exception(errorTxt);
        }
        else if (nodeX > width - 1)
        {
            string errorTxt = ($"The position {position} resulted in coordinate [{nodeX},{nodeY}] which is out of the arrey on the Y.");
            throw new System.Exception(errorTxt);
        }
        return nodeArray[nodeX, nodeY];
    }

    private void CheckForObsacle(PathNode node, float marginOfObstacleIgnorance)
    {
        int layermaskFilter = LayerMask.GetMask("TerrainObstacles");
        Vector3 position = GridCell_To_WorldPosition(node.x, node.y);

        Vector2 radiusOfObstacleDetection = new Vector2(cellSize - marginOfObstacleIgnorance, cellSize - marginOfObstacleIgnorance);

        if (Physics2D.OverlapBox(position, radiusOfObstacleDetection, 0, layermaskFilter) != null)
        {
            node.obstacle = true;
        }
    }

    public List<PathNode> GetNodeNeighbours(PathNode node)
    {
        List<PathNode> neighbours = new List<PathNode>(); 
        int debugNumber = 0;
        try
        {
            
            if (node.x - 1 >= 0)
            {
                debugNumber = 1;
                neighbours.Add(nodeArray[node.x - 1, node.y]);

                if (node.y - 1 >= 0)
                {
                    debugNumber = 2;
                    neighbours.Add(nodeArray[node.x - 1, node.y - 1]);
                }

                if (node.y + 1 < height)
                {
                    debugNumber = 3;
                    neighbours.Add(nodeArray[node.x - 1, node.y + 1]);
                }
            }
            if (node.x + 1 < width)
            {
                debugNumber = 4;
                neighbours.Add(nodeArray[node.x + 1, node.y]);

                if (node.y - 1 >= 0)
                {
                    debugNumber = 5;
                    neighbours.Add(nodeArray[node.x + 1, node.y - 1]);
                }

                if (node.y + 1 < height)
                {
                    debugNumber = 6;
                    neighbours.Add(nodeArray[node.x + 1, node.y + 1]);
                }
            }

            if (node.y - 1 >= 0) neighbours.Add(nodeArray[node.x, node.y - 1]);

            if (node.y + 1 < height) neighbours.Add(nodeArray[node.x, node.y + 1]);
        }
        catch (System.Exception)
        {
            throw new System.Exception($"Trouble getting neighbors. Node: [{node.x}, {node.y}] ; debug number: {debugNumber};");
        }

        return neighbours;
    }
}
