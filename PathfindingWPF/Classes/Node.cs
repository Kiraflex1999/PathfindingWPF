using System.Windows;

namespace PathfindingWPF.Classes
{
    internal class Node
    {
        private List<Node> _neighborNodes;

        public int Id { get; init; }
        public Point Point { get; set; }
        public double Radius { get; set; } = 10;
        public double CostFromStart { get; set; }
        public double HeuristicCost { get; set; }
        public double FinalCost { get; set; }
        public Node? ParentNode { get; set; }

        #region Constructor
        public Node(Point point)
        {
            Point = point;
            _neighborNodes = new List<Node>();
        }

        public Node(int id, Point point)
        {
            Id = id;
            Point = point;
            _neighborNodes = new List<Node>();
        }

        public Node(Point point, List<Node> neighborNodes)
        {
            Point = point;
            _neighborNodes = neighborNodes;
        }

        public Node(int id, Point point, List<Node> neighborNodes)
        {
            Id = id;
            Point = point;
            _neighborNodes = neighborNodes;
        }
        #endregion

        #region Node
        public void AddNeighborNode(Node node)
        {
            if (!_neighborNodes.Any(n => n.Point == node.Point))
            {
                _neighborNodes.Add(node);
            }
        }

        public void AddNeighborNode(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                AddNeighborNode(node);
            }
        }

        public List<Node> GetNeighborNodes()
        {
            return _neighborNodes;
        }

        public void RemoveNeighborNode(Node node)
        {
            _neighborNodes.Remove(node);
        }
        #endregion

        #region PathFinding
        public void CalculateCosts(Node currentNode, Node endNode)
        {
            CostFromStart = CalculateDistance(currentNode) + currentNode.CostFromStart;
            HeuristicCost = CalculateDistance(endNode);
            FinalCost = CostFromStart + HeuristicCost;
        }

        private double CalculateDistance(Node otherNode)
        {
            double x = Math.Abs(otherNode.Point.X - Point.X);
            double y = Math.Abs(otherNode.Point.Y - Point.Y);
            return Math.Sqrt(x * x + y * y);
        }

        internal void CalculateCostsReset()
        {
            CostFromStart = 0;
            HeuristicCost = 0;
            FinalCost = 0;
        }
        #endregion
    }
}
