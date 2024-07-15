using System.Windows;

namespace PathfindingWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Buttons
        private void ButtonSaveToDatabase_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeletePath_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.DeletePath();
        }

        private void ButtonDeleteNode_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.DeleteNodes();
        }

        private void ButtonPathFinding_Click(object sender, RoutedEventArgs e)
        {
            if (MyCanvas.GetFirstSelectedNode() != null && MyCanvas.GetSecondSelectedNode() != null)
            {
                MyCanvas.UsePathFinding();
            }
        }

        private void ButtonCreatePath_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.CreatePath();
        }
        #endregion
    }
}