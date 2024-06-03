
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
                        neighborNode.GCost = CalculateGCost(currentNode, neighborNode);
                        neighborNode.HCost = CalculateHCost(neighborNode, endNode);
                        neighborNode.FCost = neighborNode.GCost + neighborNode.HCost;

                        openSet.Add(neighborNode);
                    }
                }
            }

            return null;
        }

        private double CalculateHCost(Node neighborNode, Node endNode)
        {
            throw new NotImplementedException();
        }

        private double CalculateGCost(Node currentNode, Node neighborNode)
        {
            if (currentNode.Point == neighborNode.Point)
            {
                throw new Exception("currentNode.Point == neighborNode.Point in PathFinder.CalculateGCost()");
            }

            if (currentNode.Point.X == neighborNode.Point.X)
            {
                return Math.Abs(currentNode.Point.Y - neighborNode.Point.Y);
            }

            if (currentNode.Point.Y == neighborNode.Point.Y)
            {
                return Math.Abs(currentNode.Point.X - neighborNode.Point.X);
            }

            return CalculateHypotenuse(currentNode, neighborNode);
        }

        private double CalculateHypotenuse(Node currentNode, Node neighborNode)
        {
            throw new NotImplementedException();
        }

        private List<Node> ReconstructPath(Node startNode, Node endNode)
        {
            throw new NotImplementedException();
        }
    }
}
