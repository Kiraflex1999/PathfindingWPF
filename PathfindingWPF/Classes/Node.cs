using System.Windows;

namespace PathfindingWPF.Classes
{
    internal class Node
    {
        public int Id;
        public Point Point { get; set; } // The position of the node
        private List<Node> _neighborNodes; // List of neighboring nodes
        public double Radius { get; set; } = 10; // Radius of the node for visualization

        public double CostFromStart { get; set; } // Cost from the start node to this node
        public double HeuristicCost { get; set; } // Estimated cost from this node to the end node
        public double FinalCost { get; set; } // Total cost (CostFromStart + HeuristicCost)

        public Node? ParentNode { get; set; } // Parent node used for path reconstruction

        // Constructor to initialize a node with a given point
        public Node(Point point)
        {
            Point = point;
            _neighborNodes = new List<Node>();
        }

        // Constructor to initialize a node with a given point and Id
        public Node(int id, Point point)
        {
            Id = id;
            Point = point;
            _neighborNodes = new List<Node>();
        }

        // Constructor to initialize a node with a given point and list of neighbors
        public Node(Point point, List<Node> neighborNodes)
        {
            Point = point;
            _neighborNodes = neighborNodes;
        }

        // Adds a neighbor node if it does not already exist in the list
        public void AddNeighborNode(Node node)
        {
            if (!_neighborNodes.Any(n => n.Point == node.Point))
            {
                _neighborNodes.Add(node);
            }
        }

        // Adds a list of neighbor nodes, ensuring no duplicates
        public void AddNeighborNode(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                AddNeighborNode(node);
            }
        }

        // Returns the list of neighbor nodes
        public List<Node> GetNeighborNodes()
        {
            return _neighborNodes;
        }

        // Calculates the costs for the pathfinding algorithm
        public void CalculateCosts(Node currentNode, Node endNode)
        {
            CostFromStart = CalculateDistance(currentNode) + currentNode.CostFromStart;
            HeuristicCost = CalculateDistance(endNode);
            FinalCost = CostFromStart + HeuristicCost;
        }

        // Calculates the Euclidean distance between this node and another node
        private double CalculateDistance(Node otherNode)
        {
            double x = Math.Abs(otherNode.Point.X - Point.X);
            double y = Math.Abs(otherNode.Point.Y - Point.Y);
            return Math.Sqrt(x * x + y * y);
        }

        // Resets the costs of the node
        internal void CalculateCostsReset()
        {
            CostFromStart = 0;
            HeuristicCost = 0;
            FinalCost = 0;
        }

        // Removes neighbor from this node
        internal void RemoveNeighborNode(Node node)
        {
            _neighborNodes.Remove(node);
        }
    }
}
