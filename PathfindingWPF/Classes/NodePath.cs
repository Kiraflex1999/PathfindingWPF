using System.Windows.Media;

namespace PathfindingWPF.Classes
{
    internal class NodePath
    {
        public Node StartNode { get; set; } // The starting node of the path
        public Node EndNode { get; set; } // The ending node of the path
        public PathGeometry PathGeometry { get; set; } // The graphical representation of the path

        // Constructor to initialize a NodePath with start node, end node, and path geometry
        public NodePath(Node startNode, Node endNode, PathGeometry pathGeometry)
        {
            StartNode = startNode;
            EndNode = endNode;
            PathGeometry = pathGeometry;
        }
    }
}
