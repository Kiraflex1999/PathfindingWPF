using PathfindingWPF.Classes;
using System.Windows;
using System.Windows.Media;

namespace PathfindingWPF
{
    public partial class MainWindow : Window
    {
        private List<Chunk> _chunks;
        private List<Classes.Path> _paths;
        private HashSet<NodePath> _lines;
        private List<Node> _shortestPath;
        private SQL _sql;
        private Node? _firstSelectedNode;
        private Node? _secondSelectedNode;

        public MainWindow()
        {
            InitializeComponent();

            _sql = new SQL();
            _chunks = new List<Chunk>();
            _paths = new List<Classes.Path>();
            _shortestPath = new List<Node>();
            _lines = new HashSet<NodePath>();

            MyCanvas.SizeChanged += MyCanvas_SizeChanged;
        }

        #region Chunks
        private List<Chunk> GetChunks()
        {
            var chunks = new List<Chunk>();

            double y = 0;
            int chunkSize = 400;

            while (y < MyCanvas.ActualHeight)
            {
                double x = 0;
                while (x < MyCanvas.ActualWidth)
                {
                    var chunk = new Chunk(new Point(x, y), chunkSize);
                    x += chunkSize;
                    chunk.AddNode(_sql.GetChunkNodes(chunk));
                    chunks.Add(chunk);
                }
                y += chunkSize;
            }

            return chunks;
        }

        private void AddNeighborsToNodes()
        {
            var nodes = new List<Node>();

            foreach (var chunk in _chunks)
            {
                foreach (var node in chunk.GetNodes())
                {
                    nodes.Add(node);
                }
            }

            foreach (var node in nodes)
            {
                var neighbors = new List<Node>();
                var paths = _paths.Where(path => path.NodeId1 == node.Id || path.NodeId2 == node.Id).ToList();

                if (paths.Count() == 0) { continue; }

                foreach (var path in paths)
                {
                    if (path.NodeId1 == node.Id)
                    {
                        var neighbor = nodes.Find(n => n.Id == path.NodeId2);
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
                        var neighbor = nodes.Find(n => n.Id == path.NodeId1);
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
        #endregion

        #region MyCanvas
        private void MyCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawMyCanvas();
        }

        private void DrawMyCanvas()
        {
            MyCanvas.Children.Clear();
            _chunks.Clear();

            _chunks = GetChunks();
            _paths = _sql.GetPaths(_chunks);

            AddNeighborsToNodes();

            if (_chunks.Count > 0)
            {
                DrawNodes();
                DrawPaths();
            }
        }

        private void DrawPaths()
        {
            var geometryGroup = new GeometryGroup();
            var geometryGroupShortestPath = new GeometryGroup();

            foreach (var chunk in _chunks)
            {
                foreach (var node in chunk.GetNodes())
                {
                    foreach (var neighbor in node.GetNeighborNodes())
                    {
                        if (!_lines.Any(x => (x.StartNode == node && x.EndNode == neighbor) || (x.EndNode == node && x.StartNode == neighbor)))
                        {
                            if (_shortestPath.Contains(node) && _shortestPath.Contains(neighbor) && (node.ParentNode == neighbor || neighbor.ParentNode == node))
                            {
                                var pathGeometry = new PathGeometry();
                                var pathFigure = new PathFigure { StartPoint = node.Point };
                                pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                                pathGeometry.Figures.Add(pathFigure);
                                geometryGroupShortestPath.Children.Add(pathGeometry);
                                _lines.Add(new NodePath(node, neighbor, pathGeometry));
                            }
                            else
                            {
                                var pathGeometry = new PathGeometry();
                                var pathFigure = new PathFigure { StartPoint = node.Point };
                                pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                                pathGeometry.Figures.Add(pathFigure);
                                geometryGroup.Children.Add(pathGeometry);
                                _lines.Add(new NodePath(node, neighbor, pathGeometry));
                            }
                        }
                    }
                }
            }

            var path = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };
            path.Data = geometryGroup;

            var pathShortestPath = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
            };
            pathShortestPath.Data = geometryGroupShortestPath;

            MyCanvas.Children.Add(path);
            MyCanvas.Children.Add(pathShortestPath);
        }

        private void DrawNodes()
        {
            foreach (var chunk in _chunks)
            {
                foreach (var node in chunk.GetNodes())
                {
                    Brush nodeFill;
                    if (node == _firstSelectedNode || node == _secondSelectedNode)
                    {
                        nodeFill = Brushes.Green;
                    }
                    else
                    {
                        nodeFill = Brushes.LightBlue;
                    }

                    var ellipseGeometry = new EllipseGeometry(node.Point, node.Radius, node.Radius);

                    var nodePath = new System.Windows.Shapes.Path
                    {
                        Data = ellipseGeometry,
                        Fill = nodeFill,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };

                    MyCanvas.Children.Add(nodePath);
                }
            }
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Buttons
        private void ButtonSaveToDatabase_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeletePath_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteNode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonPathFinding_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonCreatePath_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}