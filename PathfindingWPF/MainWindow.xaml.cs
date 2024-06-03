using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

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
            DrawPathOnCanvas();
        }

        private void DrawPathOnCanvas()
        {
            Path myPath = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            GeometryGroup geometryGroup = new GeometryGroup();

            RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(50, 50, 150, 100));
            geometryGroup.Children.Add(rectangleGeometry);

            EllipseGeometry ellipseGeometry = new EllipseGeometry(new Point(300, 300), 10, 10);
            geometryGroup.Children.Add(ellipseGeometry);

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure { StartPoint = new Point(300, 300) };
            pathFigure.Segments.Add(new LineSegment(new Point(400, 300), true));
            pathFigure.Segments.Add(new LineSegment(new Point(400, 400), true));
            pathFigure.Segments.Add(new LineSegment(new Point(300, 400), true));
            pathFigure.Segments.Add(new LineSegment(new Point(300, 300), true));
            pathGeometry.Figures.Add(pathFigure);
            geometryGroup.Children.Add(pathGeometry);

            myPath.Data = geometryGroup;

            myCanvas.Children.Add(myPath);
        }
    }
}