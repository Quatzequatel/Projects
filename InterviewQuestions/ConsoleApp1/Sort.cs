using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class Sort
    {
        public enum SortType
        {
            QuickSort,
            MergeSort,
            InsertionSort,
            HeapSort,
        }
        public static void TestIt(SortType sort)
        {
            List<int> randomList = new List<int>(100);
            Random rand = new Random(5433);
            for (int i = 0; i < 100; i++)
            {
                randomList.Add(rand.Next(0, 100));
            }
            Console.WriteLine(String.Format("{0}", randomList.ToArray().Print()));
            if (sort == SortType.MergeSort) randomList.MergeSort();
            if (sort == SortType.QuickSort) randomList.QuickSort();
            if (sort == SortType.InsertionSort) randomList.InsertionSort();
            if (sort == SortType.HeapSort) randomList.HeapSort();
            Console.WriteLine(String.Format("{0}", randomList.ToArray().Print()));
        }

        public static void TestIt2(SortType sort)
        {
            int itemsCount = 10;
            List<int> list = new List<int>(itemsCount);
            Random rand = new Random(5433);
            for (int i = 0; i < itemsCount; i++)
            {
                list.Add(0);
            }
            for (int i = 0; i < itemsCount; i++)
            {
                int r = rand.Next(0, itemsCount - 1);
                while (list[r] != 0) { r = rand.Next(0, itemsCount - 1); }

                list[r] = i;
            }
            Console.WriteLine(String.Format("{0}", list.ToArray().Print()));
            if (sort == SortType.MergeSort) list.MergeSort();
            if (sort == SortType.QuickSort) list.QuickSort();
            if (sort == SortType.InsertionSort) list.InsertionSort();
            if (sort == SortType.HeapSort) list.HeapSort();
            Console.WriteLine(String.Format("{0}", list.ToArray().Print()));
        }
    }

    public static class MoreExtensions
    {
        static int callcount = 0;

        #region Bucket Sort
        public static void BucketSort<T>(this IList<T> values) where T : IComparable
        {

        }
        #endregion Bucket Sort

        #region Heap Sort
        public static void HeapSort<T>(this IList<T> values) where T : IComparable
        {
            values.buildHeap();
            for (int i = values.Count - 1; i < 1; i--)
            {
                values.Swap(0, i);
                values.heapify(0, i);
            }
        }

        private static void buildHeap<T>(this IList<T> arr) where T : IComparable
        {
            for (int i = ((arr.Count / 2) - 1); i >= 0; i--)
            {
                arr.heapify(i, arr.Count - 1);
            }
        }

        private static void heapify<T>(this IList<T> arr, int idx, int max) where T : IComparable
        {
            int largest = idx;
            int left = 2 * idx + 1;
            int right = 2 * idx + 2;

            if (left < max && arr[left].CompareTo(idx) > 0)
            {
                largest = left;
            }
            if (right < max && arr[right].CompareTo(arr[largest]) > 0)
            {
                largest = right;
            }
            if (largest != idx)
            {
                arr.Swap(idx, largest);
                arr.heapify<T>(largest, max);
            }
        }
        #endregion


        #region Insertion Sort

        public static void InsertionSort<T>(this IList<T> values) where T : IComparable
        {
            for (int i = 1; i < values.Count; i++)
            {
                values.Insert<T>(i, values[i]);
            }
        }

        private static void Insert<T>(this IList<T> values, int pos, T value) where T : IComparable
        {
            int i = pos - 1;
            while (i >= 0 && values[i].CompareTo(value) > 0)
            {
                values[i + 1] = values[i]; //shift elements greater to the right
                i--;
            }
            values[i + 1] = value;
        }

        #endregion

        #region quickSort
        /*
         Public entrypoint. input is minimimal parameters.
         */
        public static void QuickSort(this IList<int> values) //where T : IComparable
        {
            values.QuickSort(0, values.Count() - 1);
        }
        /*
         * Core quicksort method that can be recursively called.
         * split work area in half
         */
        private static void QuickSort(this IList<int> values, int low, int high) //where T : IComparable
        {
            //Console.WriteLine(String.Format("low = {0}, high = {1}", low, high));
            int i, j;
            int pivot, temp;

            if (low < high)
            {
                for (i = low, j = high, pivot = values[low]; i < j;)
                {
                    while (i < j && values[i] < pivot) { i++; }
                    while (j > i && values[j] >= pivot) { j--; }
                    if (i < j) { values.Swap(i, j); }

                }
                if (values[low] > values[j]) { values.Swap(low, j); }

                values.QuickSort(low, i - 1);
                values.QuickSort(i + 1, high);
            }
        }

        private static int Partition2(this IList<int> values, int low, int high)
        {
            int pivot = values[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (values[j] <= pivot)
                {
                    i++;
                    values.Swap(i, j);
                }
            }
            values.Swap(i+1, high);
            return i + 1;
        }

        private static int Partition(this IList<int> values, int low, int high)
        {
            int pivotIndex = low + (high - low) >> 1; //pick pivot point index
            //List<int> sample = new List<int>();
            int sampleIncrement = (high - low) >> 2; //get a sample of 4.
            int pivotValue = 0;
            if (high - low >= 2)
            {
                sampleIncrement = sampleIncrement < 1 ? 1 : sampleIncrement;
                int min = values[low];
                int max = min;
                for (int i = low; i < high; i += sampleIncrement)
                {
                    min = values[i] < min ? values[i] : min;
                    max = values[i] > max ? values[i] : max;
                }
                pivotValue = min + ((max - min) >> 1);
            }
            else
            {
                //pivotValue = values[low+1];
                if (values[low].CompareTo(values[high]) > 0)
                {
                    values.Swap(low, high);
                    low++; high--;
                }
            }


            //T pivotValue = values[pivotIndex]; //cache pivot value;

            while (low <= pivotIndex && pivotIndex <= high)
            {
                //find low item that should be in high partition.
                while (low <= pivotIndex && values[low].CompareTo(pivotValue) <= 0) { low++; }
                //find high item that should be in low partition.
                while (high >= pivotIndex && values[high].CompareTo(pivotValue) > 0) { high--; }
                if (low < high && values[low].CompareTo(values[high]) > 0)
                {
                    values.Swap(low, high);
                    low++; high--;
                }
            }

            return low + 1;
        }

        private static int GetPivotValue(this IList<int> values, int low, int high)
        {
            int min = values[low];
            int max = min;
            int sampleIncrement = (high - low) >> 2; //get a sample of 4.
            sampleIncrement = sampleIncrement < 1 ? 1 : sampleIncrement;
            for (int i = low; i < high; i += sampleIncrement)
            {
                min = values[i] < min ? values[i] : min;
                max = values[i] > max ? values[i] : max;
            }
            return min + ((max - min) >> 1);
        }
        #endregion

        #region MergeSort
        /// <summary>
        /// Source Cracking the Coding Interview. 6th edition
        /// Originally in Java translated into C# using genarics. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public static void MergeSort<T>(this IList<T> values) where T : IComparable
        {
            IList<T> helper = new List<T>(values.ToArray());
            values.MergeSort<T>(helper, 0, values.Count - 1);
        }

        private static void MergeSort<T>(this IList<T> values, IList<T> helper, int low, int high) where T : IComparable
        {
            if (low < high)
            {
                int middle = (low + high) >> 1;
                values.MergeSort(helper, low, middle); //sort top half.
                values.MergeSort(helper, middle + 1, high); //sort bottom half.
                values.Merge(helper, low, middle, high); //merge together.
            }
        }

        private static void Merge<T>(this IList<T> values, IList<T> helper, int low, int middle, int high) where T : IComparable
        {
            //copy into helper. Each time?
            for (int i = low; i <= high; i++)
            {
                helper[i] = values[i];
            }

            int hLeft = low;
            int hRight = middle + 1;
            int current = low;
            /*
             * iterate through helper array. Compare left and right half,
             * copying back the smaller element from the 2 halves into the
             * orginal array.
             */
            while (hLeft <= middle && hRight <= high)
            {
                if (helper[hLeft].CompareTo(helper[hRight]) <= 0)
                {
                    values[current] = helper[hLeft];
                    hLeft++;
                }
                else //if right elemenet is smaller than left element
                {
                    values[current] = helper[hRight];
                    hRight++;
                }
                current++;
            }
            /*Copy the rest of the left side to the original array*/
            int remaining = middle - hLeft;
            for (int i = 0; i <= remaining; i++)
            {
                values[current + i] = helper[hLeft + i];
            }
        }

        #endregion

        private static void Swap<T>(this IList<T> values, int index1, int index2) where T : IComparable
        {
            T tmp = values[index1];
            values[index1] = values[index2];
            values[index2] = tmp;
        }
    }
}
