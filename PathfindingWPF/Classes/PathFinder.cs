namespace PathfindingWPF.Classes
{
    internal class PathFinder
    {
        private HashSet<Node> _closedSet = new();
        private List<Node> _openSet = new();

        public List<Node>? Start(Node startNode, Node endNode)
        {
            _openSet.Add(startNode);

            while (_openSet.Count > 0)
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

            return null;
        }

        private void CalculateNeighborNodeCosts(Node currentNode, Node endNode)
        {
            foreach (Node neighborNode in currentNode.GetNeighborNodes())
            {
                if (!_closedSet.Contains(neighborNode))
                {
                    neighborNode.ParentNode = currentNode;
                    neighborNode.CalculateCosts(currentNode, endNode);

                    _openSet.Add(neighborNode);
                }
            }
        }

        private Node GetCurrentNode()
        {
            _openSet = _openSet.OrderBy(node => node.FinalCost).ToList();
            Node currentNode = _openSet[0];

            if (_openSet.Count > 1 && _openSet[0].FinalCost == _openSet[1].FinalCost)
            {
                double heuristicCost = _openSet[0].HeuristicCost;

                foreach (Node node in _openSet)
                {
                    if (node.FinalCost == _openSet[0].FinalCost && heuristicCost > node.HeuristicCost)
                    {
                        heuristicCost = node.HeuristicCost;
                        currentNode = node;
                    }
                }
            }

            return currentNode;
        }

        private List<Node> ReconstructPath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();

            Node start = startNode;
            Node current = endNode;

            while (start != current)
            {
                path.Add(current);
                current = current.ParentNode;
            }
            path.Add(current);
            return path;
        }
    }
}
