using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinder : MonoBehaviour
{

    

    const int MOVE_STRAIGHT_COST = 10;
    const int MOVE_DIAGONAL_COST = 14;
    const float RADIUS_OF_OBSTACLE_IGNORANCE = 0.2f;

    [SerializeField] private int width = 72;
    [SerializeField] private int height = 66;

    private PathGrid pathGrid;

    private PathNode startNode;
    private PathNode endNode;


    void Start()
    {
        pathGrid = new PathGrid(width, height, 1, RADIUS_OF_OBSTACLE_IGNORANCE);
    }

    public List<PathNode> FindPath(Vector3 startPos, Vector3 endPosition, float debugShowTime = 0)
    {
        startNode = pathGrid.WorldPosition_To_GridCell(new Vector3(startPos.x, startPos.y));
        endNode = pathGrid.WorldPosition_To_GridCell(new Vector3(endPosition.x, endPosition.y));

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        foreach (PathNode pathNode in pathGrid.nodeArray)
        {
            if (!pathNode.obstacle)
            {
                pathNode.Reset();
                openList.Add(pathNode);

                pathNode.costFromStartModule = int.MaxValue;

                CalculateFullCost(pathNode);
                pathNode.cameFromNode = null;
            }
        }

        startNode.costFromStartModule = 0;
        startNode.costToReachEndModule = CalculateDistanceCost(startNode, endNode);
        CalculateFullCost(startNode);

        int iterationCounter = 0;

        PathNode first = GetLowestFullCostNode(openList);

        while (openList.Count > 0)
        {
            iterationCounter++;
            PathNode currentNode = GetLowestFullCostNode(openList);
            //PathGridNode currentNode = startNode;

            if(currentNode == endNode)
            {
                return CalculatePath(currentNode, debugShowTime);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in pathGrid.GetNodeNeighbours(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                int tentativeFromStartCost = currentNode.costFromStartModule + CalculateDistanceCost(currentNode, neighbourNode);
                if(tentativeFromStartCost < neighbourNode.costFromStartModule)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.costFromStartModule = tentativeFromStartCost;
                    neighbourNode.costToReachEndModule = CalculateDistanceCost(neighbourNode, endNode);
                    CalculateFullCost(neighbourNode);

                    if (!openList.Contains(neighbourNode)){
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;

    }

    private List<PathNode> CalculatePath(PathNode endNode, float debugShowTime)
    {
        bool debugging = (debugShowTime > 0);
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            if (debugging)
            {
                UnityEngine.Debug.DrawLine(currentNode.worldPosition, currentNode.cameFromNode.worldPosition, Color.red, 3f);
            }
            currentNode = currentNode.cameFromNode;
            path.Add(currentNode);
        }
        
        return path;
    }

    private PathNode GetLowestFullCostNode(List<PathNode> nodeList)
    {
        PathNode lowestFCostNode = nodeList[0];
        foreach (PathNode node in nodeList)
        {
            if (node.fullCost < lowestFCostNode.fullCost)
            {
                lowestFCostNode = node;
            }
        }
        return lowestFCostNode;
    }

    private void InitializeNode(PathNode node)
    {
        node.cameFromNode = null;

        SetMax_CostFromStartModule(node);
        CalculateFullCost(node);
    }

    private void SetMax_CostFromStartModule(PathNode node)
    {
        node.costFromStartModule = int.MaxValue;
    }

    private void CalculateFullCost(PathNode node)
    {
        node.fullCost = node.costFromStartModule + node.costToReachEndModule;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
}
