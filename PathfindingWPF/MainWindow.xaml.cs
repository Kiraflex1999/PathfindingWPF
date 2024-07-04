using PathfindingWPF.Classes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PathfindingWPF
{
    public partial class MainWindow : Window
    {
        private List<Chunk> _chunks;
        private SQL _sql;
        private Node? _firstSelectedNode;
        private Node? _secondSelectedNode;

        public MainWindow()
        {
            InitializeComponent();

            _sql = new SQL();

            _chunks = new List<Chunk>();

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

            if (_chunks.Count > 0)
            {
                DrawNodes();
            }
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

                    var nodePath = new Path
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