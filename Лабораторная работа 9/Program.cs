using System;

namespace Practice9
{
    public class Program
    {
        public static Time Average(TimeArray a)
        {
            if (a == null)
                throw new ArgumentNullException("a");
            if (a.Length == 0)
                throw new ArgumentException("Массив пуст.");

            long sum = 0;
            for (int i = 0; i < a.Length; i++)
                sum += a[i].TotalMinutes;

            long avg = (sum + a.Length / 2) / a.Length;
            return Time.FromMinutes(avg);
        }

        static void Part1()
        {
            ConsoleUI.PrintMessage("=== Часть 1 ===");
            Time t1 = new Time();
            Time t2 = new Time(2, 45);
            ConsoleUI.PrintTime("t1", t1);
            ConsoleUI.PrintTime("t2", t2);

            Time t3 = ConsoleUI.ReadTime("Введите время:");
            int m = ConsoleUI.ReadInt("Сколько минут добавить: ");

            try
            {
                Time r1 = Time.AddMinutes(t3, m);   // статическая функция
                Time r2 = t3.AddMinutes(m);         // метод класса
                ConsoleUI.PrintTime("Time.AddMinutes(t, m)", r1);
                ConsoleUI.PrintTime("t.AddMinutes(m)", r2);
            }
            catch (ArgumentException e)
            {
                ConsoleUI.PrintMessage("Ошибка: " + e.Message);
            }
            ConsoleUI.PrintCounts();
        }

        static void Part2()
        {
            ConsoleUI.PrintMessage("=== Часть 2 ===");
            Time t = new Time(1, 59);
            ConsoleUI.PrintTime("t", t);

            t++;
            ConsoleUI.PrintTime("t++", t);
            t--;
            ConsoleUI.PrintTime("t--", t);

            ConsoleUI.PrintMessage("(int)t = " + (int)t);
            bool b = t;
            ConsoleUI.PrintMessage("bool(t) = " + b);
            bool z = new Time();
            ConsoleUI.PrintMessage("bool(00:00) = " + z);

            ConsoleUI.PrintTime("t + 30", t + 30);
            ConsoleUI.PrintTime("30 + t", 30 + t);
            ConsoleUI.PrintTime("t - 30", t - 30);
            ConsoleUI.PrintTime("200 - t", 200 - t);

            try
            {
                Time bad = t - 500;
                ConsoleUI.PrintTime("t - 500", bad);
            }
            catch (ArgumentException e)
            {
                ConsoleUI.PrintMessage("t - 500: ошибка: " + e.Message);
            }
            ConsoleUI.PrintCounts();
        }

        static void Part3()
        {
            ConsoleUI.PrintMessage("=== Часть 3 ===");
            TimeArray a0 = new TimeArray();
            ConsoleUI.PrintArray("Массив без параметров:", a0);

            int n = ConsoleUI.ReadInt("Размер массива: ");
            TimeArray a1 = new TimeArray(n, new Random());
            ConsoleUI.PrintArray("Случайный массив:", a1);
            if (a1.Length > 0)
                ConsoleUI.PrintTime("Среднее арифметическое", Average(a1));

            TimeArray a2 = new TimeArray(n, i => ConsoleUI.ReadTime("Элемент [" + i + "]:"));
            ConsoleUI.PrintArray("Массив, введённый с клавиатуры:", a2);
            if (a2.Length > 0)
                ConsoleUI.PrintTime("Среднее арифметическое", Average(a2));

            try
            {
                ConsoleUI.PrintMessage("Обращение к a2[" + n + "]...");
                ConsoleUI.PrintTime("a2[n]", a2[n]);
            }
            catch (ArgumentException e)
            {
                ConsoleUI.PrintMessage("Ошибка: " + e.Message);
            }
            ConsoleUI.PrintCounts();
        }

        public static void Main(string[] args)
        {
            Part1();
            Part2();
            Part3();
        }
    }
}