
namespace PathfindingWPF.Classes
{
    internal class PathFinder
    {
        public List<Node>? Start(Node startNode, Node endNode)
        {
            HashSet<Node> closedSet = new HashSet<Node>();
            List<Node> openSet = new List<Node> { startNode };

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.OrderBy(node => node.FCost).First(); //if multiple node with same FCost do something reminder!!!

                if (currentNode == endNode)
                {
                    return ReconstructPath(startNode, endNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Node neighborNode in currentNode.NeighborNodes)
                {
                    if (!closedSet.Contains(neighborNode))
                    {
                        neighborNode.NextHop = currentNode;
                        neighborNode.GCost = CalculateCost(currentNode, neighborNode);
                        neighborNode.HCost = CalculateCost(neighborNode, endNode);
                        neighborNode.FCost = neighborNode.GCost + neighborNode.HCost;

                        openSet.Add(neighborNode);
                    }
                }
            }

            return null;
        }

        private double CalculateCost(Node node1, Node node2)
        {
            if (node1.Point == node2.Point)
            {
                throw new Exception("node1.Point == node2.Point in PathFinder.CalculateCost()");
            }

            if (node1.Point.X == node2.Point.X)
            {
                return Math.Abs(node1.Point.Y - node2.Point.Y);
            }

            if (node1.Point.Y == node2.Point.Y)
            {
                return Math.Abs(node1.Point.X - node2.Point.X);
            }

            return CalculateHypotenuse(node1, node2);
        }

        private double CalculateHypotenuse(Node node1, Node node2)
        {
            double x = Math.Abs(node1.Point.X - node2.Point.X);
            double y = Math.Abs(node1.Point.Y - node2.Point.Y);
            
            return Math.Sqrt(x * x + y * y);
        }

        private List<Node> ReconstructPath(Node startNode, Node endNode)
        {
            throw new NotImplementedException();
        }
    }
}
