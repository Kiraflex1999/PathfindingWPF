using System.Windows.Media;

namespace PathfindingWPF.Classes
{
    internal class NodePath
    {
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }
        public PathGeometry PathGeometry { get; set; }

        public NodePath(Node startNode, Node endNode, PathGeometry pathGeometry) 
        {
            StartNode = startNode;
            EndNode = endNode;
            PathGeometry = pathGeometry;
        }
    }
}
