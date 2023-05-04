using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

public class Functions : MonoBehaviour
{
    public List<Vector2Int> AStar(Vector2Int _current, Vector2Int _goal)
    {
        List<Node> openList = new List<Node> { new Node(_current, 0, ManhattanDistance(_current, _goal), null) };
        List<Node> closedList = new List<Node>();

        while (openList.Count > 0)
        {
            Node currentNode = openList.OrderBy(node => node.FScore).First();

            if (currentNode.Position == _goal)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                Node node = currentNode;
                while (node != null)
                {
                    path.Add(node.Position);
                    node = node.Parent;
                }
                path.Reverse();
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Vector2Int neighbor in GetNeighbors(currentNode.Position))
            {
                if (closedList.Any(node => node.Position == neighbor))
                {
                    continue;
                }

                int gScore = currentNode.GScore + 1;

                Node neighborNode = openList.FirstOrDefault(node => node.Position == neighbor);
                if (neighborNode == null)
                {
                    neighborNode = new Node(neighbor, gScore, ManhattanDistance(neighbor, _goal), currentNode);
                    openList.Add(neighborNode);
                }
                else if (gScore < neighborNode.GScore)
                {
                    neighborNode.GScore = gScore;
                    neighborNode.Parent = currentNode;
                }
            }
        }
        return new List<Vector2Int>();
    }

    public List<Vector2Int> AStar(Vector2Int _current, Vector2Int _goal, Tilemap _tilemap)
    {
        List<Node> openList = new List<Node> { new Node(_current, 0, ManhattanDistance(_current, _goal), null) };
        List<Node> closedList = new List<Node>();

        while (openList.Count > 0)
        {
            Node currentNode = openList.OrderBy(node => node.FScore).First();

            if (currentNode.Position == _goal)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                Node node = currentNode;
                while (node != null)
                {
                    path.Add(node.Position);
                    node = node.Parent;
                }
                path.Reverse();
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Vector2Int neighbor in GetNeighbors(currentNode.Position, _tilemap))
            {
                if (closedList.Any(node => node.Position == neighbor))
                {
                    continue;
                }

                int gScore = currentNode.GScore + 1;

                Node neighborNode = openList.FirstOrDefault(node => node.Position == neighbor);
                if (neighborNode == null)
                {
                    neighborNode = new Node(neighbor, gScore, ManhattanDistance(neighbor, _goal), currentNode);
                    openList.Add(neighborNode);
                }
                else if (gScore < neighborNode.GScore)
                {
                    neighborNode.GScore = gScore;
                    neighborNode.Parent = currentNode;
                }
            }
        }
        return new List<Vector2Int>();
    }


    private List<Vector2Int> GetNeighbors(Vector2Int _position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        neighbors.Add(new Vector2Int(_position.x + 1, _position.y));
        neighbors.Add(new Vector2Int(_position.x - 1, _position.y));
        neighbors.Add(new Vector2Int(_position.x, _position.y + 1));
        neighbors.Add(new Vector2Int(_position.x, _position.y - 1));

        return neighbors;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int _position, Tilemap _tilemap)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();



        neighbors.Add(new Vector2Int(_position.x + 1, _position.y));
        neighbors.Add(new Vector2Int(_position.x - 1, _position.y));
        neighbors.Add(new Vector2Int(_position.x, _position.y + 1));
        neighbors.Add(new Vector2Int(_position.x, _position.y - 1));

        return neighbors.Where(pos => _tilemap.HasTile(new Vector3Int(pos.x, pos.y, 0))).ToList();
    }

    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        return xDistance + yDistance;
    }


    private class Node
    {
        public Vector2Int Position { get; set; }
        public int GScore { get; set; }
        public int HScore { get; set; }
        public Node Parent { get; set; }

        public int FScore => GScore + HScore;

        public Node(Vector2Int position, int gScore, int hScore, Node parent)
        {
            Position = position;
            GScore = gScore;
            HScore = hScore;
            Parent = parent;
        }
    }
}