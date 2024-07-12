using PathfindingWPF.Classes.Database;
using System.Windows.Controls;

namespace PathfindingWPF.Classes.MapObjects
{
    public class MapData
    {
        private List<Chunk> _chunks;
        private List<Node> _nodes;
        private List<Path> _paths;
        private HashSet<Path> _lines;
        private int _mapSizeX;
        private int _mapSizeY;
        private int _chunkSizeX;
        private SQL _sql;
        private Canvas _myCanvas;

        public MapData(Canvas myCanvas)
        {
            _myCanvas = myCanvas;
            _sql = new SQL();
            _chunks = new List<Chunk>();
            _nodes = new List<Node>();
            _paths = new List<Path>();
            _lines = new HashSet<Path>();

            GetMapDataFromDatabase();
        }

        #region Database
        public void GetMapDataFromDatabase()
        {
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

        public List<Path> GetPaths()
        {
            return _paths;
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
        public void AddChunk(Chunk chunk)
        {
            if (chunk == null) { return; }
            _chunks.Add(chunk);
        }

        public void AddChunk(List<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                AddChunk(chunk);
            }
        }

        public void AddNode(Node node)
        {
            if (node == null) { return; }
            _nodes.Add(node);
        }

        public void AddNode(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                AddNode(node);
            }
        }

        public void AddPath(Path path)
        {
            if (path == null) { return; }
            _paths.Add(path);
        }

        public void AddPath(List<Path> paths)
        {
            foreach (var path in paths)
            {
                AddPath(path);
            }
        }

        public void AddLine(Path line)
        {
            if (line == null) { return; }
            _lines.Add(line);
        }
        #endregion

        #region Remove
        public bool RemoveChunk(Chunk chunk)
        {
            return _chunks.Remove(chunk);
        }

        public void RemoveChunk(List<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                RemoveChunk(chunk);
            }
        }

        public bool RemoveNode(Node node)
        {
            return _nodes.Remove(node);
        }

        public void RemoveNode(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                RemoveNode(node);
            }
        }

        public bool RemovePath(Path path)
        {
            return _paths.Remove(path);
        }

        public void RemovePath(List<Path> paths)
        {
            foreach (var path in paths)
            {
                RemovePath(path);
            }
        }

        public bool RemoveLine(Path path)
        {
            return _lines.Remove(path);
        }

        public void RemoveLine(List<Path> paths)
        {
            foreach (var path in paths)
            {
                RemoveLine(path);
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
            }
        }
        #endregion
    }
}
