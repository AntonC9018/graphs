using System.Collections.Generic;
using System.Linq;

namespace Algorithm
{
    public class DominatingSets
    {
        public static IEnumerable<int> Run(Graph.Graph graph)
        {
            var algo = new DominatingSets(graph);
            var result = algo.Iterate();
            // keep the vertex indices only
            foreach (var solution in result)
            {
                yield return solution.vertexIndex;
            }
        }

        private struct Solution
        {
            public int vertexIndex, columnIndex;
            public Solution(int vertexIndex, int columnIndex)
            {
                this.vertexIndex = vertexIndex;
                this.columnIndex = columnIndex;
            }
        }

        private Stack<HashSet<int>> prevAddedVertices; // T
        private Stack<Solution> prevAddedPotentialSolutions; // S_p_j
        private HashSet<Solution> currentOptimalSolution; // B_star
        private HashSet<Solution> potentialOptimalSolution; // B
        private HashSet<int> usedVertices; // E
        private Dictionary<int, Block> blocks;

        private int numVertices;

        private DominatingSets(Graph.Graph graph)
        {
            this.prevAddedVertices = new Stack<HashSet<int>>();
            this.prevAddedPotentialSolutions = new Stack<Solution>();

            this.currentOptimalSolution = new HashSet<Solution>();
            this.potentialOptimalSolution = new HashSet<Solution>();

            this.usedVertices = new HashSet<int>(numVertices);
            this.numVertices = graph.Nodes.Length;

            // step 1
            this.blocks = InitializeBlocks(graph);
        }

        private Dictionary<int, Block> InitializeBlocks(Graph.Graph graph)
        {
            var blocks = new Dictionary<int, Block>();

            for (int i = 0; i < graph.Nodes.Length; i++)
            {
                var column = new HashSet<int>(graph.Nodes[i].connections.Length);

                int min = graph.Nodes[i].connections.Min();
                int indexOfBlockToAppendTo = min < i ? min : i;

                column.Add(i);

                foreach (int connection in graph.Nodes[i].connections)
                    column.Add(connection);

                if (!blocks.ContainsKey(indexOfBlockToAppendTo))
                {
                    blocks[indexOfBlockToAppendTo] = new Block();
                }

                blocks[indexOfBlockToAppendTo].AppendColumn(column);
            }
            return blocks;
        }

        private HashSet<Solution> Iterate()
        {
            // step 2
            int nextVertexIndex = GetMinNotInSet(usedVertices, blocks.Count); // p

            Block selectedBlock;

            if (!blocks.TryGetValue(nextVertexIndex, out selectedBlock) || selectedBlock.AreAllColumnsUsed())
            {
                // step 5
                if (nextVertexIndex == 0)
                    return currentOptimalSolution;

                UndoLastMerge();
                selectedBlock?.RefillUnused();
                return Iterate(); // step 2 (iterate again)
            }

            if (currentOptimalSolution.Count == 0
                || potentialOptimalSolution.Count + 1 < currentOptimalSolution.Count)
            {
                // step 4
                var (nextColumnIndex, nextColumn) = selectedBlock.GetNextColumn();
                AddPotentialSolution(nextVertexIndex, nextColumnIndex, nextColumn);

                if (usedVertices.Count < numVertices)
                    return Iterate(); // step 2 (iterate again)

                MarkPotentialSolutionAsCurrentOptimal();
            }
            // step 6
            UndoLastMerge();
            return Iterate();
        }

        private void AddPotentialSolution(int vertexIndex, int columnIndex, HashSet<int> vertices)
        {
            var solution = new Solution(vertexIndex, columnIndex);
            prevAddedPotentialSolutions.Push(solution);

            var verticesCopy = new HashSet<int>(vertices);
            verticesCopy.ExceptWith(usedVertices);
            prevAddedVertices.Push(verticesCopy);

            potentialOptimalSolution.Add(solution);
            usedVertices.UnionWith(verticesCopy);
        }

        private void UndoLastMerge()
        {
            if (prevAddedPotentialSolutions.Count > 0)
            {
                potentialOptimalSolution.Remove(prevAddedPotentialSolutions.Pop());
                usedVertices.ExceptWith(prevAddedVertices.Pop());
            }
        }

        private void MarkPotentialSolutionAsCurrentOptimal()
        {
            currentOptimalSolution.Clear();
            currentOptimalSolution.UnionWith(potentialOptimalSolution);
        }

        private int GetMinNotInSet(HashSet<int> set, int valueLimit)
        {
            for (int i = 0; i < valueLimit; i++)
            {
                if (!set.Contains(i))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}