using System;
using System.Collections.Generic;
using System.Linq;

namespace MinimumFrame
{
    class Program
    {
        static void Main(string[] args)
        {
            PrimAlhorithmOptimized();
            // Console.WriteLine(PrimeAlhorithm(graph));
            // Console.WriteLine(KruskallAlhorithm(edges.OrderBy(x => x.Weight).ToList(), verticesCount));
            // KruskallAlhorithmOptimized();
        }

        // Сложность за n * m
        static double PrimeAlhorithm(int[,] graph)
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
                    if (selectedVertices[edges[i].NewVetrexNumber])
                    {
                        edges.RemoveAt(i);
                        i--;
                        continue;
                    }

                    if (edges[i].Weight < minEdgeWeight)
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
        static void PrimAlhorithmOptimized()
        {
            string[] firstStr = Console.ReadLine().Split(' ');

            var verticesCount = Convert.ToInt32(firstStr[0]);
            var edgesCount = Convert.ToInt32(firstStr[1]);

            List<KeyValuePair<int, int>>[] edges = new List<KeyValuePair<int, int>>[verticesCount];
            for(int i = 0; i < verticesCount; i++)
            {
                edges[i] = new List<KeyValuePair<int, int>>();
            }

            for (int i = 0; i < edgesCount; i++)
            {
                var nextStr = Console.ReadLine().Split(' ');

                var firstVertexNumber = Convert.ToInt32(nextStr[0]) - 1;
                var secondVertexNumber = Convert.ToInt32(nextStr[1]) - 1;
                int edgeWeight = Convert.ToInt32(nextStr[2]);

                edges[firstVertexNumber].Add(new KeyValuePair<int, int>(secondVertexNumber, edgeWeight));
                edges[secondVertexNumber].Add(new KeyValuePair<int, int>(firstVertexNumber, edgeWeight));
            }

            double sum = 0;
            int[] minEdges = new int[verticesCount];

            for (int i = 0; i < minEdges.Length; i++)
            {
                minEdges[i] = int.MaxValue;
            }

            bool[] isUsed = new bool[verticesCount];
            isUsed[0] = true;
            
            SortedSet<Edge> minLengthsOrderedByLength = new SortedSet<Edge>();
            minLengthsOrderedByLength.Add(new Edge());

            for (int i = 0; i < verticesCount; ++i)
            {
                int vertexNumber = minLengthsOrderedByLength.First().NewVetrexNumber;
                double weight = minLengthsOrderedByLength.First().Weight;
                minLengthsOrderedByLength.Remove(minLengthsOrderedByLength.First());

                isUsed[vertexNumber] = true;
                sum += weight;

                for (int j = 0; j < edges[vertexNumber].Count; ++j)
                {
                    if (!isUsed[edges[vertexNumber][j].Key])
                    {
                        int to = edges[vertexNumber][j].Key;
                        int cost = edges[vertexNumber][j].Value;
                        if (cost < minEdges[to])
                        {
                            minLengthsOrderedByLength.Remove(new Edge() { NewVetrexNumber = to, Weight = minEdges[to] });
                            minEdges[to] = cost;
                            minLengthsOrderedByLength.Add(new Edge() { NewVetrexNumber = to, Weight = minEdges[to] });
                        }
                    }
                }
            }

            Console.WriteLine(sum);
        }

        /// <summary>
        /// В данном алгоритме нет разницы между SelectedVetrexNumber и NewVetrexNumber.
        /// </summary>
        /// <param name="edges">Рёбра, отсортированные по возрастанию веса</param>
        /// <param name="verticesCount">Количество вершин в графе</param>
        /// <returns></returns>
        static double KruskallAlhorithm(List<Edge> edges, int verticesCount)
        {
            // Кажому элементу массива a[i] соответствует айдишник дерева, которому принадлежит вершина под номером i.
            int[] treeIds = new int[verticesCount];
            // Количество деревьев.
            int treesCount = 0;
            // Сумма рёбер.
            double sum = 0;

            for (int i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];

                // Получаем айдишники деревьев, которым принадлежат вершины.
                int firstVertexTreeId = treeIds[edge.NewVetrexNumber];
                int secondVertexTreeId = treeIds[edge.SelectedVetrexNumber];

                if (firstVertexTreeId == 0)
                {
                    if (secondVertexTreeId == 0)
                    {
                        // Если обе вершины не принадлежат деревьям, создаём новое дерево.
                        treesCount++;
                        treeIds[edge.NewVetrexNumber] = treesCount;
                        treeIds[edge.SelectedVetrexNumber] = treesCount;
                    }
                    else
                    {
                        // Если одна из вершин принадлежит дереву, то просто добавляем вторую вершину к дереву.
                        treeIds[edge.NewVetrexNumber] = treeIds[edge.SelectedVetrexNumber];
                    }

                    sum += edge.Weight;
                }
                else
                {
                    if (secondVertexTreeId == 0)
                    {
                        // Если одна из вершин принадлежит дереву, то просто добавляем вторую вершину к дереву.
                        treeIds[edge.SelectedVetrexNumber] = treeIds[edge.NewVetrexNumber];
                        sum += edge.Weight;
                    }
                    else if (firstVertexTreeId != secondVertexTreeId)
                    {
                        // Если вершины принадлежат разным деревьям, объединяем деревья.
                        for (int j = 0; j < treeIds.Length; j++)
                        {
                            if (treeIds[j] == secondVertexTreeId)
                            {
                                treeIds[j] = firstVertexTreeId;
                            }
                        }

                        sum += edge.Weight;
                    }
                }
            }

            return sum;
        }

        static void KruskallAlhorithmOptimized()
        {
            string[] firstStr = Console.ReadLine().Split(' ');

            var verticesCount = Convert.ToInt32(firstStr[0]);
            var edgesCount = Convert.ToInt32(firstStr[1]);

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
            }

            // Кажому элементу массива a[i] соответствует номер родительской вершины, которому принадлежит вершина под номером i.
            int[] parents = new int[verticesCount];
            for (int i = 0; i < parents.Length; i++)
            {
                parents[i] = i;
            }
            // Сумма рёбер.
            double sum = 0;

            for (int i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];

                // Получаем айдишники деревьев, которым принадлежат вершины.
                int firstVertexTreeId = parents[edge.NewVetrexNumber];
                int secondVertexTreeId = parents[edge.SelectedVetrexNumber];

                firstVertexTreeId = FindSet(parents, firstVertexTreeId);
                secondVertexTreeId = FindSet(parents, secondVertexTreeId);


                if (firstVertexTreeId != secondVertexTreeId)
                {
                    // Если вершины принадлежат разным деревьям, объединяем деревья.
                    parents[firstVertexTreeId] = secondVertexTreeId;

                    sum += edge.Weight;
                }
            }

            Console.WriteLine(sum);
        }

        public static int FindSet(int[] parents, int v)
        {
            if (parents[v] == v)
                return v;
            return parents[v] = FindSet(parents, parents[v]);
        }
    }

    struct Edge : IComparable
    {
        public int SelectedVetrexNumber { set; get; }
        public int NewVetrexNumber { set; get; }
        public int Weight { set; get; }

        public int CompareTo(object edge)
        {
            if (this.Weight > ((Edge)edge).Weight)
                return 1;
            else if (this.Weight == ((Edge)edge).Weight)
            {
                return this.NewVetrexNumber - ((Edge)edge).NewVetrexNumber;
            }
            else
                return -1;
        }
    }

    /*
7 11
1 2 7
1 4 5
2 3 8
2 4 9
2 5 7
3 5 5
4 5 15
4 6 6
5 6 8
5 7 9
6 7 11
*/
}
