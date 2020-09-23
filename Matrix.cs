using System.Text;

namespace Graph
{
    public class IntMatrix
    {
        protected int[,] m_numbers;

        public int this[int x, int y]
        {
            get { return m_numbers[x, y]; }
            set { m_numbers[x, y] = value; }
        }

        public int Width => m_numbers.GetLength(1);
        public int Height => m_numbers.GetLength(0);

        public IntMatrix(int height, int width)
        {
            m_numbers = new int[height, width];
        }

        public IntMatrix(int[,] numbers)
        {
            m_numbers = numbers;
        }

        public IntMatrix(IntMatrix matrix)
        {
            m_numbers = (int[,])matrix.m_numbers.Clone();
        }

        public IntMatrix Mult(IntMatrix mat)
        {
            if (Width != mat.Height)
            {
                throw new System.Exception("The width doesn't match the height of the matrix multiplying into");
            }
            var result = new IntMatrix(Height, mat.Width);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < mat.Width; j++)
                {
                    for (int k = 0; k < Width; k++)
                    {
                        result.m_numbers[i, j] += m_numbers[i, k] * mat.m_numbers[k, j];
                    }
                }
            }
            return result;
        }

        public IntMatrix Transpose
        {
            get
            {
                var result = new IntMatrix(Width, Height);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        result.m_numbers[j, i] = m_numbers[i, j];
                    }
                }
                return result;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("", Width * Height * 4 + 20);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    sb.AppendFormat("{0,3}", m_numbers[i, j]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static IntMatrix operator -(IntMatrix matrix)
        {
            var result = new IntMatrix(matrix.Height, matrix.Width);
            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                    result.m_numbers[i, j] = -matrix.m_numbers[i, j];
            }
            return result;
        }

        public IntMatrix Fill(int value)
        {
            IntMatrix result = new IntMatrix(Height, Width);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                    result.m_numbers[i, j] = value;
            }
            return result;
        }

        public static IntMatrix operator -(IntMatrix lhs, IntMatrix rhs)
        {
            if (lhs.Width != rhs.Width || lhs.Height != rhs.Height)
            {
                throw new System.Exception("Dimensions didn't match, couldn't subtract the two matrices");
            }
            var result = new IntMatrix(lhs.Height, lhs.Width);
            for (int i = 0; i < lhs.Height; i++)
            {
                for (int j = 0; j < lhs.Width; j++)
                    result.m_numbers[i, j] = lhs.m_numbers[i, j] - rhs.m_numbers[i, j];
            }
            return result;
        }
    }
}