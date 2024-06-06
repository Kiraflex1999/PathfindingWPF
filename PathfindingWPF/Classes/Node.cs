using System.Windows;

namespace PathfindingWPF.Classes
{
    internal class Node
    {
        public Point Point { get; set; }
        private List<Node> _neighborNodes { get; set; }
        public double Radius { get; set; } = 10;

        public double GCost { get; set; }   //distance from starting node
        public double HCost { get; set; }   //distance from end node
        public double FCost { get; set; }   //GCost + HCost

        public Node? NextHop { get; set; }

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
    }
}
