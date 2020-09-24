using System;
using System.Collections.Generic;
using System.Linq;
using Graph;

namespace Algorithm
{


    public class DominatingSets
    {
        public static IEnumerable<int> Run(Graph.Graph graph)
        {
            var algo = new DominatingSets(graph);
            var result = algo.Iterate();
            // keep the vertex indices only
            foreach (var (vertexIndex, _) in result)
            {
                yield return vertexIndex;
            }
        }

        private Stack<HashSet<int>> prevAddedVertices; // T
        private Stack<(int, int)> prevAddedPotentialSolution; // S_p_j (vertex index, column index)
        private HashSet<(int, int)> currentOptimalSolution; // B_star
        private HashSet<(int, int)> potentialOptimalSolution; // B
        private HashSet<int> usedVertices; // E
        private Dictionary<int, Block> blocks;

        private int numVertices;

        private DominatingSets(Graph.Graph graph)
        {
            this.prevAddedVertices = new Stack<HashSet<int>>();
            this.prevAddedPotentialSolution = new Stack<(int, int)>();

            this.currentOptimalSolution = new HashSet<(int, int)>();
            this.potentialOptimalSolution = new HashSet<(int, int)>();

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

        private HashSet<(int, int)> Iterate()
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
            var solution = (vertexIndex, columnIndex);
            prevAddedPotentialSolution.Push(solution);

            // prevAddedVertices.Clear();
            var verticesCopy = new HashSet<int>(vertices);
            verticesCopy.ExceptWith(usedVertices);
            prevAddedVertices.Push(verticesCopy);

            potentialOptimalSolution.Add(solution);
            usedVertices.UnionWith(verticesCopy);
        }

        private void UndoLastMerge()
        {
            if (prevAddedPotentialSolution.Count > 0)
            {
                potentialOptimalSolution.Remove(prevAddedPotentialSolution.Pop());
                usedVertices.ExceptWith(prevAddedVertices.Pop());
            }
        }

        private void MarkPotentialSolutionAsCurrentOptimal()
        {
            currentOptimalSolution.Clear();
            currentOptimalSolution.UnionWith(potentialOptimalSolution);
            // potentialOptimalSolution = new HashSet<(int, int)>();
        }

        // private void Reset()
        // {
        //     potentialOptimalSolution.Clear();
        //     usedVertices.Clear();
        // }

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