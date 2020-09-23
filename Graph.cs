using System.Collections.Generic;

namespace Graph
{
    public class Node
    {
        public int[] connections;

        public Node(int[] connections)
        {
            this.connections = connections;
        }
    }

    public class Graph
    {
        private Node[] m_nodes;

        public Graph(Node[] nodeList)
        {
            m_nodes = nodeList;
        }

        public Graph(int[][] nodeList)
        {
            m_nodes = new Node[nodeList.Length];
            for (int i = 0; i < nodeList.Length; i++)
            {
                m_nodes[i] = new Node(nodeList[i]);
            }
        }

        public IntMatrix CreateAdjacencyMatrix()
        {
            var result = new IntMatrix(m_nodes.Length, m_nodes.Length);
            for (int i = 0; i < m_nodes.Length; i++)
            {
                foreach (int j in m_nodes[i].connections)
                    result[i, j] = 1;
            }
            return result;
        }

        public IntMatrix CreateUndirectedIncidenceMatrix()
        {
            // for first we need to know how many edges there are in the graph
            // somehow label the edges
            var edges = new Dictionary<(int, int), int>();
            int edgeCount = 0;
            for (int i = 0; i < m_nodes.Length; i++)
            {
                foreach (int j in m_nodes[i].connections)
                {
                    if (edges.ContainsKey((j, i)) == false)
                    {
                        edges.Add((i, j), edgeCount);
                        edgeCount++;
                    }
                }
            }

            // now construct the matrix where:
            // height = # of verteces
            // width = # of edges
            IntMatrix result = new IntMatrix(m_nodes.Length, edgeCount);
            foreach (var ((from, to), i) in edges)
            {
                result[from, i] = 1;
                result[to, i] = 1;
            }

            return result;
        }

        // only works right for a directed simple graph, that is
        // there is at most one edge between any two vertices
        public IntMatrix CreateDirectedIncidenceMatrix()
        {
            // for first we need to know how many edges there are in the graph
            // somehow label the edges
            var edges = new Dictionary<(int, int), int>();
            int edgeCount = 0;
            for (int i = 0; i < m_nodes.Length; i++)
            {
                foreach (int j in m_nodes[i].connections)
                {
                    edges.Add((i, j), edgeCount);
                    edgeCount++;
                }
            }

            // now construct the matrix where:
            // height = # of verteces
            // width = # of edges
            IntMatrix result = new IntMatrix(m_nodes.Length, edgeCount);
            foreach (var ((from, to), i) in edges)
            {
                result[from, i] = 1;
                result[to, i] = -1;
            }

            return result;
        }

        public IntMatrix CreateKirchhoffMatrix()
        {
            var result = new IntMatrix(m_nodes.Length, m_nodes.Length);
            for (int i = 0; i < m_nodes.Length; i++)
            {
                foreach (int j in m_nodes[i].connections)
                    result[i, j] = -1;

                result[i, i] = m_nodes[i].connections.Length;
            }
            return result;
        }

        public static IntMatrix ConvertKirchhoffToAdjacency(IntMatrix kirchhoff)
        {
            var result = -kirchhoff;
            for (int i = 0; i < result.Height; i++)
            {
                result[i, i] = 0;
            }
            return result;
        }

        /*
            The definition of Kirchoff for a directed graph is inconsistent
            https://www.wikiwand.com/en/Laplacian_matrix#/Definition
            By this definition, if i != j, the entry in the matrix is 
            -1 if vi and vj are adjacent. Now, the way adjacency is defined
            for vertices doesn't state whether, in a directed graph, if there
            is a connection vi -> vj, vj is to be considered adjacent to vi.
            In my mind, it is not.
            However, when I apply the formula at 
            https://www.wikiwand.com/en/Laplacian_matrix#/Incidence_matrix
            it seems to always give the result as though they were both adjacent,
            which is why this conversion diverges in its results from 
            Incidence -> Kirchhoff conversion function (that one always results 
            in a simmetric matrix).
        */
        public static IntMatrix ConvertAdjacencyToKirchhoff(IntMatrix adjacency)
        {
            var result = -adjacency;
            for (int i = 0; i < adjacency.Height; i++)
            {
                for (int j = 0; j < adjacency.Width; j++)
                    result[i, i] += adjacency[i, j];
            }
            return result;
        }

        public static IntMatrix ConvertUndirectedIncidenceToAdjacency(IntMatrix incidence)
        {
            var result = new IntMatrix(incidence.Height, incidence.Height);
            List<int> currentEdge = new List<int>(2);

            for (int j = 0; j < incidence.Width; j++)
            {
                for (int i = 0; i < incidence.Height; i++)
                {
                    if (incidence[i, j] == 1)
                        currentEdge.Add(i);
                }
                result[currentEdge[0], currentEdge[1]] = 1;
                result[currentEdge[1], currentEdge[0]] = 1;
                currentEdge.Clear();
            }
            return result;
        }

        public static IntMatrix ConvertDirectedIncidenceToAdjacency(IntMatrix incidence)
        {
            var result = new IntMatrix(incidence.Height, incidence.Height);

            for (int i = 0; i < incidence.Width; i++)
            {
                // we need to initialize the values anyway
                int from = 0, to = 0;

                for (int j = 0; j < incidence.Height; j++)
                {
                    if (incidence[j, i] == 1)
                        from = j;
                    else if (incidence[j, i] == -1)
                        to = j;
                }
                result[from, to] = 1;
                // result[to, from] = -1;
            }
            return result;
        }

        // basically diplicate logic for the default list graph representation I used
        public static IntMatrix ConvertAdjacencyToUndirectedIncidence(IntMatrix adjacency)
        {
            // for first we need to know how many edges there are in the graph
            // somehow label the edges
            var edges = new Dictionary<(int, int), int>();
            int edgeCount = 0;
            for (int i = 0; i < adjacency.Height; i++)
            {
                for (int j = 0; j < adjacency.Width; j++)
                {
                    if (adjacency[i, j] == 1 && edges.ContainsKey((j, i)) == false)
                    {
                        edges.Add((i, j), edgeCount);
                        edgeCount++;
                    }
                }
            }

            // now construct the matrix where:
            // height = # of verteces
            // width = # of edges
            IntMatrix result = new IntMatrix(adjacency.Height, edgeCount);
            foreach (var ((from, to), i) in edges)
            {
                result[from, i] = 1;
                result[to, i] = 1;
            }

            return result;
        }

        public static IntMatrix ConvertAdjacencyToDirectedIncidence(IntMatrix adjacency)
        {
            // for first we need to know how many edges there are in the graph
            // somehow label the edges
            var edges = new List<(int, int)>();
            for (int i = 0; i < adjacency.Height; i++)
            {
                for (int j = 0; j < adjacency.Width; j++)
                {
                    if (adjacency[i, j] == 1)
                    {
                        edges.Add((i, j));
                    }
                }
            }

            // now construct the matrix where:
            // height = # of verteces
            // width = # of edges
            IntMatrix result = new IntMatrix(adjacency.Height, edges.Count);
            for (int i = 0; i < edges.Count; i++)
            {
                var (from, to) = edges[i];
                result[from, i] = 1;
                result[to, i] = -1;
            }

            return result;
        }

        // K = M_t * M
        public static IntMatrix ConvertDirectedIncidenceToKirchhoff(IntMatrix incidence)
        {
            return incidence.Mult(incidence.Transpose);
        }


        public enum Mark
        {
            Not_Visited = 0,
            Visited = 1,
            Investigated = 2
        }

        public void TraverseBreadthFirst(int start)
        {
            var marks = new Mark[m_nodes.Length];
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(start);

            while (queue.Count != 0)
            {
                int currentNodeIndex = queue.Dequeue();

                if (marks[currentNodeIndex] == Mark.Not_Visited)
                {
                    System.Console.WriteLine($"Visiting node {currentNodeIndex}");
                    marks[currentNodeIndex] = Mark.Visited;

                    var node = m_nodes[currentNodeIndex];
                    foreach (int index in node.connections)
                        queue.Enqueue(index);
                }
                else
                {
                    System.Console.WriteLine($"Skipping node {currentNodeIndex}");
                }
            }
        }

        public void TraverseDepthFirst(int start)
        {
            var marks = new Mark[m_nodes.Length];
            _TraverseDepthFirst(start, marks);
        }

        private void _TraverseDepthFirst(int currentNodeIndex, Mark[] marks)
        {
            if (marks[currentNodeIndex] == Mark.Not_Visited)
            {
                System.Console.WriteLine($"Visiting node {currentNodeIndex}");

                marks[currentNodeIndex] = Mark.Visited;

                foreach (int next in m_nodes[currentNodeIndex].connections)
                    _TraverseDepthFirst(next, marks);
            }
            else
            {
                System.Console.WriteLine($"Skipping node {currentNodeIndex}");
            }
        }
    }
}