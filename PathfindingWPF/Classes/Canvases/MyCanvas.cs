using PathfindingWPF.Classes.MapObjects;
using System.Windows.Controls;
using System.Windows.Input;

namespace PathfindingWPF.Classes.Canvases
{
    internal class MyCanvas
    {
        private Canvas _myCanvas;
        private MapData _mapData;

        public MyCanvas(ref Canvas myCanvas)
        {
            _myCanvas = myCanvas;
            _mapData = new MapData(ref myCanvas);
        }

        #region MyCanvas
        public ref Canvas GetMyCanvas()
        {
            return ref _myCanvas;
        }

        public void DrawMap()
        {
            _myCanvas.Children.Clear();

            if (_mapData.GetChunks().Count > 0)
            {
                DrawNodes();
                DrawPaths();
            }
        }

        private void DrawNodes()
        {
            throw new NotImplementedException();
        }

        private void DrawPaths()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Interactions
        public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion
    }
}
