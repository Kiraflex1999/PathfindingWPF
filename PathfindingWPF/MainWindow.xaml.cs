using PathfindingWPF.Classes;
using System.Windows;

namespace PathfindingWPF
{
    public partial class MainWindow : Window
    {
        private List<Chunk> _chunks;

        public MainWindow()
        {
            InitializeComponent();

            _chunks = GetChunks();

            DrawMyCanvas();
        }

        #region Chunks
        private List<Chunk> GetChunks()
        {
            var chunks = new List<Chunk>();

            double x = 0;
            double y = 0;

            while (true)
            {
                if (x > MyCanvas.ActualWidth) { break; }
                if (y > MyCanvas.ActualHeight) { break; }

                var chunk = new Chunk(new Point(x, y));


            }

            return chunks;
        }
        #endregion

        #region MyCanvas
        private void DrawMyCanvas()
        {

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