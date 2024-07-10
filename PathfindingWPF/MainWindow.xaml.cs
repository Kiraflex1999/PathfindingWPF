using PathfindingWPF.Classes.Canvases;
using System.Windows;
using System.Windows.Input;

namespace PathfindingWPF
{
    public partial class MainWindow : Window
    {
        private MyCanvas? _myCanvas;
        private TestCanvas? _testCanvas;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Canvas
        private void MyCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _myCanvas = new MyCanvas(MyCanvas);
            _testCanvas = new TestCanvas(TestCanvas);

            _myCanvas.DrawMap();
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_myCanvas == null) { return; }
            _myCanvas.MouseLeftButtonUp(sender, e);
        }

        private void MyCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_myCanvas == null) { return; }
            _myCanvas.SizeChanged(sender, e);
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