using System;
using System.Collections.Generic;
using System.Linq;

namespace MinimumFrame
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] firstStr = Console.ReadLine().Split(' ');

            var verticesCount = Convert.ToInt32(firstStr[0]);
            var edgesCount = Convert.ToInt32(firstStr[1]);

            int[,] graph = new int[verticesCount, verticesCount];
            for(int i = 0; i < graph.GetLength(0); i++)
            {
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    graph[i, j] = int.MaxValue;
                }
            }
            List<Edge> edges = new List<Edge>();

            for (int i = 0; i < edgesCount; i++)
            {
                var nextStr = Console.ReadLine().Split(' ');

                var firstVertexNumber = Convert.ToInt32(nextStr[0]) - 1;
                var secondVertexNumber = Convert.ToInt32(nextStr[1]) - 1;
                int edgeWeight = Convert.ToInt32(nextStr[2]);

                edges.Add(new Edge()
                {
                    NewVetrexNumber = firstVertexNumber,
                    SelectedVetrexNumber = secondVertexNumber,
                    Weight = edgeWeight
                });

                if (graph[firstVertexNumber, secondVertexNumber] == 0 || graph[firstVertexNumber, secondVertexNumber] > edgeWeight)
                {
                    graph[firstVertexNumber, secondVertexNumber] = edgeWeight;
                    graph[secondVertexNumber, firstVertexNumber] = edgeWeight;
                }
            }

            Console.WriteLine(PrimeAlhorithmOptimized(graph));
            // Console.WriteLine(PrimeAlhorithm(graph));
            // Console.WriteLine(KruskallAlhorithm(edges.OrderBy(x => x.Weight).ToList()));
        }

        // Сложность за n * m
        static double PrimeAlhorithm(double[,] graph)
        {
            var verticesCount = graph.GetLength(0);
            double sum = 0;

            var nextEdgeNumber = 0;

            bool[] selectedVertices = new bool[verticesCount];
            int selectedVerticesCount = 0;

            List<Edge> edges = new List<Edge>();
            
            while (selectedVerticesCount < verticesCount)
            {
                selectedVertices[nextEdgeNumber] = true;
                selectedVerticesCount++;

                if (selectedVerticesCount == verticesCount)
                    break;

                for (int i = 0; i < graph.GetLength(1); i++)
                {
                    if (graph[nextEdgeNumber, i] < int.MaxValue && !selectedVertices[i])
                    {
                        var edge = new Edge()
                        {
                            SelectedVetrexNumber = nextEdgeNumber,
                            NewVetrexNumber = i,
                            Weight = graph[nextEdgeNumber, i]
                        };

                        edges.Add(edge);
                    }
                }
                
                double minEdgeWeight = double.MaxValue;
                var newNextEdgeNumber = -1;
                var removeEdgeNumber = -1;

                for (int i = 0; i < edges.Count; i++)
                {
                    if(selectedVertices[edges[i].NewVetrexNumber])
                    {
                        edges.RemoveAt(i);
                        i--;
                        continue;
                    }

                    if(edges[i].Weight < minEdgeWeight)
                    {
                        newNextEdgeNumber = edges[i].NewVetrexNumber;
                        minEdgeWeight = edges[i].Weight;
                        removeEdgeNumber = i;
                    }
                }

                sum += minEdgeWeight;

                nextEdgeNumber = newNextEdgeNumber;
                edges.RemoveAt(removeEdgeNumber);
            }

            return Math.Round(sum);
        }

        // Сложность за n квадрат
        static int PrimeAlhorithmOptimized(int[,] graph)
        {
            int sum = 0;
            int n = graph.GetLength(0);

            bool[] used = new bool[n];
            int[] min_e = new int[n];//INF
            for(int i = 0; i < min_e.Length; i++)
            {
                min_e[i] = int.MaxValue;
            }

            int[] sel_e = new int[n];//-1
            for (int i = 0; i < sel_e.Length; i++)
            {
                sel_e[i] = -1;
            }

            min_e[0] = 0;
            for (int i = 0; i < n; ++i)
            {
                int v = -1;
                for (int j = 0; j < n; ++j)
                    if (!used[j] && (v == -1 || min_e[j] < min_e[v]))
                        v = j;
                if (min_e[v] == int.MaxValue)
                {
                    return -1;
                }

                used[v] = true;
                if (sel_e[v] != -1)
                    sum += graph[v, sel_e[v]];

                for (int to = 0; to < n; ++to)
                    if (graph[v, to] < min_e[to])
                    {
                        min_e[to] = graph[v, to];
                        sel_e[to] = v;
                    }
            }

            return sum;
        }

        /// <summary>
        /// В данном алгоритме нет разницы между SelectedVetrexNumber и NewVetrexNumber.
        /// </summary>
        /// <param name="edges">Рёбра, отсортированные по возрастанию веса</param>
        /// <returns></returns>
        static double KruskallAlhorithm(List<Edge> edges)
        {
            List<int> addedVertices = new List<int>();
            addedVertices.Add(edges[0].NewVetrexNumber);
            addedVertices.Add(edges[0].SelectedVetrexNumber);
            double sum = edges[0].Weight;
            edges.RemoveAt(0);

            while (edges.Count > 0)
            {
                for(int i = 0; i < edges.Count; i++)
                {
                    var edge = edges[i];

                    bool containsFirstVertex = false;
                    bool containsSecondVertex = false;
                    
                    foreach (var addedVertex in addedVertices)
                    {
                        if (edge.NewVetrexNumber == addedVertex)
                        {
                            containsFirstVertex = true;

                            if (containsSecondVertex)
                            {
                                break;
                            }
                        }
                        if (edge.SelectedVetrexNumber == addedVertex)
                        {
                            containsSecondVertex = true;

                            if (containsFirstVertex)
                            {
                                break;
                            }
                        }
                    }

                    if (containsFirstVertex)
                    {
                        if (containsSecondVertex)
                        {
                            edges.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            addedVertices.Add(edge.SelectedVetrexNumber);
                            sum += edge.Weight;
                        }
                    }
                    else
                    {
                        sum += edge.Weight;
                        addedVertices.Add(edge.NewVetrexNumber);

                        if (!containsSecondVertex)
                        {
                            addedVertices.Add(edge.SelectedVetrexNumber);
                        }
                    }
                }
            }

            return sum;
        }
    }

    struct Edge
    {
        public int SelectedVetrexNumber { set; get; }
        public int NewVetrexNumber { set; get; }
        public double Weight { set; get; }
    }

//    7 11
//1 2 7
//1 4 5
//2 3 8
//2 4 9
//2 5 7
//3 5 5
//4 5 15
//4 6 6
//5 6 8
//5 7 9
//6 7 11
}
