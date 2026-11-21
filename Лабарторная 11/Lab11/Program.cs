using System.Text;
using Lab10Lib;

namespace Lab11
{
    class Program
    {
        static void Task3(TestCollections tc)
        {
            while (true)
            {
                Console.WriteLine("\n=== Задание 3. Время поиска в коллекциях ===");
                Console.WriteLine("1 - Измерить время поиска");
                Console.WriteLine("2 - Добавить студента");
                Console.WriteLine("3 - Удалить студента по ФИО");
                Console.WriteLine("4 - Показать количество элементов в коллекциях");
                Console.WriteLine("0 - Назад");
                switch (Helper.ReadInt("Выбор", 0, 4))
                {
                    case 1:
                        tc.RunSearchTest();
                        break;
                    case 2:
                    {
                        Student s = new Student();
                        Console.WriteLine("1 - ввод с клавиатуры, 2 - случайные данные");
                        if (Helper.ReadInt("Выбор", 1, 2) == 1) s.Init(); else s.RandomInit();
                        Console.WriteLine(tc.Add(s) ? "Студент добавлен во все коллекции: " + s : "Такой студент уже есть.");
                        break;
                    }
                    case 3:
                    {
                        Student s = tc.FindByName(Input.Str("ФИО"));
                        Console.WriteLine(s != null && tc.Remove(s) ? "Студент удалён из всех коллекций." : "Студент не найден.");
                        break;
                    }
                    case 4:
                        tc.PrintCounts();
                        break;
                    case 0:
                        return;
                }
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Task1Queue task1 = new Task1Queue();
            Task2Dictionary task2 = new Task2Dictionary();
            TestCollections task3 = new TestCollections(1000);

            try
            {
                while (true)
                {
                    Console.WriteLine("\n=== Практическая работа №11. Коллекции (вариант 1) ===");
                    Console.WriteLine("1 - Задание 1 (Queue)");
                    Console.WriteLine("2 - Задание 2 (Dictionary<K,T>)");
                    Console.WriteLine("3 - Задание 3 (List<T> и Dictionary<K,T>)");
                    Console.WriteLine("0 - Выход");
                    switch (Helper.ReadInt("Выбор", 0, 3))
                    {
                        case 1: task1.Run(); break;
                        case 2: task2.Run(); break;
                        case 3: Task3(task3); break;
                        case 0: return;
                    }
                }
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("\nВвод завершён.");
            }
        }
    }
}
