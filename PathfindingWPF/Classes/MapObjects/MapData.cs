using PathfindingWPF.Classes.Database;
using System.Windows.Controls;

namespace PathfindingWPF.Classes.MapObjects
{
    internal class MapData
    {
        private List<Chunk> _chunks;
        private List<Node> _nodes;
        private List<Path> _paths;
        private SQL _sql;
        private Canvas _myCanvas;

        public MapData(ref Canvas myCanvas)
        {
            _myCanvas = myCanvas;
            _sql = new SQL();

            _chunks = new List<Chunk>();
            _nodes = new List<Node>();
            _paths = new List<Path>();

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
        }

        private void AddNeighborsToNodes()
        {
            foreach (var node in _nodes)
            {
                foreach (var path in _paths)
                {
                    if (path.NodeId1 == node.Id)
                    {
                        node.AddNeighborNode(_nodes.Where(n => n.Id == path.NodeId2).Single());
                    }
                    else if (path.NodeId2 == node.Id)
                    {
                        node.AddNeighborNode(_nodes.Where(n => n.Id == path.NodeId1).Single());
                    }
                }
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

        public void RemoveEverything()
        {
            _chunks.Clear();
            _nodes.Clear();
            _paths.Clear();
        }
        #endregion
    }
}
