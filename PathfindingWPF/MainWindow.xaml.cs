using PathfindingWPF.Classes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        private Point _mouseLeftButtonUpPosition;
        private bool _mouseLeftButtonUpPressed;
        private bool _switchSelect;
        private List<Color> _whitePixelList;
        private readonly int _halfTestCanvasSize = 25;

        public MainWindow()
        {
            InitializeComponent();

            _sql = new SQL();
            _chunks = new List<Chunk>();
            _paths = new List<Classes.Path>();
            _shortestPath = new List<Node>();
            _lines = new HashSet<NodePath>();
            _whitePixelList = new List<Color>();

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

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TestCanvas.LayoutUpdated += TestCanvas_LayoutUpdated;

            _mouseLeftButtonUpPosition = e.GetPosition((Canvas)sender);

            UseTestCanvas();

            Node? clickedNode = GetClickedNode(_mouseLeftButtonUpPosition);
            if (clickedNode != null)
            {
                if (_switchSelect)
                {
                    _firstSelectedNode = clickedNode;
                    DrawMyCanvas();
                    _switchSelect = false;
                }
                else
                {
                    _secondSelectedNode = clickedNode;
                    DrawMyCanvas();
                    _switchSelect = true;
                }
            }
        }
        #endregion

        #region TestCanvas
        private void TestCanvas_LayoutUpdated(object? sender, EventArgs e)
        {
            var pixelList = GetPixelListFromTestCanvas();
            _whitePixelList = GetWhitePixelListFromPixelList(pixelList);

#if DEBUG
            Debug.WriteLine("Amount of white pixels: " + _whitePixelList.Count);
#endif

            if (_whitePixelList.Count == 0 && _mouseLeftButtonUpPressed)
            {
                TestCanvas.Children.Clear();
                MyCanvas.Children.Add(CreateCircleNode(_mouseLeftButtonUpPosition));
                foreach (var chunk in _chunks)
                {
                    if (chunk.PosistionInChunk(_mouseLeftButtonUpPosition))
                    {
                        chunk.AddNode(new Node(_mouseLeftButtonUpPosition));
                        break;
                    }
                }
                _mouseLeftButtonUpPressed = false;
            }
        }

        private UIElement CreateCircleNode(Point mousePosition)
        {
            var path = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            var ellipseGeometry = new EllipseGeometry(mousePosition, 10, 10);
            path.Data = ellipseGeometry;
            return path;
        }

        private List<Color> GetWhitePixelListFromPixelList(List<Color> pixelList)
        {
            return pixelList.Where(color => color.R == 0xFF && color.G == 0xFF && color.B == 0xFF).ToList();
        }

        private List<Color> GetPixelListFromTestCanvas()
        {
            var renderTargetBitmap = new RenderTargetBitmap((int)_halfTestCanvasSize * 2, (int)_halfTestCanvasSize * 2, 96d, 96d, PixelFormats.Pbgra32);
            TestCanvas.Measure(new Size(_halfTestCanvasSize * 2, _halfTestCanvasSize * 2));
            renderTargetBitmap.Render(TestCanvas);

            var stride = (int)TestCanvas.ActualWidth * 4;
            var arraySize = (int)TestCanvas.ActualHeight * stride;
            var pixels = new byte[arraySize];
            renderTargetBitmap.CopyPixels(pixels, stride, 0);

            var colorList = new List<Color>();

            for (int i = 0; i < pixels.Length; i += 4)
            {
                colorList.Add(Color.FromArgb(pixels[i + 3], pixels[i + 2], pixels[i + 1], pixels[i]));
            }

            return colorList;
        }

        private void UseTestCanvas()
        {
            TestCanvas.Children.Clear();
            TestCanvas.Children.Add(CreateTestCircleNode(new Point(_halfTestCanvasSize - 1, _halfTestCanvasSize - 1)));

            TestCanvasAddCloseNodes();
            _mouseLeftButtonUpPressed = true;
        }

        private Node? GetClickedNode(Point clickPosition)
        {
            foreach (var chunk in _chunks)
            {
                foreach (var node in chunk.GetNodes())
                {
                    double distance = Math.Sqrt(Math.Pow(clickPosition.X - node.Point.X, 2) + Math.Pow(clickPosition.Y - node.Point.Y, 2));

                    if (distance <= node.Radius)
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        private void TestCanvasAddCloseNodes()
        {
            foreach (var chunk in _chunks)
            {
                foreach (var node in chunk.GetNodes())
                {
                    var x = Math.Abs(node.Point.X - _mouseLeftButtonUpPosition.X);
                    var y = Math.Abs(node.Point.Y - _mouseLeftButtonUpPosition.Y);

                    if (x <= _halfTestCanvasSize && y <= _halfTestCanvasSize)
                    {
                        x = node.Point.X - _mouseLeftButtonUpPosition.X < 0 ? _halfTestCanvasSize - x : _halfTestCanvasSize + x;
                        y = node.Point.Y - _mouseLeftButtonUpPosition.Y < 0 ? _halfTestCanvasSize - y : _halfTestCanvasSize + y;

                        TestCanvas.Children.Add(CreateTestCircleNode(new Point(x, y)));
                    }
                }
            }
        }

        private UIElement CreateTestCircleNode(Point mousePosition)
        {
            var path = new System.Windows.Shapes.Path
            {
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(245, 255, 255, 255)),
            };

            var ellipseGeometry = new EllipseGeometry(mousePosition, 12, 12);
            path.Data = ellipseGeometry;
            return path;
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