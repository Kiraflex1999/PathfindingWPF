using System.Windows;

namespace PathfindingWPF.Classes
{
    internal class Chunk
    {
        private List<Node> _nodes;

        public Point Point { get; init; }
        public int SizeX { get; } = 400;
        public int SizeY { get; } = 400;

        public Chunk(Point point)
        {
            Point = point;
            _nodes = new();
        }

        #region Chunk
        public List<Node> GetNodes()
        {
            return _nodes;
        }

        public void AddNode(Node node)
        {
            if (node != null)
            {
                _nodes.Add(node);
            }
        }
        #endregion
    }
}
