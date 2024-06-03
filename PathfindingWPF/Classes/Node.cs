using System.Drawing;

namespace PathfindingWPF.Classes
{
    internal class Node
    {
        public Point Point { get; set; }
        public List<Node> NeighborNodes { get; set; }
        public double Radius { get; set; } = 10;

        public double GCost { get; set; }   //distance from starting node
        public double HCost { get; set; }   //distance from end node
        public double FCost { get; set; }   //GCost + HCost

        public Node? NextHop { get; set; }

        public Node(Point point) 
        {
            Point = point;
            NeighborNodes = new List<Node>();
        }

        public Node(Point point, List<Node> neighbors) 
        {
            Point = point;
            NeighborNodes = neighbors;
        }
    }
}
