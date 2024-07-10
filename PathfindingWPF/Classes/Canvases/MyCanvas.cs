using System.Windows.Controls;
using System.Windows.Input;

namespace PathfindingWPF.Classes.Canvases
{
    internal class MyCanvas
    {
        private readonly Canvas _myCanvas;

        public MyCanvas(ref Canvas myCanvas)
        {
            _myCanvas = myCanvas;
        }

        #region MyCanvas
        public void DrawMap()
        {

        }
        #endregion

        #region Interactions
        public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion
    }
}
