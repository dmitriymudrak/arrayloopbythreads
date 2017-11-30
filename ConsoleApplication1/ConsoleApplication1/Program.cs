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
        private static object _lock = new object();
        
        static EventWaitHandle wh = new AutoResetEvent(false);

        static void Main(string[] args)
        {
        
            int arrayRows = 10000;
            int arrayColumns = 1000;

            var arr = InitArrayRandomNums(arrayRows, arrayColumns, 100);
            
            var chunkSize = arrayRows / Environment.ProcessorCount+1;

            var startRow = 0;
            var endRow = 0;

            var taskList = new List<Task<KeyValuePair<int, int>>>();
            var results = new List<KeyValuePair<int, int>>();

            for (var counter = 0; counter < Environment.ProcessorCount; counter++)
            {
                startRow = counter * chunkSize;
                endRow = chunkSize * (counter + 1);

                if (counter == Environment.ProcessorCount - 1)
                {
                    endRow = arr.GetUpperBound(0) + 1;
                }

                var task = TaskIterate(arr, startRow, endRow);
                lock (_lock)
                {
                    results.Add(task.Result);
                }
                
            }

            var maxKvp = results.OrderBy(x => x.Value).Last();


            Console.WriteLine("Max row {0}, max sum {1}", maxKvp.Key, maxKvp.Value);

            Console.WriteLine("Done");
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
        
        public static async Task<KeyValuePair<int,int>> TaskIterate(int[,] arr, int startIndexRow, int endIndexRow)
        {
            return await Task.Run( () => Iterate(arr, startIndexRow, endIndexRow) );
        }

        private static KeyValuePair<int,int> Iterate(int[,] arr, int start, int end)
        {
            var maxSum = 0;
            var maxRow = 0;
            for (int i = start; i < end; i++)
             {
                var rowSum = 0;
                rowSum = GetRow(arr, i).Sum();
                if (rowSum > maxSum)
                {
                    maxSum = rowSum;
                    maxRow = i;
                    
                }
            }
            Console.WriteLine("MaxRow - {0}, maxSum - {1}", maxRow, maxSum);

            return new KeyValuePair<int, int>(maxRow, maxSum);
        }
    }
}
