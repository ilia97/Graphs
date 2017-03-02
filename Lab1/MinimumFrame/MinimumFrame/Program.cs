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
            List<Edge> edges = new List<Edge>();

            for (int i = 0; i < edgesCount; i++)
            {
                var nextStr = Console.ReadLine().Split(' ');

                var firstVertexNumber = Convert.ToInt32(nextStr[0]) - 1;
                var secondVertexNumber = Convert.ToInt32(nextStr[1]) - 1;
                var edgeWeight = Convert.ToInt32(nextStr[2]);

                edges.Add(new Edge()
                {
                    NewVetrexNumber = firstVertexNumber,
                    SelectedVetrexNumber = secondVertexNumber,
                    Weight = edgeWeight
                });

                graph[firstVertexNumber, secondVertexNumber] = edgeWeight;
                graph[secondVertexNumber, firstVertexNumber] = edgeWeight;
            }

            // Console.WriteLine(PrimeAlhorithm(graph));
            Console.WriteLine(KruskallAlhorithm(edges.OrderBy(x => x.Weight).ToList()));
        }

        static double PrimeAlhorithm(int[,] graph)
        {
            var verticesCount = graph.GetLength(0);
            double sum = 0;

            var nextEdgeNumber = 0;

            List<int> selectedVertices = new List<int>();

            List<Edge> edges = new List<Edge>();
            
            while (selectedVertices.Count < verticesCount)
            {
                selectedVertices.Add(nextEdgeNumber);

                if (selectedVertices.Count == verticesCount)
                    break;

                for (int i = 0; i < graph.GetLength(1); i++)
                {
                    if (graph[nextEdgeNumber, i] > 0 && !selectedVertices.Contains(i))
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
                
                var minEdgeWeight = int.MaxValue;
                var newNextEdgeNumber = -1;
                var removeEdgeNumber = -1;

                for (int i = 0; i < edges.Count; i++)
                {
                    if(selectedVertices.Contains(edges[i].NewVetrexNumber))
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
        public int Weight { set; get; }
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
