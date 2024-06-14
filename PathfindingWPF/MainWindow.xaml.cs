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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Node> _nodes;
        private List<Color> _whitePixelList;
        private HashSet<NodePath> _lines = [];
        private Point _mouseLeftButtonUpPosition;
        private bool _mouseLeftButtonUpPressed;
        private readonly double _halfTestCanvasSize = 25;

        public MainWindow()
        {
            InitializeComponent();

            _nodes = [];
            _whitePixelList = [];

            //_nodes = TempNodesCreation();

            DrawMapOnCanvas();

            //UsePathFinder();
        }

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

        private List<Node> TempNodesCreation()
        {
            Node node1 = new(new Point(200, 200));
            Node node2 = new(new Point(600, 200));
            Node node3 = new(new Point(800, 400));
            Node node4 = new(new Point(1000, 800));
            Node node5 = new(new Point(400, 400));
            Node node6 = new(new Point(700, 500));
            Node node7 = new(new Point(200, 600));
            Node node8 = new(new Point(600, 600));

            node1.AddNeighborNode([node2, node5]);
            node2.AddNeighborNode([node1, node3, node8]);
            node3.AddNeighborNode([node2, node6, node4]);
            node4.AddNeighborNode([node3, node6, node8]);
            node5.AddNeighborNode([node1, node7]);
            node6.AddNeighborNode([node3, node4, node8]);
            node7.AddNeighborNode([node5, node8]);
            node8.AddNeighborNode([node2, node4, node6, node7]);

            List<Node> nodes =
            [
                node1, node2, node3, node4, node5, node6, node7, node8
            ];

            return nodes;
        }

        private void DrawMapOnCanvas()
        {
            DrawNodesOnCanvas();

            DrawLinesOnCanvas();
        }

        private void DrawLinesOnCanvas()
        {
            Path path = new()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            GeometryGroup geometryGroup = CreateLines();

            path.Data = geometryGroup;
            MyCanvas.Children.Add(path);
        }

        private GeometryGroup CreateLines()
        {
            GeometryGroup geometryGroup = new();

            foreach (Node node in _nodes)
            {
                foreach (Node neighbor in node.GetNeighborNodes())
                {
                    //checks if a line between two nodes is in the hashset
                    if (!_lines.Where(x => (x.StartNode == node && x.EndNode == neighbor) || (x.EndNode == node && x.StartNode == neighbor)).Any())
                    {
                        PathGeometry pathGeometry = new();
                        PathFigure pathFigure = new() { StartPoint = node.Point };
                        pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                        pathGeometry.Figures.Add(pathFigure);
                        geometryGroup.Children.Add(pathGeometry);
                        _lines.Add(new NodePath(node, neighbor, pathGeometry));
                    }
                }
            }

            return geometryGroup;
        }

        private void DrawNodesOnCanvas()
        {
            Path path = new()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            GeometryGroup geometryGroup = new();

            foreach (Node node in _nodes)
            {
                EllipseGeometry ellipseGeometry = new(node.Point, node.Radius, node.Radius);
                geometryGroup.Children.Add(ellipseGeometry);
            }

            path.Data = geometryGroup;
            MyCanvas.Children.Add(path);
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TestCanvas.LayoutUpdated += TestCanvas_LayoutUpdated;

            var canvas = sender as Canvas;
            _mouseLeftButtonUpPosition = e.GetPosition(canvas);

            TestCanvas.Children.Clear();

            TestCanvas.Children.Add(CreateTestCircleNode(new Point(_halfTestCanvasSize - 1, _halfTestCanvasSize - 1)));

            TestCanvasAddCloseNodes();

            _mouseLeftButtonUpPressed = true;
        }

        private void TestCanvasAddCloseNodes()
        {
            foreach (Node node in _nodes)
            {
                double x = Math.Abs(node.Point.X - _mouseLeftButtonUpPosition.X);
                double y = Math.Abs(node.Point.Y - _mouseLeftButtonUpPosition.Y);

                if (x <= _halfTestCanvasSize && y <= _halfTestCanvasSize)
                {
                    if (node.Point.X - _mouseLeftButtonUpPosition.X < 0)
                    {
                        x = _halfTestCanvasSize - x;
                    }
                    else
                    {
                        x = _halfTestCanvasSize + x;
                    }

                    if (node.Point.Y - _mouseLeftButtonUpPosition.Y < 0)
                    {
                        y = _halfTestCanvasSize - y;
                    }
                    else
                    {
                        y = _halfTestCanvasSize + y;
                    }

                    TestCanvas.Children.Add(CreateTestCircleNode(new Point(x, y)));
                }
            }
        }

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

        private static List<Color> GetWhitePixelListFromPixelList(List<Color> pixelList)
        {
            return pixelList.Where(x => x.R == 0xFF && x.G == 0xFF && x.B == 0xFF).ToList();
        }

        private List<Color> GetPixelListFromTestCanvas()
        {
            RenderTargetBitmap renderTargetBitmap = new((int)_halfTestCanvasSize * 2, (int)_halfTestCanvasSize * 2, 96d, 96d, PixelFormats.Pbgra32);
            TestCanvas.Measure(new Size(_halfTestCanvasSize * 2, _halfTestCanvasSize * 2));

            renderTargetBitmap.Render(TestCanvas);

            int stride = (int)TestCanvas.ActualWidth * 4;
            int arraySize = (int)TestCanvas.ActualHeight * stride;
            byte[] pixels = new byte[arraySize];

            renderTargetBitmap.CopyPixels(pixels, stride, 0);

            List<Color> colorList = [];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                colorList.Add(Color.FromArgb(pixels[i + 3], pixels[i + 2], pixels[i + 1], pixels[i]));
            }

            return colorList;
        }

        private static Path CreateCircleNode(Point mousePosition)
        {
            Path path = new()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            EllipseGeometry ellipseGeometry = new(mousePosition, 10, 10);
            path.Data = ellipseGeometry;
            return path;
        }

        private static Path CreateTestCircleNode(Point mousePosition)
        {
            Path path = new()
            {
                //Stroke = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(245, 255, 255, 255)),
            };

            EllipseGeometry ellipseGeometry = new(mousePosition, 12, 12);
            path.Data = ellipseGeometry;
            return path;
        }
    }
}