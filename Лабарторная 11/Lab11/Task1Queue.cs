using System.Collections;
using Lab10Lib;

namespace Lab11
{
    // Задание 1. Необобщённая коллекция Queue (System.Collections)
    public class Task1Queue
    {
        private Queue queue = new Queue();

        public Task1Queue()
        {
            foreach (Person p in Helper.StartSet())
                queue.Enqueue(p);
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\n=== Задание 1. Queue ===");
                Console.WriteLine("1 - Добавить объект (Enqueue)");
                Console.WriteLine("2 - Удалить первый объект (Dequeue)");
                Console.WriteLine("3 - Удалить объект по ФИО");
                Console.WriteLine("4 - Показать очередь (foreach)");
                Console.WriteLine("5 - Запросы");
                Console.WriteLine("6 - Клонировать очередь");
                Console.WriteLine("7 - Отсортировать очередь");
                Console.WriteLine("8 - Найти объект");
                Console.WriteLine("0 - Назад");
                switch (Helper.ReadInt("Выбор", 0, 8))
                {
                    case 1: Add(); break;
                    case 2: RemoveFirst(); break;
                    case 3: RemoveByName(); break;
                    case 4: Print(queue, "Очередь"); break;
                    case 5: Queries(); break;
                    case 6: CloneDemo(); break;
                    case 7: Sort(); break;
                    case 8: Find(); break;
                    case 0: return;
                }
            }
        }

        private void Add()
        {
            queue.Enqueue(Helper.ReadPerson());
            Console.WriteLine("Объект добавлен в конец очереди.");
        }

        private void RemoveFirst()
        {
            if (queue.Count == 0) { Console.WriteLine("Очередь пуста."); return; }
            Person p = (Person)queue.Dequeue();
            Console.WriteLine("Удалён объект: " + p);
        }

        // Из середины очереди элемент удалить нельзя, поэтому очередь пересоздаётся без удаляемых объектов
        private void RemoveByName()
        {
            string name = Input.Str("ФИО");
            Queue result = new Queue();
            int removed = 0;
            foreach (Person p in queue)
            {
                if (p.Name == name) removed++;
                else result.Enqueue(p);
            }
            queue = result;
            Console.WriteLine(removed > 0 ? "Удалено объектов: " + removed : "Объект не найден.");
        }

        private static void Print(Queue q, string title)
        {
            Console.WriteLine(title + " (элементов: " + q.Count + "):");
            if (q.Count == 0) { Console.WriteLine("  пусто"); return; }
            int i = 1;
            foreach (Person p in q)
                Console.WriteLine("  " + (i++) + ". " + p);
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
                    foreach (Person p in queue)
                        if (p.GetType() == t) count++;
                    Console.WriteLine("Количество объектов вида «" + Helper.TypeTitle(t) + "»: " + count);
                    break;
                }
                case 2:
                {
                    Type t = Helper.ReadType();
                    bool any = false;
                    foreach (Person p in queue)
                        if (p.GetType() == t) { p.Show(); any = true; }   // виртуальный Show()
                    if (!any) Console.WriteLine("Объектов такого вида нет.");
                    break;
                }
                case 3:
                {
                    int c = Helper.ReadInt("Курс (1-6)", 1, 6);
                    bool any = false;
                    foreach (Person p in queue)
                    {
                        Student s = p as Student;
                        if (s != null && s.Course == c) { Console.WriteLine("  " + s.Name); any = true; }
                    }
                    if (!any) Console.WriteLine("Студентов этого курса нет.");
                    break;
                }
            }
        }

        // Queue.Clone() выполняет поверхностное копирование: копия очереди ссылается на те же объекты.
        // Для независимой копии клонируется и каждый элемент (метод Clone из работы 10).
        private void CloneDemo()
        {
            if (queue.Count == 0) { Console.WriteLine("Очередь пуста."); return; }

            Queue shallow = (Queue)queue.Clone();
            Queue deep = new Queue();
            foreach (Person p in queue)
                deep.Enqueue(p.Clone());

            Person first = (Person)queue.Peek();
            string oldName = first.Name;
            first.Name = oldName + " (изменено)";

            Console.WriteLine("Изменили ФИО первого объекта исходной очереди.");
            Console.WriteLine("Исходная очередь:        " + queue.Peek());
            Console.WriteLine("Поверхностная копия:     " + shallow.Peek());
            Console.WriteLine("Глубокая копия (Clone):  " + deep.Peek());
            first.Name = oldName;   // возвращаем как было
        }

        // У Queue нет метода Sort: копируем элементы в массив, сортируем и создаём очередь заново
        private void Sort()
        {
            Console.WriteLine("1 - по ФИО (IComparable), 2 - по возрасту (IComparer, SortByAge)");
            object[] array = queue.ToArray();
            if (Helper.ReadInt("Выбор", 1, 2) == 1) Array.Sort(array);
            else Array.Sort(array, new SortByAge());
            queue = new Queue(array);
            Print(queue, "Очередь после сортировки");
        }

        private void Find()
        {
            string name = Input.Str("ФИО для поиска");
            int position = 1;
            bool found = false;
            foreach (Person p in queue)
            {
                if (p.Name == name)
                {
                    Console.WriteLine("Найден (позиция в очереди " + position + "): " + p);
                    // Contains ищет объект по содержимому (используется переопределённый Equals)
                    Console.WriteLine("Contains для копии найденного объекта: " + queue.Contains(p.Clone()));
                    found = true;
                }
                position++;
            }
            if (!found) Console.WriteLine("Объект не найден.");
        }
    }
}
