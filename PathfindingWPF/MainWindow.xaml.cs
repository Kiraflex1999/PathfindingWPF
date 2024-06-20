using PathfindingWPF.Classes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PathfindingWPF
{
    public partial class MainWindow : Window
    {
        private readonly List<Node> _nodes;
        private List<Color> _whitePixelList;
        private readonly HashSet<NodePath> _lines = new();
        private Point _mouseLeftButtonUpPosition;
        private bool _mouseLeftButtonUpPressed;
        private readonly double _halfTestCanvasSize = 25;

        // Variables to store selected nodes for pathfinding
        private Node _firstSelectedNode;
        private Node _secondSelectedNode;

        // Constructor for the MainWindow class
        public MainWindow()
        {
            InitializeComponent();

            _nodes = TempNodesCreation();

            DrawMapOnCanvas();
            UsePathFinder(_nodes[0], _nodes[3]);
        }

        // Method to find and display the path between two nodes using a pathfinding algorithm
        private void UsePathFinder(Node start, Node end)
        {
            PathFinder pathFinder = new();
            List<Node> path = pathFinder.Start(start, end);

#if DEBUG
            foreach (Node node in path)
            {
                Debug.WriteLine(node.Point);
            }
#endif
        }

        // Temporary method to create and set up nodes and their connections
        private List<Node> TempNodesCreation()
        {
            var node1 = new Node(new Point(200, 200));
            var node2 = new Node(new Point(600, 200));
            var node3 = new Node(new Point(800, 400));
            var node4 = new Node(new Point(1000, 800));
            var node5 = new Node(new Point(400, 400));
            var node6 = new Node(new Point(700, 500));
            var node7 = new Node(new Point(200, 600));
            var node8 = new Node(new Point(600, 600));

            node1.AddNeighborNode(new List<Node> { node2, node5 });
            node2.AddNeighborNode(new List<Node> { node1, node3, node8 });
            node3.AddNeighborNode(new List<Node> { node2, node6, node4 });
            node4.AddNeighborNode(new List<Node> { node3, node6, node8 });
            node5.AddNeighborNode(new List<Node> { node1, node7 });
            node6.AddNeighborNode(new List<Node> { node3, node4, node8 });
            node7.AddNeighborNode(new List<Node> { node5, node8 });
            node8.AddNeighborNode(new List<Node> { node2, node4, node6, node7 });

            return new List<Node> { node1, node2, node3, node4, node5, node6, node7, node8 };
        }

        // Method to draw the map on the canvas, including nodes and lines between them
        private void DrawMapOnCanvas()
        {
            DrawNodesOnCanvas();
            DrawLinesOnCanvas();
        }

        // Method to draw lines between nodes on the canvas
        private void DrawLinesOnCanvas()
        {
            var path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            var geometryGroup = CreateLines();
            path.Data = geometryGroup;
            MyCanvas.Children.Add(path);
        }

        // Method to create lines between neighbor nodes
        private GeometryGroup CreateLines()
        {
            var geometryGroup = new GeometryGroup();

            foreach (var node in _nodes)
            {
                foreach (var neighbor in node.GetNeighborNodes())
                {
                    // Check if the line between these nodes already exists
                    if (!_lines.Any(x => (x.StartNode == node && x.EndNode == neighbor) || (x.EndNode == node && x.StartNode == neighbor)))
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

            return geometryGroup;
        }

        // Method to draw nodes on the canvas
        private void DrawNodesOnCanvas()
        {
            var path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            var geometryGroup = new GeometryGroup();

            foreach (var node in _nodes)
            {
                var ellipseGeometry = new EllipseGeometry(node.Point, node.Radius, node.Radius);
                geometryGroup.Children.Add(ellipseGeometry);
            }

            path.Data = geometryGroup;
            MyCanvas.Children.Add(path);
        }

        // Event handler for when the left mouse button is released on the canvas
        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TestCanvas.LayoutUpdated += TestCanvas_LayoutUpdated;

            _mouseLeftButtonUpPosition = e.GetPosition((Canvas)sender);

            TestCanvas.Children.Clear();
            TestCanvas.Children.Add(CreateTestCircleNode(new Point(_halfTestCanvasSize - 1, _halfTestCanvasSize - 1)));

            TestCanvasAddCloseNodes();
            _mouseLeftButtonUpPressed = true;

            // Handle node selection for pathfinding
            Node clickedNode = GetClickedNode(_mouseLeftButtonUpPosition);
            if (clickedNode != null)
            {
                if (_firstSelectedNode == null)
                {
                    _firstSelectedNode = clickedNode;
                }
                else if (_secondSelectedNode == null)
                {
                    _secondSelectedNode = clickedNode;
                    UsePathFinder(_firstSelectedNode, _secondSelectedNode);
                    // Reset after finding the path
                    _firstSelectedNode = null;
                    _secondSelectedNode = null;
                }
            }
        }

        private Node GetClickedNode(Point clickPosition)
        {
            foreach (var node in _nodes)
            {
                // Calculate the distance from the click position to the center of the node
                double distance = Math.Sqrt(Math.Pow(clickPosition.X - node.Point.X, 2) + Math.Pow(clickPosition.Y - node.Point.Y, 2));

                // If the distance is less than or equal to the node's radius, consider it a click on that node
                if (distance <= node.Radius)
                {
                    return node;
                }
            }

            return null; // Return null if no node was clicked
        }

        // Method to add nodes close to the mouse click position to the test canvas
        private void TestCanvasAddCloseNodes()
        {
            foreach (var node in _nodes)
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

        // Event handler for when the layout of the test canvas is updated
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
                _nodes.Add(new Node(_mouseLeftButtonUpPosition));
                _mouseLeftButtonUpPressed = false;
            }
        }

        // Method to filter and return only the white pixels from a list of colors
        private static List<Color> GetWhitePixelListFromPixelList(List<Color> pixelList)
        {
            return pixelList.Where(color => color.R == 0xFF && color.G == 0xFF && color.B == 0xFF).ToList();
        }

        // Method to get the list of pixels from the test canvas
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

        // Method to create a circle node at the specified position
        private static Path CreateCircleNode(Point mousePosition)
        {
            var path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            var ellipseGeometry = new EllipseGeometry(mousePosition, 10, 10);
            path.Data = ellipseGeometry;
            return path;
        }

        // Method to create a test circle node at the specified position
        private static Path CreateTestCircleNode(Point mousePosition)
        {
            var path = new Path
            {
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(245, 255, 255, 255)),
            };

            var ellipseGeometry = new EllipseGeometry(mousePosition, 12, 12);
            path.Data = ellipseGeometry;
            return path;
        }
    }
}
