namespace PathfindingWPF.Classes
{
    internal class PathFinder
    {
        private readonly HashSet<Node> _closedSet = new();
        private readonly List<Node> _openSet = new();

        public List<Node> Start(Node startNode, Node endNode)
        {
            _openSet.Add(startNode);

            while (_openSet.Any())
            {
                Node currentNode = GetCurrentNode();

                if (currentNode == endNode)
                {
                    return ReconstructPath(startNode, endNode);
                }

                _openSet.Remove(currentNode);
                _closedSet.Add(currentNode);

                CalculateNeighborNodeCosts(currentNode, endNode);
            }

            return new List<Node>();
        }

        private void CalculateNeighborNodeCosts(Node currentNode, Node endNode)
        {
            foreach (Node neighborNode in currentNode.GetNeighborNodes())
            {
                if (_closedSet.Contains(neighborNode)) continue;

                if (!_openSet.Contains(neighborNode))
                {
                    neighborNode.ParentNode = currentNode;
                    neighborNode.CalculateCosts(currentNode, endNode);
                    _openSet.Add(neighborNode);
                }
            }
        }

        private Node GetCurrentNode()
        {
            _openSet.Sort((node1, node2) =>
                node1.FinalCost == node2.FinalCost
                    ? node1.HeuristicCost.CompareTo(node2.HeuristicCost)
                    : node1.FinalCost.CompareTo(node2.FinalCost));

            return _openSet.First();
        }

        private List<Node> ReconstructPath(Node startNode, Node endNode)
        {
            List<Node> path = new();
            for (Node current = endNode; current != startNode; current = current.ParentNode)
            {
                path.Add(current);
            }
            path.Add(startNode);
            path.Reverse();
            return path;
        }
    }
}
