using System.Windows.Media;

namespace PathfindingWPF.Classes.MapObjects
{
    internal class Path
    {
        public int NodeId1 { get; init; }
        public Node? Node1 { get; set; }
        public int NodeId2 { get; init; }
        public Node? Node2 { get; set; }
        public PathGeometry? PathGeometry { get; set; }

        public Path(int nodeId1, int nodeId2)
        {
            NodeId1 = nodeId1;
            NodeId2 = nodeId2;
        }
    }
}
