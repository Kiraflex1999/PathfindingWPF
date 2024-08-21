using System.Windows;

namespace PathfindingWPF.Classes.MapObjects
{
    public class Chunk
    {
        private List<Node> _nodes;

        public Point Point { get; init; }
        public int SizeX { get; init; }

        public Chunk(Point point)
        {
            Point = point;
            _nodes = new();
            SizeX = MapData.Instance.GetChunkSizeX();
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

        public void RemoveNode(Node node)
        {
            if (node != null)
            {
                _nodes.Remove(node);
            }
        }

        public bool IsPositionInChunk(Point point)
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
