using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinesePostMan
{
    class Vertex
    {
        public int Number { set; get; }

        public List<Edge> Edges { set; get; }

        public Vertex(int number)
        {
            Number = number;

            Edges = new List<Edge>();
        }
    }
}
