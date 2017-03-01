using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinesePostMan
{
    class Edge
    {
        public Vertex Vertex1 { set; get; }

        public Vertex Vertex2 { set; get; }

        public double Weight { set; get; }

        public Edge(Vertex v1, Vertex v2, double weight)
        {
            Vertex1 = v1;
            Vertex2 = v2;

            Weight = weight;
        }
    }
}
