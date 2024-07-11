using PathfindingWPF.Classes.MapObjects;
using System.Windows;

namespace PathfindingWPF.Classes.Logic
{
    internal static class CollisionDetection
    {
        public static object? Use(Point mousePosition, MapData mapData)
        {
            foreach (var chunk in mapData.GetChunks())
            {
                if (mousePosition.X > chunk.Point.X &&
                    mousePosition.X < chunk.Point.X + chunk.SizeX &&
                    mousePosition.Y > chunk.Point.Y &&
                    mousePosition.Y < chunk.Point.Y + chunk.SizeX)
                {
                    foreach (var node in chunk.GetNodes())
                    {
                        double distance = CalculateDelta(mousePosition, node);

                        if (distance < node.Radius)
                        {
                            return node;
                        }
                    }
                }
            }
            return null;
        }

        private static double CalculateDelta(Point mousePosition, Node node)
        {
            double x = Math.Abs(mousePosition.X - node.Point.X);
            double y = Math.Abs(mousePosition.Y - node.Point.Y);
            return Math.Sqrt(x * x + y * y);
        }
    }
}
