using PathfindingWPF.Classes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace PathfindingWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TempNodesCreation();

            DrawPathOnCanvas(TempNodesCreation());

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

            List<Node> nodes = new List<Node>
            {
                node1 , node2 , node3 , node4 , node5 , node6 , node7 , node8
            };

            return nodes;
        }

        private void DrawPathOnCanvas(List<Node> nodes)
        {
            Path path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            GeometryGroup geometryGroup = new GeometryGroup();

            foreach (Node node in nodes)
            {
                EllipseGeometry ellipseGeometry = new EllipseGeometry(node.Point, node.Radius, node.Radius);
                geometryGroup.Children.Add(ellipseGeometry);
            }

            path.Data = geometryGroup;

            myCanvas.Children.Add(path);
        }

        //private void DrawPathOnCanvas()
        //{
        //    Path myPath = new Path
        //    {
        //        Stroke = Brushes.Black,
        //        StrokeThickness = 2,
        //        Fill = Brushes.LightBlue,
        //    };

        //    GeometryGroup geometryGroup = new GeometryGroup();

        //    RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(50, 50, 150, 100));
        //    geometryGroup.Children.Add(rectangleGeometry);

        //    EllipseGeometry ellipseGeometry = new EllipseGeometry(new Point(300, 300), 10, 10);
        //    geometryGroup.Children.Add(ellipseGeometry);

        //    PathGeometry pathGeometry = new PathGeometry();
        //    PathFigure pathFigure = new PathFigure { StartPoint = new Point(300, 300) };
        //    pathFigure.Segments.Add(new LineSegment(new Point(400, 300), true));
        //    pathFigure.Segments.Add(new LineSegment(new Point(400, 400), true));
        //    pathFigure.Segments.Add(new LineSegment(new Point(300, 400), true));
        //    pathFigure.Segments.Add(new LineSegment(new Point(300, 300), true));
        //    pathGeometry.Figures.Add(pathFigure);
        //    geometryGroup.Children.Add(pathGeometry);

        //    myPath.Data = geometryGroup;

        //    myCanvas.Children.Add(myPath);
        //}
    }
}