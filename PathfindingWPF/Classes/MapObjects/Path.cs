using System.Windows.Media;

namespace PathfindingWPF.Classes.MapObjects
{
    public class Path
    {
        public Node Node1 { get; init; }
        public Node Node2 { get; init; }
        public PathGeometry? PathGeometry { get; set; }

        #region Constructor
        public Path(Node node1, Node node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public Path(Node node1, Node node2, PathGeometry pathGeometry)
        {
            Node1 = node1;
            Node2 = node2;
            PathGeometry = pathGeometry;
        }
        #endregion
    }
}
