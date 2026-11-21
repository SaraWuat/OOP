using System.Diagnostics;
using Lab10Lib;

namespace Lab11
{
    // Задание 3. Сравнение времени поиска в коллекциях List<T> и Dictionary<K,T>
    public class TestCollections
    {
        private List<Student> col1 = new List<Student>();
        private List<string> col2 = new List<string>();
        private Dictionary<Person, Student> col3 = new Dictionary<Person, Student>();
        private Dictionary<string, Student> col4 = new Dictionary<string, Student>();

        public int Count { get { return col1.Count; } }
        public Student First { get { return col1[0]; } }
        public Student Central { get { return col1[col1.Count / 2]; } }
        public Student Last { get { return col1[col1.Count - 1]; } }

        // Количество элементов в каждой из четырёх коллекций
        public int CountList { get { return col1.Count; } }
        public int CountStringList { get { return col2.Count; } }
        public int CountDict { get { return col3.Count; } }
        public int CountStringDict { get { return col4.Count; } }

        // Методы поиска (именно они используются при измерении времени)
        public bool ListContains(Student s) { return col1.Contains(s); }
        public bool StringListContains(string s) { return col2.Contains(s); }
        public bool DictContainsKey(Person key) { return col3.ContainsKey(key); }
        public bool StringDictContainsKey(string key) { return col4.ContainsKey(key); }
        public bool DictContainsValue(Student s) { return col3.ContainsValue(s); }

        // Создаёт коллекции с заданным числом одинаковых элементов
        public TestCollections(int count)
        {
            while (col1.Count < count)
            {
                Student s = new Student();
                s.RandomInit();     // элемент формируется датчиком случайных чисел
                Add(s);             // повторяющиеся элементы (совпали ФИО, пол и возраст) пропускаются
            }
        }

        // Добавление одного и того же элемента во все четыре коллекции
        public bool Add(Student s)
        {
            Person key = s.BasePerson;              // подобъект базового класса
            if (col3.ContainsKey(key)) return false; // ключи словарей должны быть уникальны
            col1.Add(s);
            col2.Add(s.ToString());
            col3.Add(key, s);
            col4.Add(key.ToString(), s);
            return true;
        }

        // Удаление элемента из всех четырёх коллекций
        public bool Remove(Student s)
        {
            Person key = s.BasePerson;
            if (!col3.ContainsKey(key)) return false;
            col1.Remove(s);
            col2.Remove(s.ToString());
            col3.Remove(key);
            col4.Remove(key.ToString());
            return true;
        }

        public Student FindByName(string name)
        {
            foreach (Student s in col1)
                if (s.Name == name) return s;
            return null;
        }

        public void PrintCounts()
        {
            Console.WriteLine("List<Student>: " + col1.Count + ", List<string>: " + col2.Count +
                ", Dictionary<Person,Student>: " + col3.Count + ", Dictionary<string,Student>: " + col4.Count);
        }

        // Одиночный поиск занимает доли микросекунды, поэтому замер делается на серии повторов,
        // а результат - среднее время одного поиска в тиках Stopwatch
        private const int Repeats = 200;

        private static long Measure(Func<bool> search, out bool found)
        {
            found = false;
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < Repeats; i++)
                found = search();
            sw.Stop();
            return sw.ElapsedTicks / Repeats;
        }

        private string[] RunOnce(Student original, bool fill)
        {
            // Ищем содержимое, а не ссылку: создаём копию элемента (конструктор копирования)
            Student probe = new Student(original);
            Person probeKey = probe.BasePerson;
            string probeStr = probe.ToString();
            string probeKeyStr = probeKey.ToString();

            long[] ticks = new long[5];
            bool[] found = new bool[5];
            ticks[0] = Measure(() => ListContains(probe), out found[0]);
            ticks[1] = Measure(() => StringListContains(probeStr), out found[1]);
            ticks[2] = Measure(() => DictContainsKey(probeKey), out found[2]);
            ticks[3] = Measure(() => StringDictContainsKey(probeKeyStr), out found[3]);
            ticks[4] = Measure(() => DictContainsValue(probe), out found[4]);

            string[] cells = new string[5];
            if (fill)
                for (int i = 0; i < 5; i++)
                    cells[i] = ticks[i] + " (" + (found[i] ? "найден" : "нет") + ")";
            return cells;
        }

        public void RunSearchTest()
        {
            // Элемент, которого нет в коллекциях
            Student absent = new Student("Отсутствующий Студент Студентович", true, 99, 1, 0);
            Student[] items = { First, Central, Last, absent };
            string[] titles = { "Первый", "Центральный", "Последний", "Не входит в коллекцию" };

            // Прогревочные запуски: исключаем влияние JIT-компиляции на первые замеры
            for (int w = 0; w < 3; w++)
                foreach (Student s in items) RunOnce(s, false);

            string[] heads =
            {
                "List<Student>.Contains", "List<string>.Contains", "Dict<Person,Student>.ContainsKey",
                "Dict<string,Student>.ContainsKey", "Dict<Person,Student>.ContainsValue"
            };
            Console.WriteLine("Элементов в коллекциях: " + Count + ". Частота Stopwatch: " + Stopwatch.Frequency + " тиков/с.");
            Console.WriteLine("Время поиска, тики (в скобках - результат поиска):");
            for (int k = 0; k < items.Length; k++)
            {
                string[] cells = RunOnce(items[k], true);
                Console.WriteLine("Элемент: " + titles[k]);
                for (int i = 0; i < 5; i++)
                    Console.WriteLine("  " + heads[i].PadRight(36) + cells[i]);
            }
        }
    }
}
