using System;

namespace Practice9
{
    public static class ConsoleUI
    {
        public static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                int value;
                if (int.TryParse(Console.ReadLine(), out value))
                    return value;
                Console.WriteLine("Ошибка: нужно ввести целое число.");
            }
        }

        public static Time ReadTime(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                int h = ReadInt("  часы: ");
                int m = ReadInt("  минуты: ");
                try
                {
                    return new Time(h, m);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Ошибка: " + e.Message);
                }
            }
        }

        public static void PrintTime(string label, Time t)
        {
            Console.WriteLine("{0}: {1} (часы = {2}, минуты = {3})", label, t, t.Hours, t.Minutes);
        }

        public static void PrintMessage(string text)
        {
            Console.WriteLine(text);
        }

        public static void PrintArray(string label, TimeArray a)
        {
            Console.WriteLine(label);
            Console.Write(a.Show());
        }

        public static void PrintCounts()
        {
            Console.WriteLine("Создано объектов Time: " + Time.Count);
            Console.WriteLine("Создано объектов TimeArray: " + TimeArray.Count);
        }
    }
}