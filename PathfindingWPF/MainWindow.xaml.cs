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
        private readonly List<Node> _nodes;                     // List of nodes representing points on the map
        private List<Color> _whitePixelList = new();            // List of white pixels used in image processing
        private readonly HashSet<NodePath> _lines = new();      // Set of lines (paths) between nodes
        private Point _mouseLeftButtonUpPosition;               // Position of the mouse when left button is released
        private bool _mouseLeftButtonUpPressed;                 // Flag indicating if left mouse button is pressed
        private readonly double _halfTestCanvasSize = 25;       // Half size of the test canvas for node creation
        private List<Node> _path = new();                       // List of nodes representing the shortst found path

        // Variables to store selected nodes for pathfinding
        private Node? _firstSelectedNode;                       // First node selected for pathfinding
        private Node? _secondSelectedNode;                      // Second node selected for pathfinding
        private bool _switchSelect = true;

        // Constructor for the MainWindow class
        public MainWindow()
        {
            InitializeComponent();

            // Initialize nodes and draw the map
            _nodes = TempNodesCreation();
            DrawMapOnCanvas();

            // Example usage of pathfinding algorithm between nodes
            _firstSelectedNode = _nodes[0];
            _secondSelectedNode = _nodes[3];
            UsePathFinder(_nodes[0], _nodes[3]);
            ResetNodes();
        }

        // Method for resetting nodes after using the PathFinder
        private void ResetNodes()
        {
            foreach (Node node in _nodes)
            {
                node.ParentNode = null;
                node.CalculateCostsReset();
            }
        }

        // Method to find and display the path between two nodes using a pathfinding algorithm
        private void UsePathFinder(Node start, Node end)
        {
            PathFinder pathFinder = new PathFinder();
            _path = pathFinder.Start(start, end);

            // Output the path nodes to debug console
#if DEBUG
            foreach (Node node in _path)
            {
                Debug.WriteLine(node.Point);
            }
#endif
            DrawMapOnCanvas();

            // Reset everything
            _path.Clear();
            ResetNodes();
            _firstSelectedNode = null;
            _secondSelectedNode = null;
        }

        // Temporary method to create and set up nodes and their connections
        private List<Node> TempNodesCreation()
        {
            // Create nodes with specific positions
            var node1 = new Node(new Point(200, 200));
            var node2 = new Node(new Point(600, 200));
            var node3 = new Node(new Point(800, 400));
            var node4 = new Node(new Point(1000, 800));
            var node5 = new Node(new Point(400, 400));
            var node6 = new Node(new Point(700, 500));
            var node7 = new Node(new Point(200, 600));
            var node8 = new Node(new Point(600, 600));

            // Define neighbor relationships between nodes
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
            // Clear MyCanvas
            MyCanvas.Children.Clear();
            _lines.Clear();

            // Draw map on MyCanvas
            DrawNodesOnCanvas();
            DrawLinesOnCanvas();
        }

        // Method to draw lines between nodes on the canvas
        private void DrawLinesOnCanvas()
        {
            var geometryGroup = new GeometryGroup();
            var geometryGroupShortestPath = new GeometryGroup();

            // Iterate through each node to draw lines to its neighbors
            foreach (var node in _nodes)
            {
                foreach (var neighbor in node.GetNeighborNodes())
                {
                    // Check if the line between these nodes already exists
                    if (!_lines.Any(x => (x.StartNode == node && x.EndNode == neighbor) || (x.EndNode == node && x.StartNode == neighbor)))
                    {
                        // Check if both nodes are in _path and one is the parent of the other
                        if (_path.Contains(node) && _path.Contains(neighbor) && (node.ParentNode == neighbor || neighbor.ParentNode == node))
                        {
                            var pathGeometry = new PathGeometry();
                            var pathFigure = new PathFigure { StartPoint = node.Point };
                            pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                            pathGeometry.Figures.Add(pathFigure);
                            geometryGroupShortestPath.Children.Add(pathGeometry);
                            _lines.Add(new NodePath(node, neighbor, pathGeometry));
                        }
                        // Create a line segment between node and its neighbor
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

            // Create a path for drawing lines
            var path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };
            path.Data = geometryGroup;

            // Create a path for drawing lines
            var pathShortestPath = new Path
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
            };
            pathShortestPath.Data = geometryGroupShortestPath;

            MyCanvas.Children.Add(path);
            MyCanvas.Children.Add(pathShortestPath);
        }

        // Method to draw nodes on the canvas
        private void DrawNodesOnCanvas()
        {
            // Add ellipse geometries for each node to the geometry group
            foreach (var node in _nodes)
            {
                // Determine the fill color based on node selection status
                Brush nodeFill;
                if (node == _firstSelectedNode || node == _secondSelectedNode)
                {
                    nodeFill = Brushes.Green; // Selected node color
                }
                else
                {
                    nodeFill = Brushes.LightBlue; // Default node color
                }

                var ellipseGeometry = new EllipseGeometry(node.Point, node.Radius, node.Radius);

                // Create a path for each node to allow setting individual properties
                var nodePath = new Path
                {
                    Data = ellipseGeometry,
                    Fill = nodeFill,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                MyCanvas.Children.Add(nodePath); // Add each node to the canvas
            }
        }


        // Event handler for when the left mouse button is released on the canvas
        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Subscribe to layout update event for test canvas
            TestCanvas.LayoutUpdated += TestCanvas_LayoutUpdated;

            // Get position of mouse when left button is released
            _mouseLeftButtonUpPosition = e.GetPosition((Canvas)sender);

            // Clear children of test canvas and add a test circle node
            TestCanvas.Children.Clear();
            TestCanvas.Children.Add(CreateTestCircleNode(new Point(_halfTestCanvasSize - 1, _halfTestCanvasSize - 1)));

            // Add nodes close to mouse click position to test canvas
            TestCanvasAddCloseNodes();
            _mouseLeftButtonUpPressed = true;

            // Handle node selection
            Node? clickedNode = GetClickedNode(_mouseLeftButtonUpPosition);
            if (clickedNode != null)
            {
                if (_switchSelect)
                {
                    _firstSelectedNode = clickedNode;
                    DrawMapOnCanvas();
                    _switchSelect = false;
                }
                else
                {
                    _secondSelectedNode = clickedNode;
                    DrawMapOnCanvas();
                    _switchSelect = true;
                }
            }
        }

        // Method to determine if a node was clicked based on click position
        private Node? GetClickedNode(Point clickPosition)
        {
            foreach (var node in _nodes)
            {
                // Calculate distance from click position to node center
                double distance = Math.Sqrt(Math.Pow(clickPosition.X - node.Point.X, 2) + Math.Pow(clickPosition.Y - node.Point.Y, 2));

                // Consider it a click if within node radius
                if (distance <= node.Radius)
                {
                    return node;
                }
            }

            return null; // No node clicked
        }

        // Method to add nodes close to the mouse click position to the test canvas
        private void TestCanvasAddCloseNodes()
        {
            foreach (var node in _nodes)
            {
                // Calculate the absolute distance between the node and the mouse click position
                var x = Math.Abs(node.Point.X - _mouseLeftButtonUpPosition.X);
                var y = Math.Abs(node.Point.Y - _mouseLeftButtonUpPosition.Y);

                // Check if the node is within the halfTestCanvasSize range from the click position
                if (x <= _halfTestCanvasSize && y <= _halfTestCanvasSize)
                {
                    x = node.Point.X - _mouseLeftButtonUpPosition.X < 0 ? _halfTestCanvasSize - x : _halfTestCanvasSize + x;
                    y = node.Point.Y - _mouseLeftButtonUpPosition.Y < 0 ? _halfTestCanvasSize - y : _halfTestCanvasSize + y;

                    // Create a test circle node at the adjusted position and add it to the test canvas
                    TestCanvas.Children.Add(CreateTestCircleNode(new Point(x, y)));
                }
            }
        }


        // Event handler for when the layout of the test canvas is updated
        private void TestCanvas_LayoutUpdated(object? sender, EventArgs e)
        {
            // Get list of pixels from test canvas and filter for white pixels
            var pixelList = GetPixelListFromTestCanvas();
            _whitePixelList = GetWhitePixelListFromPixelList(pixelList);

            // Output amount of white pixels to debug console
#if DEBUG
            Debug.WriteLine("Amount of white pixels: " + _whitePixelList.Count);
#endif

            // If no white pixels and mouse button was pressed, create a new node on main canvas
            if (_whitePixelList.Count == 0 && _mouseLeftButtonUpPressed)
            {
                TestCanvas.Children.Clear();
                MyCanvas.Children.Add(CreateCircleNode(_mouseLeftButtonUpPosition));
                _nodes.Add(new Node(_mouseLeftButtonUpPosition));
                _mouseLeftButtonUpPressed = false;
            }
        }

        // Method to filter and return only white pixels from list of colors
        private static List<Color> GetWhitePixelListFromPixelList(List<Color> pixelList)
        {
            return pixelList.Where(color => color.R == 0xFF && color.G == 0xFF && color.B == 0xFF).ToList();
        }

        // Method to get list of pixels from test canvas
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

            // Convert byte array to list of colors
            for (int i = 0; i < pixels.Length; i += 4)
            {
                colorList.Add(Color.FromArgb(pixels[i + 3], pixels[i + 2], pixels[i + 1], pixels[i]));
            }

            return colorList;
        }

        // Method to create a circle node at specified position
        private static Path CreateCircleNode(Point mousePosition)
        {
            var path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            // Create ellipse geometry for the circle node
            var ellipseGeometry = new EllipseGeometry(mousePosition, 10, 10);
            path.Data = ellipseGeometry;
            return path;
        }

        // Method to create a test circle node at specified position
        private static Path CreateTestCircleNode(Point mousePosition)
        {
            var path = new Path
            {
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(245, 255, 255, 255)), // Light semi-transparent white
            };

            // Create ellipse geometry for the test circle node
            var ellipseGeometry = new EllipseGeometry(mousePosition, 12, 12);
            path.Data = ellipseGeometry;
            return path;
        }

        // Method to start pathfinding between selected nodes
        private void ButtonPathFinding_Click(object sender, RoutedEventArgs e)
        {
            if (_firstSelectedNode != null && _secondSelectedNode != null)
            {
                UsePathFinder(_firstSelectedNode, _secondSelectedNode);
            }
        }

        // Method to create path between selected nodes
        private void ButtonCreatePath_Click(object sender, RoutedEventArgs e)
        {
            if (_firstSelectedNode != null && _secondSelectedNode != null)
            {
                if (_firstSelectedNode == _secondSelectedNode) { return; }

                if (_firstSelectedNode.GetNeighborNodes().Contains(_secondSelectedNode)) { return; }

                _firstSelectedNode.AddNeighborNode(_secondSelectedNode);
                _secondSelectedNode.AddNeighborNode(_firstSelectedNode);

                DrawMapOnCanvas();
            }
        }

        // Method to delete selected nodes
        private void ButtonDeleteNode_Click(object sender, RoutedEventArgs e)
        {
            if (_firstSelectedNode == null && _secondSelectedNode == null) { return; }

            if (_firstSelectedNode == _secondSelectedNode) { _secondSelectedNode = null; }

            DeleteNode(_firstSelectedNode);
            DeleteNode(_secondSelectedNode);

            DrawMapOnCanvas();
        }

        // Method to delete a node
        private void DeleteNode(Node? node)
        {
            if (node == null) { return; }

            foreach (Node neighbor in node.GetNeighborNodes())
            {
                neighbor.RemoveNeighborNode(node);
            }

            _nodes.Remove(node);
        }

        // Method to delete the path between the two selected nodes
        private void ButtonDeletePath_Click(object sender, RoutedEventArgs e)
        {
            if (_firstSelectedNode == null || _secondSelectedNode == null) { return; }

            if (_firstSelectedNode == _secondSelectedNode) { return; }

            _firstSelectedNode.RemoveNeighborNode(_secondSelectedNode);
            _secondSelectedNode.RemoveNeighborNode(_firstSelectedNode);

            DrawMapOnCanvas();
        }
    }
}
