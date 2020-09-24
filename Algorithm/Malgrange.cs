using System.Collections.Generic;
using Graph;

namespace Algorithm
{
    public static class Malgrange
    {
        public static HashSet<HashSet<int>> Run(Graph.Graph graph)
        {
            // Initial conditions
            var adjacency = graph.CreateAdjacencyMatrix();
            var invertedAdjacency = adjacency.Fill(1) - adjacency;
            var cover = IntializeCover(invertedAdjacency); // C
            var subCover = new HashSet<SubMatrix>();          // X

            while (true)
            {
                var nextCover = Iteration(cover, subCover);

                if (nextCover.SetEquals(cover))
                    break;

                cover = nextCover;
                subCover = CalculateSubCover(nextCover);
            }

            var stableSets = ExtractStableSets(cover);
            var result = ExtractResult(stableSets);

            return result;
        }

        private static HashSet<SubMatrix> IntializeCover(IntMatrix invertedAdjacency)
        {
            var cover = new HashSet<SubMatrix>();
            for (int i = 0; i < invertedAdjacency.Height; i++)
            {
                var subMatrix = new SubMatrix();

                for (int j = 0; j < invertedAdjacency.Width; j++)
                {
                    if (invertedAdjacency[i, j] == 1)
                        subMatrix.columnIndices.Add(j);
                }

                if (subMatrix.columnIndices.Count > 0)
                {
                    subMatrix.rowIndices.Add(i);
                    cover.Add(subMatrix);
                }
            }
            return cover;
        }

        private static HashSet<SubMatrix> Iteration(HashSet<SubMatrix> prevCover, HashSet<SubMatrix> subCover)
        {
            var withoutSubCover = new HashSet<SubMatrix>(prevCover);
            withoutSubCover.ExceptWith(subCover);
            var nextCover = new HashSet<SubMatrix>(withoutSubCover);

            foreach (var subMatrixA in withoutSubCover)
            {
                foreach (var subMatrixB in withoutSubCover)
                {
                    var intersectionSubMatrix = subMatrixA.IntersectionStar(subMatrixB);

                    if (intersectionSubMatrix.Width != 0 && intersectionSubMatrix.Height != 0)
                        nextCover.Add(intersectionSubMatrix);

                    var unionSubMatrix = subMatrixA.UnionStar(subMatrixB);

                    if (unionSubMatrix.Width != 0 && unionSubMatrix.Height != 0)
                        nextCover.Add(unionSubMatrix);
                }
            }

            return nextCover;
        }

        private static HashSet<SubMatrix> CalculateSubCover(HashSet<SubMatrix> cover)
        {
            var subCover = new HashSet<SubMatrix>();
            foreach (var subMatrixA in cover)
            {
                foreach (var subMatrixB in cover)
                {
                    if (subMatrixA.IsProperSubMatrixOf(subMatrixB))
                        subCover.Add(subMatrixA);
                }
            }
            return subCover;
        }

        private static HashSet<SubMatrix> ExtractStableSets(HashSet<SubMatrix> combinations)
        {
            var squareSets = new HashSet<SubMatrix>();
            foreach (var subMatrixA in combinations)
            {
                if (subMatrixA.IsSquare())
                    squareSets.Add(subMatrixA);
            }

            var stableSets = new HashSet<SubMatrix>();
            foreach (var subMatrixA in squareSets)
            {
                if (ContainsNoSuperMatrixOf(squareSets, subMatrixA))
                    stableSets.Add(subMatrixA);
            }

            return stableSets;
        }

        private static bool ContainsNoSuperMatrixOf(HashSet<SubMatrix> set, SubMatrix subMatrixA)
        {
            foreach (var subMatrixB in set)
            {
                if (subMatrixA.IsProperSubMatrixOf(subMatrixB))
                    return false;
            }
            return true;
        }

        private static HashSet<HashSet<int>> ExtractResult(HashSet<SubMatrix> stableSets)
        {
            var result = new HashSet<HashSet<int>>();
            foreach (var set in stableSets)
                result.Add(set.rowIndices);
            return result;
        }
    }
}