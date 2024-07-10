using System.Windows.Controls;

namespace PathfindingWPF.Classes.Canvases
{
    internal class MyCanvas
    {
        private readonly Canvas _myCanvas;

        public MyCanvas(ref Canvas myCanvas)
        {
            _myCanvas = myCanvas;
        }
    }
}
