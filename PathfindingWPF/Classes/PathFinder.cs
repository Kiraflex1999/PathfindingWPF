
using System.Xml.Linq;

namespace PathfindingWPF.Classes
{
    internal class PathFinder
    {
        public List<Node>? Start(Node startNode, Node endNode)
        {
            HashSet<Node> closedSet = new HashSet<Node>();
            List<Node> openSet = new List<Node> { startNode };

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.OrderBy(node => node.FCost).First(); //if multiple node with same FCost do something reminder!!!

                if (currentNode == endNode)
                {
                    return ReconstructPath(startNode, endNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Node neighborNode in currentNode.GetNeighborNodes())
                {
                    if (!closedSet.Contains(neighborNode))
                    {
                        neighborNode.NextHop = currentNode;
                        neighborNode.GCost = CalculateGCost(currentNode, neighborNode);
                        neighborNode.HCost = CalculateHCost(neighborNode, endNode);
                        neighborNode.FCost = neighborNode.GCost + neighborNode.HCost;

                        openSet.Add(neighborNode);
                    }
                }
            }

            return null;
        }

        private double CalculateHCost(Node currentNode, Node endNode)
        {
            if (currentNode.Point == endNode.Point)
            {
                return 0;
            }

            if (currentNode.Point.X == endNode.Point.X)
            {
                return Math.Abs(currentNode.Point.Y - endNode.Point.Y);
            }

            if (currentNode.Point.Y == endNode.Point.Y)
            {
                return Math.Abs(currentNode.Point.X - endNode.Point.X);
            }
            return CalculateHypotenuse(currentNode, endNode);
        }

        private double CalculateGCost(Node currentNode, Node neighborNode)
        {
            if (currentNode.Point == neighborNode.Point)
            {
                throw new Exception("CalculateGCost");
            }

            if (currentNode.Point.X == neighborNode.Point.X)
            {
                return Math.Abs(currentNode.Point.Y - neighborNode.Point.Y + currentNode.GCost);
            }

            if (currentNode.Point.Y == neighborNode.Point.Y)
            {
                return Math.Abs(currentNode.Point.X - neighborNode.Point.X + currentNode.GCost);
            }

            return CalculateHypotenuse(currentNode, neighborNode) + currentNode.GCost;
        }

        private double CalculateHypotenuse(Node node1, Node node2)
        {
            double x = Math.Abs(node1.Point.X - node2.Point.X);
            double y = Math.Abs(node1.Point.Y - node2.Point.Y);
            
            return Math.Sqrt(x * x + y * y);
        }

        private List<Node> ReconstructPath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();

            Node start = startNode;
            Node current = endNode;

            while (start != current)
            {
                path.Add(current);
                current = current.NextHop;
            }
            path.Add(current);
            return path;
        }
    }
}
