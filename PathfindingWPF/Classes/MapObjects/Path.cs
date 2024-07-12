using System.Windows.Media;

namespace PathfindingWPF.Classes.MapObjects
{
    public class Path
    {
        public int NodeId1 { get; init; }
        public int NodeId2 { get; init; }
        public PathGeometry? PathGeometry { get; set; }

        #region Constructor
        public Path(int nodeId1 = 0, int nodeId2 = 0)
        {
            NodeId1 = nodeId1;
            NodeId2 = nodeId2;
        }

        public Path(int nodeId1, int nodeId2, PathGeometry pathGeometry)
        {
            NodeId1 = nodeId1;
            NodeId2 = nodeId2;
            PathGeometry = pathGeometry;
        }
        #endregion
    }
}
