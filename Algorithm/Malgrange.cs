using System.Collections.Generic;
using Graph;

namespace Algorithm
{
    public class IterationResult
    {
        public HashSet<SubMatrix> nextCoverage;
        public HashSet<SubMatrix> combinations;
    }

    public static class Malgrange
    {
        public static HashSet<HashSet<int>> Run(Graph.Graph graph)
        {
            // Initial conditions
            var adjacency = graph.CreateAdjacencyMatrix();
            var invertedAdjacency = adjacency.Fill(1) - adjacency;
            var coverage = IntializeCoverage(invertedAdjacency); // C
            var subCoverage = new HashSet<SubMatrix>();          // X

            while (true)
            {
                var nextCoverage = Iteration(coverage, subCoverage);

                if (nextCoverage.SetEquals(coverage))
                    break;

                coverage = nextCoverage;
                subCoverage = CalculateSubCoverage(nextCoverage);
            }

            var stableSets = ExtractStableSets(coverage);
            var result = ExtractResult(stableSets);

            return result;
        }

        private static HashSet<SubMatrix> IntializeCoverage(Int2dMatrix invertedAdjacency)
        {
            var coverage = new HashSet<SubMatrix>();
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
                    coverage.Add(subMatrix);
                }
            }
            return coverage;
        }

        private static HashSet<SubMatrix> Iteration(HashSet<SubMatrix> prevCoverage, HashSet<SubMatrix> subCoverage)
        {
            var withoutSubCoverage = new HashSet<SubMatrix>(prevCoverage);
            withoutSubCoverage.ExceptWith(subCoverage);
            var nextCoverage = new HashSet<SubMatrix>(withoutSubCoverage);

            foreach (var subMatrixA in withoutSubCoverage)
            {
                foreach (var subMatrixB in withoutSubCoverage)
                {
                    var intersectionSubMatrix = subMatrixA.IntersectionStar(subMatrixB);

                    if (intersectionSubMatrix.Width != 0 && intersectionSubMatrix.Height != 0)
                        nextCoverage.Add(intersectionSubMatrix);

                    var unionSubMatrix = subMatrixA.UnionStar(subMatrixB);

                    if (unionSubMatrix.Width != 0 && unionSubMatrix.Height != 0)
                        nextCoverage.Add(unionSubMatrix);
                }
            }

            return nextCoverage;
        }

        private static HashSet<SubMatrix> CalculateSubCoverage(HashSet<SubMatrix> coverage)
        {
            var subCoverage = new HashSet<SubMatrix>();
            foreach (var subMatrixA in coverage)
            {
                foreach (var subMatrixB in coverage)
                {
                    if (subMatrixA.IsProperSubMatrixOf(subMatrixB))
                        subCoverage.Add(subMatrixA);
                }
            }
            return subCoverage;
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