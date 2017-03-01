using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinesePostMan
{
    class Program
    {
        static void Main(string[] args)
        {

            // Создание графа ---------------------------------------
            Console.WriteLine("Enter the count of graph vertices:");
            int verticesCount = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the count of graph edges:");
            int edgesCount = Convert.ToInt32(Console.ReadLine());

            // Представляем граф в виде двумерного массива. 
            var graph = new Graph(verticesCount);

            for (int i = 0; i < edgesCount; i++)
            {
                Console.WriteLine("Add an edge to the graph in format \"{first vertex number} {second vertex number} {weight of the edge}\" (numeration starts with 0):");
                var newEdgeString = Console.ReadLine();

                var newEdgeArray = newEdgeString.Split(' ');

                var firstVertex = Convert.ToInt32(newEdgeArray[0]);
                var secondVertex = Convert.ToInt32(newEdgeArray[1]);
                var edgeWeight = Convert.ToInt32(newEdgeArray[2]);

                graph.AddEdge(firstVertex, secondVertex, edgeWeight);
            }
            //-----------------------------------------------------------------------------------

            // Массив, хранящий длины минимальных путей от вершины i до вершины j.
            var minDistancesMatrix = new int[verticesCount, verticesCount];
            // Массив, хранящий номер вершины, путь через которую оказался выгоднее, чем предыдущий.
            var transitionMatrix = new int[verticesCount, verticesCount];

            for (int i = 0; i < graph.ArrayGraph.GetLength(0); i++)
            {
                for (int j = 0; j < graph.ArrayGraph.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        minDistancesMatrix[i, j] = 0;
                    }
                    else if (graph.ArrayGraph[i, j] == 0)
                    {
                        minDistancesMatrix[i, j] = int.MaxValue;
                    }
                    else
                    {
                        minDistancesMatrix[i, j] = graph.ArrayGraph[i, j];
                    }

                    transitionMatrix[i, j] = -1;
                }
            }

            for (int k = 0; k < verticesCount; ++k)
            {
                for (int i = 0; i < verticesCount; ++i)
                {
                    for (int j = 0; j < verticesCount; ++j)
                    {
                        if (minDistancesMatrix[i, j] > minDistancesMatrix[i, k] + minDistancesMatrix[k, j])
                        {
                            minDistancesMatrix[i, j] = minDistancesMatrix[i, k] + minDistancesMatrix[k, j];
                            transitionMatrix[i, j] = k;
                        }
                    }
                }
            }
            // ------------------------------------------------------

            // Находим номера вершин, и, соответственно, рёбра, которые образуют минимальный путь от одной вершины к другой 
            // для всех вершин из множества вершин с нечётной степенью.

            // Количество этих вершин чётное согласно какой-то лемме 
            List<int> verticesWithOddDegree = new List<int>();

            for (int i = 0; i < verticesCount; ++i)
            {
                if (graph.Vertices[i].Edges.Count % 2 == 1)
                {
                    verticesWithOddDegree.Add(i);
                }
            }

            // TODO: Реализовать алгоритм нахождения паросочетаний минимального веса.
            // Сейчас: Берутся просто пары вершин с номерами 2*i и 2*i+1. 
            int[][] ways = new int[verticesWithOddDegree.Count / 2][];

            for (int i = 0; i < verticesWithOddDegree.Count / 2; i++)
            {
                ways[i] = new int[2] { verticesWithOddDegree[i * 2], verticesWithOddDegree[i * 2 + 1] };
            }

            // Находим, какие рёбра необходимо условно добавить к нашему графу.
            foreach (var way in ways)
            {
                int[] vertices = GetFullWay(way[0], way[1], transitionMatrix);

                for (int i = 0; i < vertices.Length - 1; i++)
                {
                    // Так как вес нам больше не важен, можем добавлять рёбра с нулевым весом.
                    graph.AddEdge(vertices[i], vertices[i + 1], 0);
                }
            }

            // Нахождение Эйлерова пути.
            Stack<Vertex> stack = new Stack<Vertex>();
            List<int> result = new List<int>();
            stack.Push(graph.Vertices[0]);

            while (stack.Count > 0)
            {
                var vertex = stack.Peek();

                if(vertex.Edges.Count == 0)
                {
                    result.Add(vertex.Number);
                    stack.Pop();
                }
                else
                {
                    var edge = vertex.Edges[0];

                    Vertex newVertex = null;

                    if (edge.Vertex1 == vertex)
                    {
                        newVertex = edge.Vertex2;
                    }
                    else
                    {
                        newVertex = edge.Vertex1;
                    }

                    graph.RemoveEdge(edge);
                    stack.Push(newVertex);
                }
            }

            // Вывод результата.
            Console.WriteLine("The result is:");
            foreach (var c in result)
            {
                Console.Write($"{c} ");
            }
        }

        static int[] GetFullWay(int VertexA, int VertexB, int[,] transitionMatrix)
        {
            if (transitionMatrix[VertexA, VertexB] == -1)
            {
                return new int[0] { };
            }
            else
            {
                int newVertex = transitionMatrix[VertexA, VertexB];

                var vertexAArray = new int[] { VertexA };
                var vertexBArray = new int[] { VertexB };
                var newVertexArray = new int[] { newVertex };

                // Соединяем вершину А, массив вершин между А и промежуточной вершиной, промежуточную вершину, массив вершин между промежуточной вершиной и В, и вершину В.
                return vertexAArray
                    .Concat(GetFullWay(VertexA, newVertex, transitionMatrix))
                    .Concat(newVertexArray)
                    .Concat(GetFullWay(newVertex, VertexB, transitionMatrix))
                    .Concat(vertexBArray)
                    .ToArray();
            }
        }

        // http://e-maxx.ru/algo/assignment_hungary
   //     static int[,] FindPerfectMatchingOfMinimalWeight(int[,] a)
   //     {
   //         List<int> u = new List<int>(n + 1), v(m + 1), p(m + 1), way(m + 1);
   //         for (int i = 1; i <= n; ++i)
   //         {
   //             p[0] = i;
   //             int j0 = 0;
   //             vector<int> minv (m + 1, INF);
   //         vector<char> used (m + 1, false);
   //         do
   //         {
   //             used[j0] = true;
   //             int i0 = p[j0], delta = INF, j1;
   //             for (int j = 1; j <= m; ++j)
   //                 if (!used[j])
   //                 {
   //                     int cur = a[i0][j] - u[i0] - v[j];
   //                     if (cur < minv[j])
   //                         minv[j] = cur,  way[j] = j0;
   //                     if (minv[j] < delta)
   //                         delta = minv[j],  j1 = j;
   //                 }
   //             for (int j = 0; j <= m; ++j)
   //                 if (used[j])
   //                     u[p[j]] += delta,  v[j] -= delta;
			//else
			//	minv[j] -= delta;
   //             j0 = j1;
   //         } while (p[j0] != 0);
   //         do
   //         {
   //             int j1 = way[j0];
   //             p[j0] = p[j1];
   //             j0 = j1;
   //         } while (j0);
   //     }
    //}
    }
}
