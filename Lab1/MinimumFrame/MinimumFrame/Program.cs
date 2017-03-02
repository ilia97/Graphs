using System;
using System.Collections.Generic;

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

            for (int i = 0; i < edgesCount; i++)
            {
                var nextStr = Console.ReadLine().Split(' ');

                var firstVertexNumber = Convert.ToInt32(nextStr[0]) - 1;
                var secondVertexNumber = Convert.ToInt32(nextStr[1]) - 1;
                var edgeWeight = Convert.ToInt32(nextStr[2]);

                graph[firstVertexNumber, secondVertexNumber] = edgeWeight;
                graph[secondVertexNumber, firstVertexNumber] = edgeWeight;
            }

            Console.WriteLine(PrimeAlhorithm(graph));
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
                    if(edges[i].Weight < minEdgeWeight)
                    {
                        newNextEdgeNumber = edges[i].NewVetrexNumber;
                        minEdgeWeight = edges[i].Weight;
                        removeEdgeNumber = i;
                    }
                }

                sum += minEdgeWeight;

                nextEdgeNumber = newNextEdgeNumber;
                edges.Remove(edges[removeEdgeNumber]);
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
}
