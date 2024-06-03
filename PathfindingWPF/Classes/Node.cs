using System.Drawing;

namespace PathfindingWPF.Classes
{
    internal class Node
    {
        public Point Point { get; set; }
        public List<Node> Neighbors { get; set; }
        public double Radius { get; set; } = 10;

        public double GCost { get; set; }   //distance from starting node
        public double HCost { get; set; }   //distance from end node
        public double FCost { get; set; }   //GCost + HCost

        public Node(Point point) 
        {
            Point = point;
            Neighbors = new List<Node>();
        }

        public Node(Point point, List<Node> neighbors) 
        {
            Point = point;
            Neighbors = neighbors;
        }
    }
}
