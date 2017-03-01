using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinesePostMan
{
    class Graph
    {
        public List<Edge> Edges { get; set; }

        public Vertex[] Vertices { get; set; }

        public int[,] ArrayGraph { set; get; }

        public Graph(int verticesCount)
        {
            Vertices = new Vertex[verticesCount];
            Edges = new List<Edge>();

            for (int i = 0; i < verticesCount; i++)
            {
                Vertices[i] = new Vertex(i);
            }

            ArrayGraph = new int[verticesCount, verticesCount];
        }

        public void AddEdge(int firstNumber, int secondNumber, int weight)
        {
            var edge = new Edge(Vertices[firstNumber], Vertices[secondNumber], weight);

            Vertices[firstNumber].Edges.Add(edge);
            Vertices[secondNumber].Edges.Add(edge);

            Edges.Add(edge);

            ArrayGraph[firstNumber, secondNumber] = weight;
            ArrayGraph[secondNumber, firstNumber] = weight;
        }

        public void RemoveEdge(Edge edge)
        {
            ArrayGraph[edge.Vertex1.Number, edge.Vertex2.Number] = 0;
            ArrayGraph[edge.Vertex2.Number, edge.Vertex1.Number] = 0;

            Vertices[edge.Vertex1.Number].Edges.Remove(edge);
            Vertices[edge.Vertex2.Number].Edges.Remove(edge);

            Edges.Remove(edge);
        }
    }
}
