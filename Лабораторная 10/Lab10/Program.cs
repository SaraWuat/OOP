using System;
using System.Text;
using Lab10Lib;

namespace Lab10
{
    class Program
    {
        static Random rnd = new Random();

        // Создаёт случайный объект одного из 4 типов иерархии
        static Person NewPerson(int type)
        {
            Person p;
            switch (type)
            {
                case 1: p = new Student(); break;
                case 2: p = new Employee(); break;
                case 3: p = new Teacher(); break;
                default: p = new Person(); break;
            }
            p.RandomInit();
            return p;
        }

        // Массив из n случайных объектов, первые 4 - по одному каждого типа
        static Person[] NewArray(int n)
        {
            Person[] a = new Person[n];
            for (int i = 0; i < n; i++)
                a[i] = NewPerson(i < 4 ? i : rnd.Next(4));
            return a;
        }

        // Ввод целого числа из диапазона [min; max] с повтором при ошибке
        static int ReadInt(string msg, int min, int max)
        {
            int v;
            Console.Write(msg + ": ");
            while (!int.TryParse(Console.ReadLine(), out v) || v < min || v > max)
                Console.Write("Введите ещё раз: ");
            return v;
        }

        static void ShowArray(Person[] a)
        {
            foreach (Person p in a) p.Show();
        }

        static void ShowAll(Person[] a)
        {
            foreach (Person p in a) Console.WriteLine("  " + p.Name + ", " + p.Age + " л. (" + p.GetType().Name + ")");
        }

        // Просмотр элемента типа IInit (Show есть у Person и у Group)
        static void ShowItem(IInit x)
        {
            Person p = x as Person;
            if (p != null) p.Show();
            else ((Group)x).Show();
        }

        // ---------- Часть 1 ----------
        static void Part1(Person[] a)
        {
            Console.WriteLine("--- Виртуальный Show() ---");
            ShowArray(a);

            Console.WriteLine("\n--- Невиртуальный Print() ---");
            foreach (Person p in a) p.Print();

            Console.WriteLine("\nShow() виртуальный: версия выбирается по типу объекта.");
            Console.WriteLine("Print() невиртуальный: версия выбирается по типу ссылки (Person).");

            Console.WriteLine("\n--- Equals ---");
            Student s = new Student("Азанов Матвей Александрович", true, 19, 2, 4.5);
            Student s2 = new Student(s);
            Console.WriteLine("Копия равна оригиналу: " + s.Equals(s2));
            s2.Course = 3;
            Console.WriteLine("После изменения курса копии: " + s.Equals(s2));
        }

        // ---------- Часть 2 ----------
        static void Part2(Person[] a)
        {
            ShowArray(a);

            // Запрос 1: имена студентов указанного курса
            int c = ReadInt("\nЗапрос 1. Курс (1-6)", 1, 6);
            Console.WriteLine("Студенты " + c + " курса:");
            foreach (Person p in a)
            {
                Student s = p as Student;
                if (s != null && s.Course == c) Console.WriteLine("  " + s.Name);
            }

            // Запрос 2: имена и должность преподавателей указанной кафедры
            Console.Write("\nКафедры в массиве:");
            foreach (Person p in a)
            {
                Teacher t = p as Teacher;
                if (t != null) Console.Write(" " + t.Dept.Name);
            }
            Console.WriteLine();
            string d = Input.Str("Запрос 2. Кафедра");
            Console.WriteLine("Преподаватели кафедры " + d + ":");
            foreach (Person p in a)
            {
                Teacher t = p as Teacher;
                if (t != null && t.Dept.Name == d)
                    Console.WriteLine("  " + t.Name + ", " + t.Position);
            }

            // Запрос 3: количество сотрудников (включая преподавателей) со стажем не менее заданного
            int e = ReadInt("\nЗапрос 3. Минимальный стаж (0-60)", 0, 60);
            int n = 0;
            foreach (Person p in a)
            {
                Employee emp = p as Employee;  // Teacher тоже является Employee
                if (emp != null && emp.Experience >= e) n++;
            }
            Console.WriteLine("Сотрудников со стажем не менее " + e + ": " + n);

            // Запрос 4: количество объектов каждого типа (оператор is)
            int cs = 0, ce = 0, ct = 0, cp = 0;
            foreach (Person p in a)
            {
                if (p is Teacher) ct++;
                else if (p is Employee) ce++;
                else if (p is Student) cs++;
                else cp++;
            }
            Console.WriteLine("\nЗапрос 4. Персон: " + cp + ", студентов: " + cs +
                ", сотрудников: " + ce + ", преподавателей: " + ct);
        }

        // Бинарный поиск по ФИО в массиве, отсортированном по ФИО
        static int BinSearch(Person[] a, string key)
        {
            int l = 0, r = a.Length - 1;
            while (l <= r)
            {
                int m = (l + r) / 2;
                int c = string.Compare(a[m].Name, key);
                if (c == 0) return m;
                if (c < 0) l = m + 1; else r = m - 1;
            }
            return -1;
        }

        // ---------- Часть 3: сортировка и поиск ----------
        static void Part3Sort(Person[] source)
        {
            Person[] a = (Person[])source.Clone();  // исходный массив не меняем

            Console.WriteLine("--- Сортировка по ФИО (IComparable) ---");
            Array.Sort(a);
            ShowAll(a);

            // Бинарный поиск (массив отсортирован по ФИО)
            string key = Input.Str("\nФИО для поиска (бинарный поиск)");
            int i = BinSearch(a, key);
            Console.WriteLine(i >= 0 ? "Собственный поиск: найден, индекс " + i : "Собственный поиск: не найден");
            Console.WriteLine("Array.BinarySearch: " + Array.BinarySearch(a, new Person(key, true, 1)));

            Array.Sort(a, new SortByAge());  // IComparer: по возрасту
            Console.WriteLine("\n--- Сортировка по возрасту (IComparer) ---");
            ShowAll(a);

            int age = ReadInt("\nВозраст для поиска (1-120)", 1, 120);
            int j = Array.BinarySearch(a, new Person("", true, age), new SortByAge());
            Console.WriteLine(j >= 0 ? "Найден, индекс " + j + ": " + a[j].Name : "Не найден");

            Array.Sort(a, new SortByNameDesc());
            Console.WriteLine("\n--- Сортировка по ФИО в обратном порядке (IComparer) ---");
            ShowAll(a);
        }

        // ---------- Часть 3: массив IInit ----------
        static void Part3IInit()
        {
            IInit[] arr = { new Person(), new Student(), new Employee(), new Teacher(), new Group() };

            Console.WriteLine("--- RandomInit() для всех элементов ---");
            foreach (IInit x in arr)
            {
                x.RandomInit();
                ShowItem(x);
            }

            // Ввод с клавиатуры (Init) для выбранного элемента: вызывается версия его класса
            Console.WriteLine("\n--- Init() для выбранного элемента ---");
            for (int i = 0; i < arr.Length; i++)
                Console.WriteLine(i + ": " + arr[i].GetType().Name);
            int k = ReadInt("Номер элемента", 0, arr.Length - 1);
            arr[k].Init();
            ShowItem(arr[k]);
        }

        // ---------- Часть 3: клонирование ----------
        static void Part3Clone()
        {
            Teacher t = new Teacher("Иванов Иван Петрович", true, 45, 20, "ИТАС", "к.т.н.");
            Teacher sh = (Teacher)t.ShallowCopy();
            Teacher cl = (Teacher)t.Clone();
            t.Dept.Name = "ИЗМЕНЕНА";
            Console.WriteLine("Кафедра оригинала: " + t.Dept.Name);
            Console.WriteLine("Кафедра поверхностной копии: " + sh.Dept.Name + " (общий объект Department)");
            Console.WriteLine("Кафедра клона: " + cl.Dept.Name + " (независимый объект)");
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Person[] arr = NewArray(10);  // общий массив для частей 1-3

            while (true)
            {
                Console.WriteLine("\n1 - Часть 1 (виртуальные функции, Equals)");
                Console.WriteLine("2 - Часть 2 (запросы, is / as)");
                Console.WriteLine("3 - Часть 3 (сортировка и поиск)");
                Console.WriteLine("4 - Часть 3 (массив IInit, Init)");
                Console.WriteLine("5 - Часть 3 (клонирование)");
                Console.WriteLine("6 - Создать массив заново");
                Console.WriteLine("0 - Выход");
                int k = ReadInt("Выбор", 0, 6);
                Console.WriteLine();
                switch (k)
                {
                    case 1: Part1(arr); break;
                    case 2: Part2(arr); break;
                    case 3: Part3Sort(arr); break;
                    case 4: Part3IInit(); break;
                    case 5: Part3Clone(); break;
                    case 6: arr = NewArray(10); Console.WriteLine("Массив создан заново."); break;
                    case 0: return;
                }
            }
        }
    }
}