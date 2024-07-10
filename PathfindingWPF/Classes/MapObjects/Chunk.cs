using System.Windows;

namespace PathfindingWPF.Classes.MapObjects
{
    internal class Chunk
    {
        private List<Node> _nodes;

        public Point Point { get; init; }
        public int SizeX { get; init; }

        public Chunk(Point point, int sizeX)
        {
            Point = point;
            SizeX = sizeX;
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

        public void AddNode(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                AddNode(node);
            }
        }

        public bool PosistionInChunk(Point point)
        {
            if (point.X > Point.X &&
                point.X < Point.X + SizeX &&
                point.Y > Point.Y &&
                point.Y < Point.Y + SizeX)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
