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
                openSet = openSet.OrderBy(node => node.FCost).ToList();
                Node currentNode = openSet[0];

                if (openSet.Count > 1 && openSet[0].FCost == openSet[1].FCost)
                {
                    double hCost = openSet[0].HCost;

                    foreach (Node node in openSet)
                    {
                        if (node.FCost == openSet[0].FCost && hCost > node.HCost)
                        {
                            hCost = node.HCost;
                            currentNode = node;
                        }
                    }
                }

                if (currentNode == endNode)
                {
                    return ReconstructPath(startNode, endNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Node neighborNode in currentNode.GetNeighborNodes())
                {
                    if (!closedSet.Contains(neighborNode))
                    {
                        neighborNode.NextHop = currentNode;
                        neighborNode.GCost = CalculateGCost(currentNode, neighborNode);
                        neighborNode.HCost = CalculateHypotenuse(neighborNode, endNode);
                        neighborNode.FCost = neighborNode.GCost + neighborNode.HCost;

                        openSet.Add(neighborNode);
                    }
                }
            }

            return null;
        }

        private double CalculateGCost(Node currentNode, Node neighborNode)
        {
            if (currentNode.Point == neighborNode.Point)
            {
                throw new Exception("CalculateGCost");
            }

            return CalculateHypotenuse(currentNode, neighborNode) + currentNode.GCost;
        }

        private double CalculateHypotenuse(Node node1, Node node2)
        {
            double x = Math.Abs(node1.Point.X - node2.Point.X);
            double y = Math.Abs(node1.Point.Y - node2.Point.Y);
            
            return Math.Sqrt(x * x + y * y);
        }

        private List<Node> ReconstructPath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();

            Node start = startNode;
            Node current = endNode;

            while (start != current)
            {
                path.Add(current);
                current = current.NextHop;
            }
            path.Add(current);
            return path;
        }
    }
}
