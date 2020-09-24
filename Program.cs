// Run with `dotnet run`
using System;

namespace Graph
{
    class Program
    {
        static void Main(string[] args)
        {
            // UndirectedDemo();
            // DirectedDemo();
            // UndirectedTraversalDemo();
            // DirectedTraversalDemo();
            // MalgrangeDemo();
            DominatingSetsDemo();
        }

        static void UndirectedDemo()
        {
            var graph = new Graph(
                new int[][]
                {
                    new int [] { 1, 2, 3 }, // 0 -> 1, 2
                    new int [] { 0, 2 },    // 1 -> 0, 2
                    new int [] { 0, 1, 3 }, // 2 -> 0, 1, 3
                    new int [] { 0, 2 }     // 3 -> 0, 2
                }
            );

            var adjacency = graph.CreateAdjacencyMatrix();
            var incidence = graph.CreateUndirectedIncidenceMatrix();
            var kirchhoff = graph.CreateKirchhoffMatrix();

            Console.WriteLine("Adjacency:");
            Console.WriteLine(adjacency);
            Console.WriteLine("Undirected incidence:");
            Console.WriteLine(incidence);
            Console.WriteLine("Kirchhoff:");
            Console.WriteLine(kirchhoff);

            Console.WriteLine("\nConversions. \n");
            Console.WriteLine("Adjacency -> Kirchhoff");
            Console.WriteLine(Graph.ConvertAdjacencyToKirchhoff(adjacency));
            Console.WriteLine("Kirchhoff -> Adjacency");
            Console.WriteLine(Graph.ConvertKirchhoffToAdjacency(kirchhoff));
            Console.WriteLine("Adjacency -> Incidence");
            Console.WriteLine(Graph.ConvertAdjacencyToUndirectedIncidence(adjacency));
            Console.WriteLine("Incidence -> Adjacency");
            Console.WriteLine(Graph.ConvertUndirectedIncidenceToAdjacency(incidence));
            // Console.WriteLine("Incidence -> Kirchhoff (only works right for a directed graph)");
            // Console.WriteLine(Graph.ConvertDirectedIncidenceToKirchhoff(incidence));
        }

        static void DirectedDemo()
        {
            var graph = new Graph(
                new int[][]
                {
                    new int [] { 1, 2 },    // 0 -> 1, 2
                    new int [] { 2 },       // 1 -> 2
                    new int [] { },         // 2 -> x
                    new int [] { 1, 2 }     // 3 -> 1, 2
                }
            );

            var adjacency = graph.CreateAdjacencyMatrix();
            var incidence = graph.CreateDirectedIncidenceMatrix();
            var kirchhoff = graph.CreateKirchhoffMatrix();

            Console.WriteLine("Adjacency:");
            Console.WriteLine(adjacency);
            Console.WriteLine("Directed incidence:");
            Console.WriteLine(incidence);
            Console.WriteLine("Kirchhoff:");
            Console.WriteLine(kirchhoff);

            Console.WriteLine("\nConversions. \n");
            Console.WriteLine("Adjacency -> Kirchhoff");
            Console.WriteLine(Graph.ConvertAdjacencyToKirchhoff(adjacency));
            Console.WriteLine("Kirchhoff -> Adjacency");
            Console.WriteLine(Graph.ConvertKirchhoffToAdjacency(kirchhoff));
            Console.WriteLine("Adjacency -> Incidence");
            Console.WriteLine(Graph.ConvertAdjacencyToDirectedIncidence(adjacency));
            Console.WriteLine("Incidence -> Adjacency");
            Console.WriteLine(Graph.ConvertDirectedIncidenceToAdjacency(incidence));
            Console.WriteLine("Incidence -> Kirchhoff (only works right for a directed graph)");
            Console.WriteLine(Graph.ConvertDirectedIncidenceToKirchhoff(incidence));
        }

        static void UndirectedTraversalDemo()
        {
            var graph = new Graph(
                new int[][]
                {
                    new int [] { 1, 2, 3 }, // 0 -> 1, 2
                    new int [] { 0, 2 },    // 1 -> 0, 2
                    new int [] { 0, 1, 3 }, // 2 -> 0, 1, 3
                    new int [] { 0, 2 }     // 3 -> 0, 2
                }
            );

            // starting node index
            int start = 0;

            System.Console.WriteLine($"\nTraversing breadth first starting from {start}.");
            graph.TraverseBreadthFirst(start);
            System.Console.WriteLine($"\nTraversing depth first starting from {start}.");
            graph.TraverseDepthFirst(start);
        }

        static void DirectedTraversalDemo()
        {
            var graph = new Graph(
                new int[][]
                {
                    new int [] { 1, 2 },    // 0 -> 1
                    new int [] { 2 },       // 1 -> 2
                    new int [] { 3, 1 },    // 2 -> 3, 1
                    new int [] { 1, 2 }     // 3 -> 1, 2
                }
            );

            // starting node index
            int start = 0;

            System.Console.WriteLine($"\nTraversing breadth first starting from {start}.");
            graph.TraverseBreadthFirst(start);
            System.Console.WriteLine($"\nTraversing depth first starting from {start}.");
            graph.TraverseDepthFirst(start);
        }

        static void MalgrangeDemo()
        {
            // var graph = new Graph(
            //     // 0 — 1 — 2 — 4
            //     //          \
            //     //           3
            //     new int[][]
            //     {
            //         new int [] { 1 },
            //         new int [] { 0, 2 },
            //         new int [] { 1, 3, 4},
            //         new int [] { 2 },
            //         new int [] { 2 }
            //     }
            // );
            var graph = new Graph(
                new int[][]
                {
                    new int [] { 1, 2, 4 },
                    new int [] { 0, 4, 3 },
                    new int [] { 0, 5, 3, 4},
                    new int [] { 1, 2, 4 },
                    new int [] { 0, 1, 2, 3, 5 },
                    new int [] { 4, 2 }
                }
            );
            var maxStableSets = Algorithm.Malgrange.Run(graph);

            foreach (var set in maxStableSets)
            {
                foreach (var i in set)
                {
                    System.Console.Write($"{i}, ");
                }
                System.Console.WriteLine();
            }
        }

        static void DominatingSetsDemo()
        {
            // 2 — 1 — 3
            // |
            // 0 — 4
            var graph = new Graph(
                new int[][]
                {
                    new int [] { 2, 4 },
                    new int [] { 2, 3 },
                    new int [] { 0, 1 },
                    new int [] { 1 },
                    new int [] { 0 },
                }
            );

            var result = Algorithm.DominatingSets.Run(graph);
            foreach (var vertexIndex in result)
            {
                System.Console.WriteLine(vertexIndex);
            }
        }
    }
}
