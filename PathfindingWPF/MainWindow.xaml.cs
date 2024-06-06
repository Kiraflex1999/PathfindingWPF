using PathfindingWPF.Classes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PathfindingWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Node> _nodes;

        public MainWindow()
        {
            InitializeComponent();

            _nodes = TempNodesCreation();

            DrawPathOnCanvas();

            PathFinder pathFinder = new PathFinder();
            List<Node> path = pathFinder.Start(_nodes[0], _nodes[3]);   //node id -1 == index

            foreach (Node node in path)
            {
                Debug.WriteLine(node.Point);
            }
        }

        private List<Node> TempNodesCreation()
        {
            Node node1 = new Node(new Point(200, 200));
            Node node2 = new Node(new Point(600, 200));
            Node node3 = new Node(new Point(800, 400));
            Node node4 = new Node(new Point(1000, 800));
            Node node5 = new Node(new Point(400, 400));
            Node node6 = new Node(new Point(700, 500));
            Node node7 = new Node(new Point(200, 600));
            Node node8 = new Node(new Point(600, 600));

            node1.AddNeighborNode(new List<Node> { node2, node5 });
            node2.AddNeighborNode(new List<Node> { node1, node3, node8 });
            node3.AddNeighborNode(new List<Node> { node2, node6, node4 });
            node4.AddNeighborNode(new List<Node> { node3, node6, node8 });
            node5.AddNeighborNode(new List<Node> { node1, node7 });
            node6.AddNeighborNode(new List<Node> { node3, node4, node8 });
            node7.AddNeighborNode(new List<Node> { node5, node8 });
            node8.AddNeighborNode(new List<Node> { node2, node4, node6, node7 });

            List<Node> nodes = new List<Node>
            {
                node1, node2, node3, node4, node5, node6, node7, node8
            };

            return nodes;
        }

        private void DrawPathOnCanvas()
        {
            Path path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            GeometryGroup geometryGroup = new GeometryGroup();

            foreach (Node node in _nodes)
            {
                EllipseGeometry ellipseGeometry = new EllipseGeometry(node.Point, node.Radius, node.Radius);
                geometryGroup.Children.Add(ellipseGeometry);
            }

            HashSet<NodePath> lines = new HashSet<NodePath>();

            foreach (Node node in _nodes)
            {
                foreach (Node neighbor in node.GetNeighborNodes())
                {
                    if (lines.Where(x => (x.StartNode == node && x.EndNode == neighbor) || (x.EndNode == node && x.StartNode == neighbor)).Count() < 1)
                    {
                        PathGeometry pathGeometry = new PathGeometry();
                        PathFigure pathFigure = new PathFigure { StartPoint = node.Point };
                        pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                        pathGeometry.Figures.Add(pathFigure);
                        geometryGroup.Children.Add(pathGeometry);
                        lines.Add(new NodePath(node, neighbor, pathGeometry));
                    }
                }
            }

            path.Data = geometryGroup;

            myCanvas.Children.Add(path);
        }

        private void myCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            Point mousePosition = e.GetPosition(canvas);

            Canvas testCanvas = new Canvas();

            foreach (Node node in _nodes)
            {

            }

            Path path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            EllipseGeometry ellipseGeometry = new EllipseGeometry(mousePosition, 10, 10);
            path.Data = ellipseGeometry;
            canvas.Children.Add(path);

            _nodes.Add(new Node(mousePosition));
        }
    }
}