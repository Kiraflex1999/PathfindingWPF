using PathfindingWPF.Classes.Database;
using System.Windows.Controls;

namespace PathfindingWPF.Classes.MapObjects
{
    public class MapData
    {
        private List<Chunk> _chunks;
        private List<Node> _nodes;
        private List<Node> _removedNodes;
        private List<Path> _paths;
        private List<Path> _removedPaths;
        private HashSet<Path> _lines;
        private int _mapSizeX;
        private int _mapSizeY;
        private int _chunkSizeX = 250;
        private SQL _sql;
        private Canvas? _myCanvas;

        private static readonly Lazy<MapData> _instance = new(() => new MapData());

        public static MapData Instance => _instance.Value;

        private MapData()
        {
            _sql = new SQL();
            _chunks = new List<Chunk>();
            _nodes = new List<Node>();
            _paths = new List<Path>();
            _lines = new HashSet<Path>();
            _removedNodes = new List<Node>();
            _removedPaths = new List<Path>();
        }

        public void Initialize(Canvas myCanvas)
        {
            _myCanvas = myCanvas;

            GetMapDataFromDatabase();
        }

        #region Database
        public void GetMapDataFromDatabase()
        {
            if (_myCanvas == null) { throw new ArgumentNullException(nameof(_myCanvas)); }

            RemoveEverything();

            _chunks = GetChunksFromDatabase();
            _nodes = GetNodesFromChunks();
            _paths = GetPathsFromDatabase();

            AddNeighborsToNodes();
            GetMapSizeFromChunks();
        }

        private void GetMapSizeFromChunks()
        {
            _mapSizeX = 0;
            _mapSizeY = 0;
            _chunkSizeX = 0;

            bool isMapSizeXFinished = false;

            Chunk? lastChunk = null;

            foreach (var chunk in _chunks)
            {
                if (lastChunk == null)
                {
                    lastChunk = chunk;
                    _mapSizeX += chunk.SizeX;
                    _mapSizeY += chunk.SizeX;
                    _chunkSizeX = chunk.SizeX;
                    continue;
                }

                if (chunk.Point.X > lastChunk.Point.X && !isMapSizeXFinished)
                {
                    _mapSizeX += chunk.SizeX;
                    lastChunk = chunk;
                    continue;
                }

                if (chunk.Point.Y > lastChunk.Point.Y)
                {
                    _mapSizeY += chunk.SizeX;
                    lastChunk = chunk;
                    isMapSizeXFinished = true;
                    continue;
                }
            }
        }

        private void AddNeighborsToNodes()
        {
            foreach (var node in _nodes)
            {
                var neighbors = new List<Node>();
                foreach (var path in _paths)
                {
                    if (path.NodeId1 == node.Id)
                    {
                        var neighbor = _nodes.Find(n => n.Id == path.NodeId2);
                        if (neighbor != null)
                        {
                            if (!neighbors.Contains(neighbor))
                            {
                                neighbors.Add(neighbor);
                            }
                        }
                    }
                    else if (path.NodeId2 == node.Id)
                    {
                        var neighbor = _nodes.Find(n => n.Id == path.NodeId1);
                        if (neighbor != null)
                        {
                            if (!neighbors.Contains(neighbor))
                            {
                                neighbors.Add(neighbor);
                            }
                        }
                    }
                }
                node.AddNeighborNode(neighbors);
            }
        }

        private List<Path> GetPathsFromDatabase()
        {
            return _sql.GetPaths(_nodes);
        }

        private List<Node> GetNodesFromChunks()
        {
            var nodes = new List<Node>();
            foreach (var chunk in _chunks)
            {
                foreach (var node in chunk.GetNodes())
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }

        private List<Chunk> GetChunksFromDatabase()
        {
            if (_myCanvas == null) { throw new ArgumentNullException(nameof(_myCanvas)); }

            return _sql.GetChunks(_myCanvas.ActualHeight, _myCanvas.ActualWidth);
        }
        #endregion

        #region Get
        public List<Chunk> GetChunks()
        {
            return _chunks;
        }

        public List<Node> GetNodes()
        {
            return _nodes;
        }

        public HashSet<Path> GetLines()
        {
            return _lines;
        }

        public int GetMapSizeX()
        {
            return _mapSizeX;
        }

        public int GetMapSizeY()
        {
            return _mapSizeY;
        }

        public int GetChunkSizeX()
        {
            return _chunkSizeX;
        }
        #endregion

        #region Add
        public void AddNode(Node node)
        {
            if (node == null) { return; }
            _nodes.Add(node);
        }

        public void AddLine(Node node1, Node node2)
        {
            node1.AddNeighborNode(node2);
            node2.AddNeighborNode(node1);
        }
        #endregion

        #region Remove
        public void RemoveNode(Node node)
        {
            _nodes.Remove(node);
            _removedNodes.Add(node);

            foreach (var chunk in _chunks)
            {
                if (chunk.GetNodes().Contains(node))
                {
                    chunk.RemoveNode(node);
                }
            }
        }

        public void RemoveAllLines()
        {
            _lines.Clear();
        }

        public void RemoveEverything()
        {
            _chunks.Clear();
            _nodes.Clear();
            _paths.Clear();
            _lines.Clear();
        }

        public void ResetNodes()
        {
            foreach (var node in _nodes)
            {
                node.ParentNode = null;
                node.CalculateCostsReset();
            }
        }
        #endregion
    }
}
