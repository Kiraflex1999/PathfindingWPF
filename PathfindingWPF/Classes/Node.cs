using System.Windows;

namespace PathfindingWPF.Classes
{
    internal class Node
    {
        public Point Point { get; set; }
        private List<Node> _NeighborNodes { get; set; }
        public double Radius { get; set; } = 10;

        public double GCost { get; set; }   //distance from starting node
        public double HCost { get; set; }   //distance from end node
        public double FCost { get; set; }   //GCost + HCost

        public Node? NextHop { get; set; }

        public Node(Point point) 
        {
            Point = point;
            _NeighborNodes = new List<Node>();
        }

        public Node(Point point, List<Node> neighbors) 
        {
            Point = point;
            _NeighborNodes = neighbors;
        }

        public void AddNeighbor(Node node)
        {
            if (_NeighborNodes.Where(n => n.Point == node.Point).Count() < 1) 
            {
                _NeighborNodes.Add(node);
            }
        }

        public void AddNeighbor(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                AddNeighbor(node);
            }
        }

        public List<Node> GetNeighborNodes()
        {
            return _NeighborNodes;
        }
    }
}
