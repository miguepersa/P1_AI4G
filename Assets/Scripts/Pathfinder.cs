using UnityEngine;
using System.Collections.Generic;
using System;

public class Pathfinder: MonoBehaviour
{
    public int gridWidth = 100;
    public int gridHeight = 100;
    private TileGraph tileGraph;

    void Start()
    {
        tileGraph = new TileGraph(gridWidth, gridHeight);
        tileGraph.InitializeTiles();
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 endPos)
    {
        Node startNode = tileGraph.nodes[(int)startPos.x + 50, (int)startPos.z + 50];
        Node endNode = tileGraph.nodes[(int)endPos.x + 50, (int)endPos.z + 50];

        return AStar(startNode, endNode);
    }

    private List<Node> AStar(Node startNode, Node goalNode)
    {
        List<Node> openList = new List<Node> { startNode };
        HashSet<Node> closedList = new HashSet<Node>();

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gCost = new Dictionary<Node, float> { [startNode] = 0 };
        Dictionary<Node, float> fCost = new Dictionary<Node, float> { [startNode] = Heuristic(startNode, goalNode) };

        while (openList.Count > 0)
        {
            // Get the node with the lowest fCost
            Node currentNode = openList[0];

            foreach (Node node in openList)
            {
                if (fCost[node] < fCost[currentNode])
                    currentNode = node;
            }

            if (currentNode == goalNode)
                return ReconstructPath(cameFrom, currentNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);
            List<Connection> connections = tileGraph.GetConnections(currentNode);

            foreach (Connection connection in connections)
            {
                Node neighbor = connection.ToNode;
                if (closedList.Contains(neighbor)) continue;

                float tentativeGCost = gCost[currentNode] + connection.GetCost();

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);

                else if (tentativeGCost >= gCost[neighbor])
                    continue;

                cameFrom[neighbor] = currentNode;
                gCost[neighbor] = tentativeGCost;
                fCost[neighbor] = gCost[neighbor] + Heuristic(neighbor, goalNode);
            }
        }

        return new List<Node>(); // Return empty path if no path is found
    }

    private float Heuristic(Node a, Node b)
    {
        // Use Manhattan distance for a grid-based map
        return (a.Position - b.Position).magnitude;
    }

    private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node currentNode)
    {
        List<Node> path = new List<Node>();
        while (cameFrom.ContainsKey(currentNode))
        {
            path.Add(currentNode);
            currentNode = cameFrom[currentNode];
        }
        path.Reverse();
        return path;
    }

    void OnDrawGizmos()
    {
        // Only draw the gizmos if the tileGraph has been initialized
        if (tileGraph?.nodes != null)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Node node = tileGraph.nodes[x, y];
                    Gizmos.color = node.isWalkable ? Color.green : Color.red;

                    // Draw a cube at each node's position
                    Vector3 nodePosition = new Vector3(node.Position.x, -0.49f, node.Position.z);
                    Gizmos.DrawWireCube(nodePosition, Vector3.one * 1f);
                }
            }
        }
    }
}