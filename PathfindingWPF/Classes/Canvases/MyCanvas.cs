using PathfindingWPF.Classes.MapObjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PathfindingWPF.Classes.Canvases
{
    internal class MyCanvas
    {
        private Canvas _myCanvas;
        private MapData _mapData;
        private Node? _firstSelectedNode;
        private Node? _secondSelectedNode;
        private List<Node> _shortestPath;

        public MyCanvas(Canvas myCanvas)
        {
            _myCanvas = myCanvas;
            _mapData = new MapData(myCanvas);
            _shortestPath = new List<Node>();
        }

        #region MyCanvas
        public ref Canvas GetMyCanvas()
        {
            return ref _myCanvas;
        }

        public void DrawMap()
        {
            _myCanvas.Children.Clear();
            _mapData.RemoveAllLines();

            if (_mapData.GetChunks().Count > 0)
            {
                DrawNodes();
                DrawPaths();
            }
        }

        private void DrawNodes()
        {
            foreach (var node in _mapData.GetNodes())
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

                var nodePath = new System.Windows.Shapes.Path
                {
                    Data = ellipseGeometry,
                    Fill = nodeFill,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                _myCanvas.Children.Add(nodePath);
            }
        }

        private void DrawPaths()
        {
            var geometryGroup = new GeometryGroup();
            var geometryGroupShortestPath = new GeometryGroup();

            foreach (var node in _mapData.GetNodes())
            {
                foreach (var neighbor in node.GetNeighborNodes())
                {
                    if (!_mapData.GetLines().Any(x => (x.NodeId1 == node.Id && x.NodeId2 == neighbor.Id) || (x.NodeId2 == node.Id && x.NodeId1 == neighbor.Id)))
                    {
                        if (_shortestPath.Contains(node) && _shortestPath.Contains(neighbor) && (node.ParentNode == neighbor || neighbor.ParentNode == node))
                        {
                            var pathGeometry = new PathGeometry();
                            var pathFigure = new PathFigure { StartPoint = node.Point };
                            pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                            pathGeometry.Figures.Add(pathFigure);
                            geometryGroupShortestPath.Children.Add(pathGeometry);
                            _mapData.GetLines().Add(new Path(node.Id, neighbor.Id, pathGeometry));
                        }
                        else
                        {
                            var pathGeometry = new PathGeometry();
                            var pathFigure = new PathFigure { StartPoint = node.Point };
                            pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                            pathGeometry.Figures.Add(pathFigure);
                            geometryGroup.Children.Add(pathGeometry);
                            _mapData.GetLines().Add(new Path(node.Id, neighbor.Id, pathGeometry));
                        }
                    }
                }
            }

            var line = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };
            line.Data = geometryGroup;

            var lineShortestPath = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
            };
            lineShortestPath.Data = geometryGroupShortestPath;

            _myCanvas.Children.Add(line);
            _myCanvas.Children.Add(lineShortestPath);
        }
        #endregion

        #region Interactions
        public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        public void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_myCanvas.ActualWidth > _mapData.GetMapSizeX() - _mapData.GetMapSizeX() ||
                _myCanvas.ActualHeight > _mapData.GetMapSizeY() - _mapData.GetMapSizeX())
            {
                _mapData.GetMapDataFromDatabase();
                DrawMap();
            }
        }
        #endregion
    }
}
