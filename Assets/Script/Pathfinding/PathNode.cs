using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public bool obstacle = false;

    public int costFromStartModule = 0;
    public int costToReachEndModule = 0;
    public int fullCost = 0;

    public Vector3 worldPosition;

    public int x = 0;
    public int y = 0;

    public PathNode cameFromNode;

    public PathNode(int x, int y, Vector3 position)
    {
        this.x = x;
        this.y = y;
        this.worldPosition = position;
    }

    public void Reset()
    {
        costFromStartModule = 0;
        costToReachEndModule = 0;
        fullCost = 0;
        cameFromNode = null;
    }




}
