using PathfindingWPF.Classes.Logic;
using PathfindingWPF.Classes.MapObjects;
using PathfindingWPF.Classes.Pathfinding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PathfindingWPF.Classes.Canvases
{
    public class MyCanvas : Canvas
    {
        private MapData _mapData;
        private Node? _firstSelectedNode;
        private Node? _secondSelectedNode;
        private List<Node> _shortestPath;
        private bool _switchSelect;

        public MyCanvas()
        {
            _mapData = new MapData(this);
            _shortestPath = new List<Node>();

            MouseLeftButtonUp += OnMouseLeftButtonUp;
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
        }

        #region Get
        public MapData GetMapData()
        {
            return _mapData;
        }

        public Node? GetFirstSelectedNode()
        {
            return _firstSelectedNode;
        }

        public Node? GetSecondSelectedNode()
        {
            return _firstSelectedNode;
        }
        #endregion

        #region Pathfinding
        public void UsePathFinding()
        {
            if (_firstSelectedNode == null || _secondSelectedNode == null) { return; }

            var pathFinder = new PathFinder();
            _shortestPath = pathFinder.Start(_firstSelectedNode, _secondSelectedNode);

            DrawMap();

            _shortestPath.Clear();
            _mapData.ResetNodes();
            _firstSelectedNode = null;
            _secondSelectedNode = null;
        }
        #endregion

        #region MyCanvas
        public void DrawMap()
        {
            Children.Clear();
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

                Children.Add(nodePath);
            }
        }

        private void SelectNode(Node x)
        {
            if (_switchSelect)
            {
                _firstSelectedNode = (Node)x;
                DrawMap();
                _switchSelect = false;
            }
            else
            {
                _secondSelectedNode = (Node)x;
                DrawMap();
                _switchSelect = true;
            }
        }

        private void CreateNode(Point mousePosition)
        {
            foreach (var chunk in _mapData.GetChunks())
            {
                if (mousePosition.X > chunk.Point.X &&
                    mousePosition.X < chunk.Point.X + chunk.SizeX &&
                    mousePosition.Y > chunk.Point.Y &&
                    mousePosition.Y < chunk.Point.Y + chunk.SizeX)
                {
                    var newNode = new Node(mousePosition);

                    chunk.AddNode(newNode);
                    _mapData.AddNode(newNode);
                }
            }
            DrawMap();
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

            Children.Add(line);
            Children.Add(lineShortestPath);
        }
        #endregion

        #region Event Handlers
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawMap();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition((Canvas)sender);

            var x = CollisionDetection.Use(mousePosition, _mapData);

            switch (x)
            {
                case Node:
                    SelectNode((Node)x);
                    break;

                case Path:
                    throw new NotImplementedException();

                case CollisionDetection.ENewNode.True:
                    CreateNode(mousePosition);
                    break;

                case CollisionDetection.ENewNode.False:
                    break;

                default:
                    throw new Exception("CollisionDetection Error");
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth > _mapData.GetMapSizeX() - _mapData.GetChunkSizeX() ||
                ActualHeight > _mapData.GetMapSizeY() - _mapData.GetChunkSizeX())
            {
                _mapData.GetMapDataFromDatabase();
                DrawMap();
            }

            if (ActualWidth + _mapData.GetChunkSizeX() < _mapData.GetMapSizeX() ||
                ActualHeight + _mapData.GetChunkSizeX() < _mapData.GetMapSizeX())
            {
                _mapData.GetMapDataFromDatabase();
                DrawMap();
            }
        }
        #endregion
    }
}
