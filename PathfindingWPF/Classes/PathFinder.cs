namespace PathfindingWPF.Classes
{
    internal class PathFinder
    {
        private HashSet<Node> _closedSet = new(); // Nodes that have already been evaluated
        private List<Node> _openSet = new(); // Nodes that need to be evaluated

        // Starts the pathfinding algorithm from startNode to endNode
        public List<Node> Start(Node startNode, Node endNode)
        {
            _openSet.Add(startNode); // Add the starting node to the open set

            // Loop until there are no more nodes to evaluate
            while (_openSet.Any())
            {
                Node currentNode = GetCurrentNode(); // Get the node with the lowest cost

                // If the end node is reached, reconstruct and return the path
                if (currentNode == endNode)
                {
                    return ReconstructPath(startNode, endNode);
                }

                // Move the current node from open set to closed set
                _openSet.Remove(currentNode);
                _closedSet.Add(currentNode);

                // Evaluate the costs of the neighboring nodes
                CalculateNeighborNodeCosts(currentNode, endNode);
            }

            // Return an empty path if no path is found
            return new List<Node>();
        }

        // Calculates the costs for the neighboring nodes of the current node
        private void CalculateNeighborNodeCosts(Node currentNode, Node endNode)
        {
            foreach (Node neighborNode in currentNode.GetNeighborNodes())
            {
                // Ignore the neighbor if it is already evaluated
                if (_closedSet.Contains(neighborNode)) continue;

                // If the neighbor is not in the open set, add it and calculate its costs
                if (true /*!_openSet.Contains(neighborNode)*/)
                {
                    neighborNode.ParentNode = currentNode; // Set the parent node for path reconstruction
                    neighborNode.CalculateCosts(currentNode, endNode); // Calculate the costs
                    _openSet.Add(neighborNode); // Add the neighbor to the open set
                }
            }
        }

        // Gets the node with the lowest cost from the open set
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

        // Reconstructs the path from the end node to the start node
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
