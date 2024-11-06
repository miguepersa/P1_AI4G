using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public Vector3 Position { get; private set; }  // Tile position in the grid
    public bool isWalkable = true;

    public Node(Vector3 position)
    {
        Position = position;
    }
}

public class Connection
{
    public Node FromNode { get; private set; }
    public Node ToNode { get; private set; }
    private float cost;

    public Connection(Node fromNode, Node toNode)
    {
        FromNode = fromNode;
        ToNode = toNode;
        this.cost = (ToNode.Position - FromNode.Position).magnitude;
    }

    public float GetCost()
    {
        return cost;
    }
}

public class Graph
{
    private Dictionary<Node, List<Connection>> adjacencyList = new Dictionary<Node, List<Connection>>();

    public void AddNode(Node node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = new List<Connection>();
        }
    }
}

