using System.Windows;

namespace PathfindingWPF.Classes
{
    internal class Node
    {
        public Point Point { get; set; }
        private List<Node> _neighborNodes { get; set; }
        public double Radius { get; set; } = 10;

        public double CostFromStart { get; set; }
        public double HeuristicCost {  get; set; }
        public double FinalCost { get; set; }

        public Node? ParentNode { get; set; }

        public Node(Point point) 
        {
            Point = point;
            _neighborNodes = new List<Node>();
        }

        public Node(Point point, List<Node> neighborNodes) 
        {
            Point = point;
            _neighborNodes = neighborNodes;
        }

        public void AddNeighborNode(Node node)
        {
            if (_neighborNodes.Where(n => n.Point == node.Point).Count() < 1) 
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

        public void CalculateCosts(Node currentNode, Node endNode)
        {
            CostFromStart = CalculateHypotenuse(currentNode) + currentNode.CostFromStart;
            HeuristicCost = CalculateHypotenuse(endNode);
            FinalCost = CostFromStart + HeuristicCost;
        }

        private double CalculateHypotenuse(Node currentNode)
        {
            double x = Math.Abs(currentNode.Point.X - this.Point.X);
            double y = Math.Abs(currentNode.Point.Y - this.Point.Y);

            return Math.Sqrt(x * x + y * y);
        }
    }
}
