using System;
using System.Collections.Generic;
using Graph;

namespace Algorithm
{


    public class DominatingSets
    {
        public static HashSet<(int, int)> Run(Graph.Graph graph)
        {
            var algo = new DominatingSets(graph);
            return algo.Iterate();
        }

        private HashSet<int> prevAddedVertices; // T
        private (int, int) prevAddedPotentialSolution; // S_p_j (vertex index, column index)
        private HashSet<(int, int)> currentOptimalSolution; // B_star
        private HashSet<(int, int)> potentialOptimalSolution; // B
        private HashSet<int> usedVertices; // E
        private Dictionary<int, Block> blocks;

        private int numVertices;

        private DominatingSets(Graph.Graph graph)
        {
            this.prevAddedVertices = new HashSet<int>();
            this.prevAddedPotentialSolution = (0, 0);

            this.currentOptimalSolution = new HashSet<(int, int)>();
            this.potentialOptimalSolution = new HashSet<(int, int)>();

            this.usedVertices = new HashSet<int>(numVertices);
            this.numVertices = graph.Nodes.Length;
            this.blocks = InitializeBlocks(graph);
        }

        private Dictionary<int, Block> InitializeBlocks(Graph.Graph graph)
        {
            var blocks = new Dictionary<int, Block>();

            for (int i = 0; i < graph.Nodes.Length; i++)
            {
                var column = new HashSet<int>(graph.Nodes[i].connections.Length);
                int indexOfBlockToAppendTo = i;
                bool isIndexNotOverwritten() => indexOfBlockToAppendTo == i;

                column.Add(i);

                foreach (int connection in graph.Nodes[i].connections)
                {
                    if (isIndexNotOverwritten() && blocks.ContainsKey(connection))
                        indexOfBlockToAppendTo = connection;

                    column.Add(connection);
                }

                if (isIndexNotOverwritten())
                {
                    blocks[i] = new Block();
                }

                blocks[indexOfBlockToAppendTo].AppendColumn(column);
            }
            return blocks;
        }

        int count = 0;

        private HashSet<(int, int)> Iterate()
        {
            if (count++ > 5) return null;

            // step 2
            int nextVertexIndex = GetMinNotInSet(usedVertices, blocks.Count); // p

            Block selectedBlock;

            if (!blocks.TryGetValue(nextVertexIndex, out selectedBlock) || selectedBlock.AreAllColumnsUsed())
            {
                // step 5
                if (nextVertexIndex == 0)
                    return currentOptimalSolution;

                UndoLastMerge();
                System.Console.WriteLine("Refilling");
                selectedBlock?.RefillUnused();
                return Iterate();
            }

            if (currentOptimalSolution.Count == 0
                || potentialOptimalSolution.Count + 1 < currentOptimalSolution.Count)
            {
                // step 4
                var (nextColumnIndex, nextColumn) = selectedBlock.GetNextColumn();
                AddPotentialSolution(nextVertexIndex, nextColumnIndex, nextColumn);

                if (usedVertices.Count < numVertices)
                    // step 2 (iterate again)
                    return Iterate();

                MarkPotentialSolutionAsCurrentOptimal();
            }
            UndoLastMerge();

            return Iterate();
        }

        private void AddPotentialSolution(int vertexIndex, int columnIndex, HashSet<int> vertices)
        {
            prevAddedPotentialSolution = (vertexIndex, columnIndex);

            prevAddedVertices.Clear();
            prevAddedVertices.UnionWith(vertices);
            prevAddedVertices.ExceptWith(usedVertices);

            potentialOptimalSolution.Add(prevAddedPotentialSolution);
            usedVertices.UnionWith(prevAddedVertices);
        }

        private void UndoLastMerge()
        {
            if (prevAddedVertices != null)
            {
                potentialOptimalSolution.Remove(prevAddedPotentialSolution);
                usedVertices.ExceptWith(prevAddedVertices);
                prevAddedVertices.Clear();
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