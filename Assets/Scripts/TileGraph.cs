using System.Collections.Generic;
using UnityEngine;

public class TileGraph : Graph
{
    private int tileWidth;
    private int tileHeight;
    public Node[,] nodes;
    public LayerMask obstacleLayer = LayerMask.GetMask("Obstacle"); // Define an obstacle layer mask

    public TileGraph(int width, int height) : base()
    {
        
        tileWidth = width;
        tileHeight = height;
        nodes = new Node[width, height];
    }

    public void InitializeTiles()
    {
        for (int x = -(tileWidth / 2); x < tileWidth / 2; x++)
        {
            for (int y = -(tileHeight / 2); y < (tileHeight / 2); y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                Node node = new Node(position);

                // Check if there is an obstacle at this tile position
                if (Physics.CheckSphere(position, 0.1f, obstacleLayer))
                {
                    node.isWalkable = false;
                }

                nodes[x + (tileWidth / 2), y + (tileHeight / 2)] = node;
                AddNode(node);
            }
        }
    }


    public List<Connection> GetConnections(Node fromNode)
    {

        List<Connection> connections = new List<Connection>();
        for (int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                int newNodeX = (int) fromNode.Position.x + i + (tileWidth / 2);
                int newNodeY = (int) fromNode.Position.z + j + (tileHeight / 2);

                // Ensure the new position is within bounds
                if (newNodeX >= 0 && newNodeX < tileWidth && newNodeY >= 0 && newNodeY < tileHeight)
                {
                    Node toNode = nodes[newNodeX, newNodeY];
                    if (toNode.isWalkable)
                    {
                        connections.Add(new Connection(fromNode, toNode));
                    }
                }
            }
        }

        return connections;
    }
}
