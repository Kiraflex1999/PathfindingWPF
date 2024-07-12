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
        private Node? _firstSelectedNode;
        private Node? _secondSelectedNode;
        private List<Node> _shortestPath;
        private bool _switchSelect;
        private bool _isHandlingMouseEvent;

        public MyCanvas()
        {
            _shortestPath = new List<Node>();

            MouseLeftButtonUp += OnMouseLeftButtonUp;
            SizeChanged += OnSizeChanged;
            Initialized += OnInitialized;
        }

        #region Get
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
            MapData.Instance.ResetNodes();
            _firstSelectedNode = null;
            _secondSelectedNode = null;
        }
        #endregion

        #region MyCanvas
        public void DrawMap()
        {
            Children.Clear();
            MapData.Instance.RemoveAllLines();

            if (MapData.Instance.GetChunks().Count > 0)
            {
                DrawNodes();
                DrawPaths();
            }
        }

        private void DrawNodes()
        {
            foreach (var node in MapData.Instance.GetNodes())
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
            foreach (var chunk in MapData.Instance.GetChunks())
            {
                if (mousePosition.X > chunk.Point.X &&
                    mousePosition.X < chunk.Point.X + chunk.SizeX &&
                    mousePosition.Y > chunk.Point.Y &&
                    mousePosition.Y < chunk.Point.Y + chunk.SizeX)
                {
                    var newNode = new Node(mousePosition);

                    chunk.AddNode(newNode);
                    MapData.Instance.AddNode(newNode);
                }
            }
            DrawMap();
        }

        private void DrawPaths()
        {
            var geometryGroup = new GeometryGroup();
            var geometryGroupShortestPath = new GeometryGroup();

            foreach (var node in MapData.Instance.GetNodes())
            {
                foreach (var neighbor in node.GetNeighborNodes())
                {
                    if (!MapData.Instance.GetLines().Any(x => (x.NodeId1 == node.Id && x.NodeId2 == neighbor.Id) || (x.NodeId2 == node.Id && x.NodeId1 == neighbor.Id)))
                    {
                        if (_shortestPath.Contains(node) && _shortestPath.Contains(neighbor) && (node.ParentNode == neighbor || neighbor.ParentNode == node))
                        {
                            var pathGeometry = new PathGeometry();
                            var pathFigure = new PathFigure { StartPoint = node.Point };
                            pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                            pathGeometry.Figures.Add(pathFigure);
                            geometryGroupShortestPath.Children.Add(pathGeometry);
                            MapData.Instance.GetLines().Add(new Path(node.Id, neighbor.Id, pathGeometry));
                        }
                        else
                        {
                            var pathGeometry = new PathGeometry();
                            var pathFigure = new PathFigure { StartPoint = node.Point };
                            pathFigure.Segments.Add(new LineSegment(neighbor.Point, true));
                            pathGeometry.Figures.Add(pathFigure);
                            geometryGroup.Children.Add(pathGeometry);
                            MapData.Instance.GetLines().Add(new Path(node.Id, neighbor.Id, pathGeometry));
                        }
                    }
                }
            }

            var line = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Data = geometryGroup
            };

            var lineShortestPath = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
                Data = geometryGroupShortestPath
            };

            Children.Add(line);
            Children.Add(lineShortestPath);
        }
        #endregion

        #region Event Handlers
        private void OnInitialized(object? sender, EventArgs e)
        {
            MapData.Instance.Initialize(this);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isHandlingMouseEvent) return;

            _isHandlingMouseEvent = true;

            try
            {
                var mousePosition = e.GetPosition((Canvas)sender);

                var x = CollisionDetection.Use(mousePosition);

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
            finally
            {
                _isHandlingMouseEvent = false;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth > MapData.Instance.GetMapSizeX() - MapData.Instance.GetChunkSizeX() ||
                ActualHeight > MapData.Instance.GetMapSizeY() - MapData.Instance.GetChunkSizeX())
            {
                MapData.Instance.GetMapDataFromDatabase();
                DrawMap();
            }

            if (ActualWidth + MapData.Instance.GetChunkSizeX() < MapData.Instance.GetMapSizeX() ||
                ActualHeight + MapData.Instance.GetChunkSizeX() < MapData.Instance.GetMapSizeX())
            {
                MapData.Instance.GetMapDataFromDatabase();
                DrawMap();
            }
        }
        #endregion
    }
}
