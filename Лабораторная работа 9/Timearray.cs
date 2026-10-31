using System;
using System.Text;

namespace Practice9
{
    public class TimeArray
    {
        private Time[] arr;
        private static int count = 0;

        public static int Count
        {
            get { return count; }
        }

        public int Length
        {
            get { return arr.Length; }
        }

        public TimeArray()
        {
            arr = new Time[0];
            count++;
        }

        public TimeArray(int size, Random rnd)
        {
            if (size < 0)
                throw new ArgumentException("Размер массива не может быть отрицательным.");
            if (rnd == null)
                throw new ArgumentNullException("rnd");
            arr = new Time[size];
            for (int i = 0; i < size; i++)
                arr[i] = new Time(rnd.Next(0, 24), rnd.Next(0, 60));
            count++;
        }

        public TimeArray(int size, Func<int, Time> reader)
        {
            if (size < 0)
                throw new ArgumentException("Размер массива не может быть отрицательным.");
            if (reader == null)
                throw new ArgumentNullException("reader");
            arr = new Time[size];
            for (int i = 0; i < size; i++)
                arr[i] = reader(i);
            count++;
        }

        public Time this[int index]
        {
            get
            {
                CheckIndex(index);
                return arr[index];
            }
            set
            {
                CheckIndex(index);
                if (value == null)
                    throw new ArgumentException("Элемент не может быть null.");
                arr[index] = value;
            }
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= arr.Length)
                throw new ArgumentException("Индекс выходит за пределы массива.");
        }

        public string Show()
        {
            if (arr.Length == 0)
                return "(массив пуст)";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
                sb.AppendLine(string.Format("[{0}] = {1}", i, arr[i]));
            return sb.ToString();
        }
    }
}