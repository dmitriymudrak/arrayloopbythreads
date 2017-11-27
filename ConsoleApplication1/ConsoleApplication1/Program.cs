using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxSum = 0;
            int maxRow = 0;

            int arrayRows = 10000;
            int arrayColumns = 1000;

            var arr = InitArrayRandomNums(arrayRows, arrayColumns, 100);
            
            var chunkSize = arrayRows / Environment.ProcessorCount+1;

            var startRow = 0;
            var endRow = 0;

            var threadList = new List<Thread>();

            for(var counter = 0; counter < Environment.ProcessorCount; counter++)
            {    
                startRow = counter * chunkSize;
                endRow = chunkSize * (counter + 1);

                if (counter == Environment.ProcessorCount - 1)
                {
                    endRow = arr.GetUpperBound(0) + 1;
                }

                Console.WriteLine("Counter {0}, processors {1}", counter, Environment.ProcessorCount);
                Console.WriteLine("Array range {0} - {1}", startRow, endRow);

                threadList.Add(new Thread(() =>
                {
                    for (int i = startRow; i < endRow; i++)
                    {
                        var rowSum = 0;
                        rowSum = GetRow(arr, i).Sum();
                        if (rowSum > maxSum)
                        {
                            maxSum = rowSum;
                            maxRow = i;
                        }
                    }
                }));
            }

            foreach (var thread in threadList) thread.Start();
            foreach (var thread in threadList) thread.Join();

            Console.WriteLine("MaxRow - {0}, maxSum - {1}", maxRow, maxSum);
            Console.ReadLine();
        }

        public static int[,] InitArrayRandomNums(int rows, int cols, int maxRandValue)
        {
            int[,] arr = new int[rows, cols];

            var rand = new Random();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    arr[i, j] = rand.Next(maxRandValue);
                }
            }

            return arr;
        }
        
        public static IEnumerable<int> GetRow(int[,] arr, int row)
        {
            for(var i = 0; i <= arr.GetUpperBound(1); i++)
            {
                yield return arr[row, i];
            }
        }

    }
}
