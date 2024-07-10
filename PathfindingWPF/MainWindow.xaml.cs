using PathfindingWPF.Classes.Canvases;
using PathfindingWPF.Classes.MapObjects;
using System.Windows;
using System.Windows.Input;

namespace PathfindingWPF
{
    public partial class MainWindow : Window
    {
        private MyCanvas _myCanvas;
        private TestCanvas _testCanvas;
        private MapData _mapData;

        public MainWindow()
        {
            InitializeComponent();

            _myCanvas = new MyCanvas(ref MyCanvas);
            _testCanvas = new TestCanvas(ref TestCanvas);
            _mapData = new MapData();

        }

        #region Canvas
        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _myCanvas.MouseLeftButtonUp(sender, e);
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