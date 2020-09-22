using System.Collections.Generic;

namespace Algorithm
{
    public class SubMatrix
    {
        public HashSet<int> rowIndices;
        public HashSet<int> columnIndices;

        public int Height => rowIndices.Count;
        public int Width => columnIndices.Count;

        public SubMatrix()
        {
            rowIndices = new HashSet<int>();
            columnIndices = new HashSet<int>();
        }

        public SubMatrix(SubMatrix matrix)
        {
            rowIndices = new HashSet<int>(matrix.rowIndices);
            columnIndices = new HashSet<int>(matrix.columnIndices);
        }

        public SubMatrix IntersectionStar(SubMatrix subMatrix)
        {
            var result = new SubMatrix(this);
            result.rowIndices.IntersectWith(subMatrix.rowIndices);
            result.columnIndices.UnionWith(subMatrix.columnIndices);
            return result;
        }

        public SubMatrix UnionStar(SubMatrix subMatrix)
        {
            var result = new SubMatrix(this);
            result.rowIndices.UnionWith(subMatrix.rowIndices);
            result.columnIndices.IntersectWith(subMatrix.columnIndices);
            return result;
        }

        public bool IsProperSubMatrixOf(SubMatrix subMatrix)
        {
            return rowIndices.IsProperSubsetOf(subMatrix.rowIndices)
                && columnIndices.IsProperSubsetOf(subMatrix.columnIndices);
        }

        public bool IsSubMatrixOf(SubMatrix subMatrix)
        {
            return rowIndices.IsSubsetOf(subMatrix.rowIndices)
                && columnIndices.IsSubsetOf(subMatrix.columnIndices);
        }

        public bool IsSquare()
        {
            return Width == Height;
        }

        public override bool Equals(object obj)
        {
            return ((SubMatrix)obj).columnIndices.SetEquals(columnIndices)
                && ((SubMatrix)obj).rowIndices.SetEquals(rowIndices);
        }

        public override int GetHashCode()
        {
            int sum = 0;
            foreach (int i in columnIndices)
                sum += i;
            foreach (int j in rowIndices)
                sum ^= j;
            return sum;
        }
    }
}