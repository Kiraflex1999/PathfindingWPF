using PathfindingWPF.Classes.MapObjects;
using System.Windows;

namespace PathfindingWPF.Classes.Logic
{
    internal static class CollisionDetection
    {
        public enum ENewNode
        {
            True,
            False,
        }

        public static object? Use(Point mousePosition)
        {
            foreach (var node in MapData.Instance.GetNodes())
            {
                double distance = CalculateDelta(mousePosition, node);

                if (distance <= node.Radius)
                {
                    return node;
                }
                if (distance > node.Radius && distance <= node.Radius * 2 + 5)
                {
                    return ENewNode.False;
                }
            }

            return ENewNode.True;
        }

        private static double CalculateDelta(Point mousePosition, Node node)
        {
            double x = Math.Abs(mousePosition.X - node.Point.X);
            double y = Math.Abs(mousePosition.Y - node.Point.Y);
            return Math.Sqrt(x * x + y * y);
        }
    }
}
