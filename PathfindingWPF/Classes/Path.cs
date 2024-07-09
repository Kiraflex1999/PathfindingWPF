namespace PathfindingWPF.Classes
{
    internal class Path
    {
        public int NodeId1 { get; init; }
        public int NodeId2 { get; init; }

        public Path(int nodeId1, int nodeId2)
        {
            NodeId1 = nodeId1;
            NodeId2 = nodeId2;
        }
    }
}
