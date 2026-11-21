using Lab10Lib;

namespace Lab11
{
    // Задание 2. Обобщённая коллекция Dictionary<K,T>: ключ - номер, значение - объект иерархии
    public class Task2Dictionary
    {
        private Dictionary<int, Person> dict = new Dictionary<int, Person>();
        private int nextKey = 1;

        public Task2Dictionary()
        {
            foreach (Person p in Helper.StartSet())
                dict.Add(nextKey++, p);
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\n=== Задание 2. Dictionary<int, Person> ===");
                Console.WriteLine("1 - Добавить объект");
                Console.WriteLine("2 - Удалить объект по ключу");
                Console.WriteLine("3 - Показать словарь (foreach)");
                Console.WriteLine("4 - Запросы");
                Console.WriteLine("5 - Клонировать словарь");
                Console.WriteLine("6 - Отсортировать");
                Console.WriteLine("7 - Найти объект");
                Console.WriteLine("0 - Назад");
                switch (Helper.ReadInt("Выбор", 0, 7))
                {
                    case 1: Add(); break;
                    case 2: Remove(); break;
                    case 3: Print(dict, "Словарь"); break;
                    case 4: Queries(); break;
                    case 5: CloneDemo(); break;
                    case 6: Sort(); break;
                    case 7: Find(); break;
                    case 0: return;
                }
            }
        }

        private void Add()
        {
            dict.Add(nextKey, Helper.ReadPerson());
            Console.WriteLine("Объект добавлен с ключом " + nextKey + ".");
            nextKey++;
        }

        private void Remove()
        {
            int key = Helper.ReadInt("Ключ", 1, int.MaxValue);
            Person removed;
            if (dict.TryGetValue(key, out removed) && dict.Remove(key))
                Console.WriteLine("Удалён объект: " + removed);
            else
                Console.WriteLine("Элемента с таким ключом нет.");
        }

        private static void Print(Dictionary<int, Person> d, string title)
        {
            Console.WriteLine(title + " (элементов: " + d.Count + "):");
            if (d.Count == 0) { Console.WriteLine("  пусто"); return; }
            foreach (KeyValuePair<int, Person> pair in d)
                Console.WriteLine("  [" + pair.Key + "] " + pair.Value);
        }

        private void Queries()
        {
            Console.WriteLine("1 - Количество объектов заданного вида");
            Console.WriteLine("2 - Печать объектов заданного вида");
            Console.WriteLine("3 - Имена студентов заданного курса");
            switch (Helper.ReadInt("Выбор", 1, 3))
            {
                case 1:
                {
                    Type t = Helper.ReadType();
                    int count = 0;
                    foreach (Person p in dict.Values)
                        if (p.GetType() == t) count++;
                    Console.WriteLine("Количество объектов вида «" + Helper.TypeTitle(t) + "»: " + count);
                    break;
                }
                case 2:
                {
                    Type t = Helper.ReadType();
                    bool any = false;
                    foreach (KeyValuePair<int, Person> pair in dict)
                        if (pair.Value.GetType() == t)
                        {
                            Console.Write("[" + pair.Key + "] ");
                            pair.Value.Show();   // виртуальный Show()
                            any = true;
                        }
                    if (!any) Console.WriteLine("Объектов такого вида нет.");
                    break;
                }
                case 3:
                {
                    int c = Helper.ReadInt("Курс (1-6)", 1, 6);
                    bool any = false;
                    foreach (KeyValuePair<int, Person> pair in dict)
                    {
                        Student s = pair.Value as Student;
                        if (s != null && s.Course == c) { Console.WriteLine("  [" + pair.Key + "] " + s.Name); any = true; }
                    }
                    if (!any) Console.WriteLine("Студентов этого курса нет.");
                    break;
                }
            }
        }

        // Конструктор Dictionary(IDictionary) копирует только ссылки (поверхностная копия).
        // Для независимой копии в новый словарь добавляются клоны значений.
        private void CloneDemo()
        {
            if (dict.Count == 0) { Console.WriteLine("Словарь пуст."); return; }

            Dictionary<int, Person> shallow = new Dictionary<int, Person>(dict);
            Dictionary<int, Person> deep = new Dictionary<int, Person>();
            foreach (KeyValuePair<int, Person> pair in dict)
                deep.Add(pair.Key, (Person)pair.Value.Clone());

            int key = 0;
            foreach (int k in dict.Keys) { key = k; break; }
            string oldName = dict[key].Name;
            dict[key].Name = oldName + " (изменено)";

            Console.WriteLine("Изменили ФИО объекта [" + key + "] в исходном словаре.");
            Console.WriteLine("Исходный словарь:        " + dict[key]);
            Console.WriteLine("Поверхностная копия:     " + shallow[key]);
            Console.WriteLine("Глубокая копия (Clone):  " + deep[key]);
            dict[key].Name = oldName;
        }

        // Dictionary не хранит порядок элементов, поэтому сортировка выполняется над списком пар
        private void Sort()
        {
            Console.WriteLine("1 - по ФИО (IComparable), 2 - по возрасту");
            List<KeyValuePair<int, Person>> list = new List<KeyValuePair<int, Person>>(dict);
            if (Helper.ReadInt("Выбор", 1, 2) == 1)
                list.Sort((a, b) => a.Value.CompareTo(b.Value));
            else
                list.Sort((a, b) => a.Value.Age.CompareTo(b.Value.Age));

            Console.WriteLine("Элементы после сортировки:");
            foreach (KeyValuePair<int, Person> pair in list)
                Console.WriteLine("  [" + pair.Key + "] " + pair.Value);
        }

        private void Find()
        {
            Console.WriteLine("1 - Поиск по ключу, 2 - Поиск по ФИО");
            if (Helper.ReadInt("Выбор", 1, 2) == 1)
            {
                int key = Helper.ReadInt("Ключ", 1, int.MaxValue);
                Person p;
                if (dict.TryGetValue(key, out p)) Console.WriteLine("Найден: " + p);
                else Console.WriteLine("Элемента с таким ключом нет.");
            }
            else
            {
                string name = Input.Str("ФИО");
                bool found = false;
                foreach (KeyValuePair<int, Person> pair in dict)
                    if (pair.Value.Name == name)
                    {
                        Console.WriteLine("Найден [" + pair.Key + "]: " + pair.Value);
                        // ContainsValue ищет значение по содержимому (Equals)
                        Console.WriteLine("ContainsValue для копии найденного объекта: " + dict.ContainsValue((Person)pair.Value.Clone()));
                        found = true;
                    }
                if (!found) Console.WriteLine("Объект не найден.");
            }
        }
    }
}
