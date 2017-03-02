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
                        // Чтобы в дальнейшем присложении двух элементов не было нереполнения типа int.
                        minDistancesMatrix[i, j] = (int.MaxValue - 1) / 2;
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

            // Алгоритм нахождения паросочетаний минимального веса.
            // В алгоритме нумерация вершин начинается с единицы. Меняем массив так, чтобы нумерация начиналась с 1.
            var minDistancesMatrixCopy = new int[minDistancesMatrix.GetLength(0) + 1, minDistancesMatrix.GetLength(1) + 1];
            for(int i = 0; i < minDistancesMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < minDistancesMatrix.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        minDistancesMatrixCopy[i + 1, j + 1] = short.MaxValue;
                    }
                    else
                    {
                        minDistancesMatrixCopy[i + 1, j + 1] = minDistancesMatrix[i, j];
                    }
                }
            }

            int[] ways = FindPerfectMatchingOfMinimalWeight(minDistancesMatrixCopy, minDistancesMatrix.GetLength(0));

            // Находим, какие рёбра необходимо условно добавить к нашему графу.
            foreach (var oddVertex in verticesWithOddDegree)
            {
                // Так как в массиве ways нумерация вершин начинается с 1, то вначале добавляем 1, а потом вычитаем 1 из результата.
                int[] vertices = GetFullWay(oddVertex, ways[oddVertex + 1] - 1, transitionMatrix);

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

                if (vertex.Edges.Count == 0)
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
        static int[] FindPerfectMatchingOfMinimalWeight(int[,] a, int n)
        {
            int[] u = new int[n + 1];
            int[] v = new int[n + 1];
            int[] p = new int[n + 1];
            int[] way = new int[n + 1];

            for (int i = 1; i <= n; ++i)
            {
                p[0] = i;
                int j0 = 0;
                int[] minv = new int[n + 1];

                for (int j = 0; j < minv.Length; j++)
                {
                    minv[j] = int.MaxValue;
                }

                bool[] used = new bool[n + 1];
                do
                {
                    used[j0] = true;
                    int i0 = p[j0];
                    int delta = int.MaxValue;
                    int j1 = 0;
                    for (int j = 1; j <= n; ++j)
                    {
                        if (!used[j])
                        {
                            int cur = a[i0, j] - u[i0] - v[j];
                            if (cur < minv[j])
                            {
                                minv[j] = cur;
                                way[j] = j0;
                            }
                            if (minv[j] < delta)
                            {
                                delta = minv[j];
                                j1 = j;
                            }
                        }
                    }
                    for (int j = 0; j <= n; ++j)
                    {
                        if (used[j])
                        {
                            u[p[j]] += delta;
                            v[j] -= delta;
                        }
                        else
                        {
                            minv[j] -= delta;
                        }
                        j0 = j1;
                    }
                } while (p[j0] != 0);
                do
                {
                    int j1 = way[j0];
                    p[j0] = p[j1];
                    j0 = j1;
                } while (j0 > 0);
            }

            return p;
        }

        //// ----------------------------------------------------------------------------
        //public class Hungarian
        //{

        //    private int numRows;
        //    private int numCols;

        //    private bool[,] primes;
        //    private bool[,] stars;
        //    private bool[] rowsCovered;
        //    private bool[] colsCovered;
        //    private int[,] costs;

        //    public Hungarian(int[,] theCosts)
        //    {
        //        costs = theCosts;
        //        numRows = costs.GetLength(0);
        //        numCols = costs.GetLength(1);

        //        primes = new bool[numRows, numCols];
        //        stars = new bool[numRows, numCols];

        //        // Инициализация массивов с покрытием строк/столбцов
        //        rowsCovered = new bool[numRows];
        //        colsCovered = new bool[numCols];
        //        for (int i = 0; i < numRows; i++)
        //        {
        //            rowsCovered[i] = false;
        //        }
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            colsCovered[j] = false;
        //        }
        //        // Инициализация матриц
        //        for (int i = 0; i < numRows; i++)
        //        {
        //            for (int j = 0; j < numCols; j++)
        //            {
        //                primes[i, j] = false;
        //                stars[i, j] = false;
        //            }
        //        }
        //    }

        //    public int[,] execute()
        //    {
        //        subtractRowColMins();

        //        this.findStars(); // O(n^2)
        //        this.resetCovered(); // O(n);
        //        this.coverStarredZeroCols(); // O(n^2)

        //        while (!allColsCovered())
        //        {
        //            int[] primedLocation = this.primeUncoveredZero(); // O(n^2)

        //            // It's possible that we couldn't find a zero to prime, so we have to induce some zeros so we can find one to prime
        //            if (primedLocation[0] == -1)
        //            {
        //                this.minUncoveredRowsCols(); // O(n^2)
        //                primedLocation = this.primeUncoveredZero(); // O(n^2)
        //            }

        //            // is there a starred 0 in the primed zeros row?
        //            int primedRow = primedLocation[0];
        //            int starCol = this.findStarColInRow(primedRow);
        //            if (starCol != -1)
        //            {
        //                // cover ther row of the primedLocation and uncover the star column
        //                rowsCovered[primedRow] = true;
        //                colsCovered[starCol] = false;
        //            }
        //            else
        //            { // otherwise we need to find an augmenting path and start over.
        //                this.augmentPathStartingAtPrime(primedLocation);
        //                this.resetCovered();
        //                this.resetPrimes();
        //                this.coverStarredZeroCols();
        //            }
        //        }

        //        return this.starsToAssignments(); // O(n^2)

        //    }

        //    /*
        //     * the starred 0's in each column are the assignments.
        //     * O(n^2)
        //     */
        //    public int[,] starsToAssignments()
        //    {
        //        int[,] toRet = new int[numCols, 2];
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            toRet[j, 0] = this.findStarRowInCol(j);
        //            toRet[j, 1] = j;
        //            // O(n)
        //        }
        //        return toRet;
        //    }

        //    /*
        //     * resets prime information
        //     */
        //    public void resetPrimes()
        //    {
        //        for (int i = 0; i < numRows; i++)
        //        {
        //            for (int j = 0; j < numCols; j++)
        //            {
        //                primes[i, j] = false;
        //            }
        //        }
        //    }


        //    /*
        //     * resets covered information, O(n)
        //     */
        //    public void resetCovered()
        //    {
        //        for (int i = 0; i < numRows; i++)
        //        {
        //            rowsCovered[i] = false;
        //        }
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            colsCovered[j] = false;
        //        }
        //    }

        //    /*
        //     * get the first zero in each column, star it if there isn't already a star in that row
        //     * cover the row and column of the star made, and continue to the next column
        //     * O(n^2)
        //     */
        //    public void findStars()
        //    {
        //        bool[] rowStars = new bool[numRows];
        //        bool[] colStars = new bool[numCols];

        //        for (int i = 0; i < numRows; i++)
        //        {
        //            rowStars[i] = false;
        //        }
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            colStars[j] = false;
        //        }

        //        for (int j = 0; j < numCols; j++)
        //        {
        //            for (int i = 0; i < numRows; i++)
        //            {
        //                if (costs[i, j] == 0 && !rowStars[i] && !colStars[j])
        //                {
        //                    stars[i, j] = true;
        //                    rowStars[i] = true;
        //                    colStars[j] = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    /*
        //     * Finds the minimum uncovered value, and adds it to all the covered rows then
        //     * subtracts it from all the uncovered columns. This results in a cost matrix with
        //     * at least one more zero.
        //     */
        //    private void minUncoveredRowsCols()
        //    {
        //        // find min uncovered value
        //        int minUncovered = int.MaxValue;
        //        for (int i = 0; i < numRows; i++)
        //        {
        //            if (!rowsCovered[i])
        //            {
        //                for (int j = 0; j < numCols; j++)
        //                {
        //                    if (!colsCovered[j])
        //                    {
        //                        if (costs[i, j] < minUncovered)
        //                        {
        //                            minUncovered = costs[i, j];
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // add that value to all the COVERED rows.
        //        for (int i = 0; i < numRows; i++)
        //        {
        //            if (rowsCovered[i])
        //            {
        //                for (int j = 0; j < numCols; j++)
        //                {
        //                    costs[i, j] = costs[i, j] + minUncovered;

        //                }
        //            }
        //        }

        //        // subtract that value from all the UNcovered columns
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            if (!colsCovered[j])
        //            {
        //                for (int i = 0; i < numRows; i++)
        //                {
        //                    costs[i, j] = costs[i, j] - minUncovered;
        //                }
        //            }
        //        }
        //    }

        //    /*
        //     * Finds an uncovered zero, primes it, and returns an array
        //     * describing the row and column of the newly primed zero.
        //     * If no uncovered zero could be found, returns -1 in the indices.
        //     * O(n^2)
        //     */
        //    private int[] primeUncoveredZero()
        //    {
        //        int[] location = new int[2];

        //        for (int i = 0; i < numRows; i++)
        //        {
        //            if (!rowsCovered[i])
        //            {
        //                for (int j = 0; j < numCols; j++)
        //                {
        //                    if (!colsCovered[j])
        //                    {
        //                        if (costs[i, j] == 0)
        //                        {
        //                            primes[i, j] = true;
        //                            location[0] = i;
        //                            location[1] = j;
        //                            return location;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        location[0] = -1;
        //        location[1] = -1;
        //        return location;
        //    }

        //    /*
        //     * Starting at a given primed location[0=row,1=col], we find an augmenting path
        //     * consisting of a primed , starred , primed , ..., primed. (note that it begins and ends with a prime)
        //     * We do this by starting at the location, going to a starred zero in the same column, then going to a primed zero in
        //     * the same row, etc, until we get to a prime with no star in the column.
        //     * O(n^2)
        //     */
        //    private void augmentPathStartingAtPrime(int[] location)
        //    {
        //        // Make the arraylists sufficiently large to begin with
        //        List<int[]> primeLocations = new List<int[]>(numRows + numCols);
        //        List<int[]> starLocations = new List<int[]>(numRows + numCols);
        //        primeLocations.Add(location);

        //        int currentRow = location[0];
        //        int currentCol = location[1];
        //        while (true)
        //        { // add stars and primes in pairs
        //            int starRow = findStarRowInCol(currentCol);
        //            // at some point we won't be able to find a star. if this is the case, break.
        //            if (starRow == -1)
        //            {
        //                break;
        //            }
        //            int[] starLocation = new int[] {
        //        starRow, currentCol
        //    };
        //            starLocations.Add(starLocation);
        //            currentRow = starRow;

        //            int primeCol = findPrimeColInRow(currentRow);
        //            int[] primeLocation = new int[] {
        //        currentRow, primeCol
        //    };
        //            primeLocations.Add(primeLocation);
        //            currentCol = primeCol;
        //        }

        //        unStarLocations(starLocations);
        //        this.starLocations(primeLocations);
        //    }


        //    /*
        //     * Given an arraylist of  locations, star them
        //     */
        //    private void starLocations(List<int[]> locations)
        //    {
        //        for (int k = 0; k < locations.Count; k++)
        //        {
        //            int[] location = locations[k];
        //            int row = location[0];
        //            int col = location[1];
        //            stars[row, col] = true;
        //        }
        //    }

        //    /*
        //     * Given an arraylist of starred locations, unstar them
        //     */
        //    private void unStarLocations(List<int[]> starLocations)
        //    {
        //        for (int k = 0; k < starLocations.Count; k++)
        //        {
        //            int[] starLocation = starLocations[k];
        //            int row = starLocation[0];
        //            int col = starLocation[1];
        //            stars[row, col] = false;
        //        }
        //    }


        //    /*
        //     * Given a row index, finds a column with a prime. returns -1 if this isn't possible.
        //     */
        //    private int findPrimeColInRow(int theRow)
        //    {
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            if (primes[theRow, j])
        //            {
        //                return j;
        //            }
        //        }
        //        return -1;
        //    }




        //    /*
        //     * Given a column index, finds a row with a star. returns -1 if this isn't possible.
        //     */
        //    public int findStarRowInCol(int theCol)
        //    {
        //        for (int i = 0; i < numRows; i++)
        //        {
        //            if (stars[i, theCol])
        //            {
        //                return i;
        //            }
        //        }
        //        return -1;
        //    }


        //    public int findStarColInRow(int theRow)
        //    {
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            if (stars[theRow, j])
        //            {
        //                return j;
        //            }
        //        }
        //        return -1;
        //    }

        //    // looks at the colsCovered array, and returns true if all entries are true, false otherwise
        //    private bool allColsCovered()
        //    {
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            if (!colsCovered[j])
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }

        //    /*
        //     * sets the columns covered if they contain starred zeros
        //     * O(n^2)
        //     */
        //    private void coverStarredZeroCols()
        //    {
        //        for (int j = 0; j < numCols; j++)
        //        {
        //            colsCovered[j] = false;
        //            for (int i = 0; i < numRows; i++)
        //            {
        //                if (stars[i, j])
        //                {
        //                    colsCovered[j] = true;
        //                    break; // break inner loop to save a bit of time
        //                }
        //            }
        //        }
        //    }

        //    private void subtractRowColMins()
        //    {
        //        for (int i = 0; i < numRows; i++)
        //        { //for each row
        //            int rowMin = int.MaxValue;
        //            for (int j = 0; j < numCols; j++)
        //            { // grab the smallest element in that row
        //                if (costs[i, j] < rowMin)
        //                {
        //                    rowMin = costs[i, j];
        //                }
        //            }
        //            for (int j = 0; j < numCols; j++)
        //            { // subtract that from each element
        //                costs[i, j] = costs[i, j] - rowMin;
        //            }
        //        }

        //        for (int j = 0; j < numCols; j++)
        //        { // for each col
        //            int colMin = int.MaxValue;
        //            for (int i = 0; i < numRows; i++)
        //            { // grab the smallest element in that column
        //                if (costs[i, j] < colMin)
        //                {
        //                    colMin = costs[i, j];
        //                }
        //            }
        //            for (int i = 0; i < numRows; i++)
        //            { // subtract that from each element
        //                costs[i, j] = costs[i, j] - colMin;
        //            }
        //        }
        //    }

        //}
        // ----------------------------------------------------------------------------
    }
}
