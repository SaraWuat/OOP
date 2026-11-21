using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab10Lib;
using Lab11;

namespace Lab11Tests
{
    [TestClass]
    [DoNotParallelize]  // тесты подменяют Console.Out / Console.In, одновременный запуск их ломает
    public class UnitTests
    {
        // ---------- Вспомогательные методы ----------

        static readonly TextReader oldIn = Console.In;

        [TestInitialize]
        public void Init()
        {
            // Единый формат чисел (F2 даёт «4.00», а не «4,00»), независимо от настроек системы
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetIn(oldIn);
        }

        // Перехват вывода в консоль
        static string Capture(Action a)
        {
            TextWriter old = Console.Out;
            StringWriter sw = new StringWriter();
            Console.SetOut(sw);
            try { a(); }
            finally { Console.SetOut(old); }
            return sw.ToString();
        }

        // Подмена ввода с клавиатуры
        static void SetInput(string s)
        {
            Console.SetIn(new StringReader(s));
        }

        // Запуск меню задания 1 / задания 2 с заданным вводом, результат - всё, что выведено в консоль
        static string RunTask1(string input)
        {
            SetInput(input);
            return Capture(() => new Task1Queue().Run());
        }

        static string RunTask2(string input)
        {
            SetInput(input);
            return Capture(() => new Task2Dictionary().Run());
        }

        static bool Throws<T>(Action a) where T : Exception
        {
            try { a(); }
            catch (T) { return true; }
            return false;
        }

        // Ввод с клавиатуры известного студента (для меню: тип 2 - студент, 1 - ввод с клавиатуры)
        const string KnownName = "Тестов Тест Тестович";
        const string AddKnownStudent = "1\n2\n1\n" + KnownName + "\nм\n20\n3\n4\n";
        const string KnownStudentLine = "Студент: " + KnownName + ", М, 20 л., 3 курс, балл: 4.00";

        static int Count(string text, string part)
        {
            int n = 0, i = 0;
            while ((i = text.IndexOf(part, i, StringComparison.Ordinal)) >= 0) { n++; i += part.Length; }
            return n;
        }

        // Строка вывода, начинающаяся с заданного префикса
        static string Line(string output, string prefix)
        {
            foreach (string l in output.Split('\n'))
                if (l.StartsWith(prefix)) return l;
            return null;
        }

        // Разбор списка объектов, напечатанного после marker: возвращает пары (ФИО, возраст)
        static List<Tuple<string, int>> Parse(string output, string marker)
        {
            int i = output.IndexOf(marker, StringComparison.Ordinal);
            Assert.IsTrue(i >= 0, "В выводе нет строки: " + marker);
            string tail = output.Substring(i);
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();
            foreach (Match m in Regex.Matches(tail, @"^\s+(?:\d+\.|\[\d+\]) [^:\r\n]+: (.+?), (?:М|Ж), (\d+) л\.", RegexOptions.Multiline))
                list.Add(Tuple.Create(m.Groups[1].Value, int.Parse(m.Groups[2].Value)));
            return list;
        }

        static Student MakeStudent() { return new Student("Иванов", true, 19, 2, 4.5); }

        // ====================================================================
        // Изменения иерархии классов в работе 11: BasePerson и ToString
        // ====================================================================

        [TestMethod]
        public void BasePerson_IsPersonWithSameData()
        {
            Student s = MakeStudent();
            Person b = s.BasePerson;
            Assert.AreEqual(typeof(Person), b.GetType());
            Assert.AreEqual("Иванов", b.Name);
            Assert.IsTrue(b.Male);
            Assert.AreEqual(19, b.Age);
        }

        [TestMethod]
        public void BasePerson_ReturnsNewObjectEveryTime()
        {
            Student s = MakeStudent();
            Person a = s.BasePerson;
            Person b = s.BasePerson;
            Assert.IsFalse(ReferenceEquals(a, b));
            a.Name = "Другой";
            Assert.AreEqual("Иванов", s.Name);       // изменение подобъекта не затрагивает студента
            Assert.AreEqual("Иванов", b.Name);
        }

        [TestMethod]
        public void BasePerson_EqualsPersonWithSameFields()
        {
            Person p = new Person("Иванов", true, 19);
            Person b = MakeStudent().BasePerson;
            Assert.IsTrue(p.Equals(b));
            Assert.AreEqual(p.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void BasePerson_NotEqualToStudentItself()
        {
            Student s = MakeStudent();
            Assert.IsFalse(s.Equals(s.BasePerson));
            Assert.IsFalse(s.BasePerson.Equals(s));
        }

        [TestMethod]
        public void BasePerson_ChangesAfterStudentChanges()
        {
            Student s = MakeStudent();
            s.Age = 25;
            s.Male = false;
            Assert.AreEqual(25, s.BasePerson.Age);
            Assert.IsFalse(s.BasePerson.Male);
        }

        [TestMethod]
        public void ToString_Person()
        {
            Assert.AreEqual("Персона: Иванов, М, 30 л.", new Person("Иванов", true, 30).ToString());
            Assert.AreEqual("Персона: Иванова, Ж, 28 л.", new Person("Иванова", false, 28).ToString());
        }

        [TestMethod]
        public void ToString_Student()
        {
            Assert.AreEqual("Студент: Иванов, М, 19 л., 2 курс, балл: 4.50", MakeStudent().ToString());
        }

        [TestMethod]
        public void ToString_Employee()
        {
            Employee e = new Employee("Иванов", true, 30, "Инженер", 5);
            Assert.AreEqual("Сотрудник: Иванов, М, 30 л., Инженер, стаж: 5 л.", e.ToString());
        }

        [TestMethod]
        public void ToString_Teacher()
        {
            Teacher t = new Teacher("Петров", true, 45, 20, "ИТАС", "к.т.н.");
            Assert.AreEqual("Преподаватель: Петров, М, 45 л., стаж: 20 л., степень: к.т.н., каф. ИТАС", t.ToString());
        }

        [TestMethod]
        public void ToString_IsVirtual_WorksThroughBaseReference()
        {
            Person p = MakeStudent();
            Assert.IsTrue(p.ToString().StartsWith("Студент:"));
            object o = new Teacher("Петров", true, 45, 20, "ИТАС", "к.т.н.");
            Assert.IsTrue(o.ToString().StartsWith("Преподаватель:"));
        }

        [TestMethod]
        public void ToString_EqualObjects_SameString()
        {
            Assert.AreEqual(MakeStudent().ToString(), new Student(MakeStudent()).ToString());
        }

        [TestMethod]
        public void ToString_DifferentPersons_DifferentStrings()
        {
            // строка Person используется как ключ словаря, поэтому она должна различать ФИО, пол и возраст
            string a = new Person("Иванов", true, 30).ToString();
            Assert.AreNotEqual(a, new Person("Петров", true, 30).ToString());
            Assert.AreNotEqual(a, new Person("Иванов", false, 30).ToString());
            Assert.AreNotEqual(a, new Person("Иванов", true, 31).ToString());
        }

        // ====================================================================
        // Вспомогательный класс Helper
        // ====================================================================

        [TestMethod]
        public void Helper_ReadInt_ValidFirstTry()
        {
            SetInput("5\n");
            int v = 0;
            string output = Capture(() => v = Helper.ReadInt("Выбор", 1, 10));
            Assert.AreEqual(5, v);
            Assert.IsFalse(output.Contains("Введите число"));
        }

        [TestMethod]
        public void Helper_ReadInt_BoundsAreIncluded()
        {
            SetInput("1\n10\n");
            int a = 0, b = 0;
            Capture(() => { a = Helper.ReadInt("Выбор", 1, 10); b = Helper.ReadInt("Выбор", 1, 10); });
            Assert.AreEqual(1, a);
            Assert.AreEqual(10, b);
        }

        [TestMethod]
        public void Helper_ReadInt_RepeatsOnInvalidInput()
        {
            SetInput("abc\n0\n11\n\n7\n");
            int v = 0;
            string output = Capture(() => v = Helper.ReadInt("Выбор", 1, 10));
            Assert.AreEqual(7, v);
            Assert.AreEqual(4, Count(output, "Введите число от 1 до 10."));
        }

        [TestMethod]
        public void Helper_ReadInt_EndOfInput_Throws()
        {
            SetInput("");
            Assert.IsTrue(Throws<EndOfStreamException>(() => Capture(() => Helper.ReadInt("Выбор", 1, 10))));
        }

        [TestMethod]
        public void Helper_ReadType_AllKinds()
        {
            SetInput("1\n2\n3\n4\n");
            Type[] t = new Type[4];
            Capture(() => { for (int i = 0; i < 4; i++) t[i] = Helper.ReadType(); });
            Assert.AreEqual(typeof(Person), t[0]);
            Assert.AreEqual(typeof(Student), t[1]);
            Assert.AreEqual(typeof(Employee), t[2]);
            Assert.AreEqual(typeof(Teacher), t[3]);
        }

        [TestMethod]
        public void Helper_Create_ReturnsRequestedType()
        {
            Assert.AreEqual(typeof(Person), Helper.Create(typeof(Person)).GetType());
            Assert.AreEqual(typeof(Student), Helper.Create(typeof(Student)).GetType());
            Assert.AreEqual(typeof(Employee), Helper.Create(typeof(Employee)).GetType());
            Assert.AreEqual(typeof(Teacher), Helper.Create(typeof(Teacher)).GetType());
        }

        [TestMethod]
        public void Helper_TypeTitle()
        {
            Assert.AreEqual("персона", Helper.TypeTitle(typeof(Person)));
            Assert.AreEqual("студент", Helper.TypeTitle(typeof(Student)));
            Assert.AreEqual("сотрудник", Helper.TypeTitle(typeof(Employee)));
            Assert.AreEqual("преподаватель", Helper.TypeTitle(typeof(Teacher)));
        }

        [TestMethod]
        public void Helper_StartSet_Composition()
        {
            Person[] set = Helper.StartSet();
            Assert.AreEqual(8, set.Length);
            int persons = 0, students = 0, employees = 0, teachers = 0;
            foreach (Person p in set)
            {
                Type t = p.GetType();
                if (t == typeof(Person)) persons++;
                else if (t == typeof(Student)) students++;
                else if (t == typeof(Employee)) employees++;
                else if (t == typeof(Teacher)) teachers++;
            }
            Assert.AreEqual(1, persons);
            Assert.AreEqual(3, students);
            Assert.AreEqual(2, employees);
            Assert.AreEqual(2, teachers);
        }

        [TestMethod]
        public void Helper_StartSet_ObjectsAreFilled()
        {
            foreach (Person p in Helper.StartSet())
            {
                Assert.AreNotEqual("Без имени", p.Name);
                Assert.IsTrue(p.Age >= 17 && p.Age <= 70);
            }
        }

        [TestMethod]
        public void Helper_ReadPerson_KeyboardStudent()
        {
            SetInput("2\n1\nИванов\nм\n19\n2\n4\n");
            Person p = null;
            Capture(() => p = Helper.ReadPerson());
            Assert.AreEqual(typeof(Student), p.GetType());
            Assert.AreEqual("Студент: Иванов, М, 19 л., 2 курс, балл: 4.00", p.ToString());
        }

        [TestMethod]
        public void Helper_ReadPerson_KeyboardTeacher()
        {
            // Teacher.Init: ФИО, пол, возраст, должность, стаж (Employee), кафедра, степень
            SetInput("4\n1\nПетров\nм\n45\nпреподаватель\n20\nИТАС\nк.т.н.\n");
            Person p = null;
            Capture(() => p = Helper.ReadPerson());
            Assert.AreEqual(typeof(Teacher), p.GetType());
            Assert.AreEqual("Преподаватель: Петров, М, 45 л., стаж: 20 л., степень: к.т.н., каф. ИТАС", p.ToString());
        }

        [TestMethod]
        public void Helper_ReadPerson_RandomPerson()
        {
            SetInput("1\n2\n");
            Person p = null;
            Capture(() => p = Helper.ReadPerson());
            Assert.AreEqual(typeof(Person), p.GetType());
            Assert.AreNotEqual("Без имени", p.Name);
        }

        // ====================================================================
        // Стандартные коллекции с объектами иерархии (то, на чём построены задания 1 и 2)
        // ====================================================================

        [TestMethod]
        public void Queue_Fifo()
        {
            Queue q = new Queue();
            q.Enqueue(new Person("А", true, 20));
            q.Enqueue(new Person("Б", true, 21));
            Assert.AreEqual("А", ((Person)q.Dequeue()).Name);
            Assert.AreEqual("Б", ((Person)q.Peek()).Name);
            Assert.AreEqual(1, q.Count);
        }

        [TestMethod]
        public void Queue_Contains_FindsCopyByContent()
        {
            Queue q = new Queue();
            Student s = MakeStudent();
            q.Enqueue(new Person("Сидоров", true, 40));
            q.Enqueue(s);
            Assert.IsTrue(q.Contains(new Student(s)));
            Assert.IsTrue(q.Contains(s.Clone()));
        }

        [TestMethod]
        public void Queue_Contains_DifferentObjectNotFound()
        {
            Queue q = new Queue();
            q.Enqueue(MakeStudent());
            Assert.IsFalse(q.Contains(new Student("Иванов", true, 19, 3, 4.5)));   // другой курс
            Assert.IsFalse(q.Contains(new Person("Иванов", true, 19)));            // другой тип
        }

        [TestMethod]
        public void Queue_ShallowClone_SharesObjects()
        {
            Queue q = new Queue();
            q.Enqueue(MakeStudent());
            Queue c = (Queue)q.Clone();
            Assert.IsTrue(ReferenceEquals(q.Peek(), c.Peek()));
            ((Person)q.Peek()).Name = "Изменено";
            Assert.AreEqual("Изменено", ((Person)c.Peek()).Name);
        }

        [TestMethod]
        public void Queue_DeepClone_IsIndependent()
        {
            Queue q = new Queue();
            q.Enqueue(MakeStudent());
            q.Enqueue(new Teacher("Петров", true, 45, 20, "ИТАС", "к.т.н."));
            Queue deep = new Queue();
            foreach (Person p in q) deep.Enqueue(p.Clone());

            Assert.AreEqual(2, deep.Count);
            Assert.IsFalse(ReferenceEquals(q.Peek(), deep.Peek()));
            Assert.IsTrue(q.Peek().Equals(deep.Peek()));        // содержимое то же
            ((Person)q.Peek()).Name = "Изменено";
            Assert.AreEqual("Иванов", ((Person)deep.Peek()).Name);
        }

        [TestMethod]
        public void Queue_SortByName_UsesIComparable()
        {
            Queue q = new Queue();
            q.Enqueue(new Person("Яковлев", true, 20));
            q.Enqueue(new Person("Абрамов", true, 50));
            q.Enqueue(new Person("Лебедев", true, 30));
            object[] a = q.ToArray();
            Array.Sort(a);
            q = new Queue(a);
            Assert.AreEqual("Абрамов", ((Person)q.Dequeue()).Name);
            Assert.AreEqual("Лебедев", ((Person)q.Dequeue()).Name);
            Assert.AreEqual("Яковлев", ((Person)q.Dequeue()).Name);
        }

        [TestMethod]
        public void Queue_SortByAge_UsesSortByAge()
        {
            Queue q = new Queue();
            q.Enqueue(new Person("А", true, 50));
            q.Enqueue(new Person("Б", true, 20));
            q.Enqueue(new Person("В", true, 30));
            object[] a = q.ToArray();
            Array.Sort(a, new SortByAge());
            q = new Queue(a);
            Assert.AreEqual(20, ((Person)q.Dequeue()).Age);
            Assert.AreEqual(30, ((Person)q.Dequeue()).Age);
            Assert.AreEqual(50, ((Person)q.Dequeue()).Age);
        }

        [TestMethod]
        public void Dictionary_ContainsKey_FindsCopyOfKey()
        {
            Dictionary<Person, Student> d = new Dictionary<Person, Student>();
            Student s = MakeStudent();
            d.Add(s.BasePerson, s);
            Assert.IsTrue(d.ContainsKey(new Person("Иванов", true, 19)));       // новый объект с тем же содержимым
            Assert.IsTrue(d.ContainsKey(new Student(s).BasePerson));
            Assert.IsFalse(d.ContainsKey(new Person("Иванов", false, 19)));
            Assert.IsFalse(d.ContainsKey(new Person("Иванов", true, 20)));
        }

        [TestMethod]
        public void Dictionary_ContainsKey_StudentIsNotPersonKey()
        {
            Dictionary<Person, Student> d = new Dictionary<Person, Student>();
            Student s = MakeStudent();
            d.Add(s.BasePerson, s);
            Assert.IsFalse(d.ContainsKey(s));    // ключ - Person, а не Student
        }

        [TestMethod]
        public void Dictionary_ContainsValue_FindsCopyOfValue()
        {
            Dictionary<Person, Student> d = new Dictionary<Person, Student>();
            Student s = MakeStudent();
            d.Add(s.BasePerson, s);
            Assert.IsTrue(d.ContainsValue(new Student(s)));
            Assert.IsFalse(d.ContainsValue(new Student("Иванов", true, 19, 2, 5.0)));   // другой балл
        }

        [TestMethod]
        public void Dictionary_StringKey_FindsByToString()
        {
            Dictionary<string, Student> d = new Dictionary<string, Student>();
            Student s = MakeStudent();
            d.Add(s.BasePerson.ToString(), s);
            Assert.IsTrue(d.ContainsKey(new Student(s).BasePerson.ToString()));
            Assert.IsFalse(d.ContainsKey(new Person("Петров", true, 19).ToString()));
        }

        [TestMethod]
        public void Dictionary_DuplicateKey_Throws()
        {
            Dictionary<Person, Student> d = new Dictionary<Person, Student>();
            Student s = MakeStudent();
            d.Add(s.BasePerson, s);
            Assert.IsTrue(Throws<ArgumentException>(() => d.Add(new Student(s).BasePerson, new Student(s))));
        }

        [TestMethod]
        public void Dictionary_ShallowCopy_SharesValues()
        {
            Dictionary<int, Person> d = new Dictionary<int, Person>();
            d.Add(1, MakeStudent());
            Dictionary<int, Person> c = new Dictionary<int, Person>(d);
            Assert.IsTrue(ReferenceEquals(d[1], c[1]));
            d[1].Name = "Изменено";
            Assert.AreEqual("Изменено", c[1].Name);
        }

        [TestMethod]
        public void Dictionary_DeepCopy_IsIndependent()
        {
            Dictionary<int, Person> d = new Dictionary<int, Person>();
            d.Add(1, MakeStudent());
            Dictionary<int, Person> deep = new Dictionary<int, Person>();
            foreach (KeyValuePair<int, Person> pair in d)
                deep.Add(pair.Key, (Person)pair.Value.Clone());
            d[1].Name = "Изменено";
            Assert.AreEqual("Иванов", deep[1].Name);
            Assert.IsTrue(deep[1] is Student);
        }

        [TestMethod]
        public void Dictionary_SortByValues_UsesIComparable()
        {
            Dictionary<int, Person> d = new Dictionary<int, Person>();
            d.Add(1, new Person("Яковлев", true, 20));
            d.Add(2, new Person("Абрамов", true, 50));
            d.Add(3, new Person("Лебедев", true, 30));
            List<KeyValuePair<int, Person>> list = new List<KeyValuePair<int, Person>>(d);
            list.Sort((a, b) => a.Value.CompareTo(b.Value));
            Assert.AreEqual(2, list[0].Key);
            Assert.AreEqual(3, list[1].Key);
            Assert.AreEqual(1, list[2].Key);
        }

        // ====================================================================
        // Задание 1. Task1Queue (меню, управляемое подменой ввода)
        // ====================================================================

        [TestMethod]
        public void Task1_InitialQueueHasEightElements()
        {
            string o = RunTask1("4\n0\n");
            Assert.IsTrue(o.Contains("Очередь (элементов: 8):"));
        }

        [TestMethod]
        public void Task1_AddRandomObject()
        {
            string o = RunTask1("1\n2\n2\n4\n0\n");
            Assert.IsTrue(o.Contains("Объект добавлен в конец очереди."));
            Assert.IsTrue(o.Contains("Очередь (элементов: 9):"));
        }

        [TestMethod]
        public void Task1_AddKeyboardObject_GoesToTheEnd()
        {
            string o = RunTask1(AddKnownStudent + "4\n0\n");
            Assert.IsTrue(o.Contains("9. " + KnownStudentLine));
        }

        [TestMethod]
        public void Task1_AddAllKinds()
        {
            // персона, студент, сотрудник, преподаватель - случайные данные
            string o = RunTask1("1\n1\n2\n1\n2\n2\n1\n3\n2\n1\n4\n2\n4\n0\n");
            Assert.IsTrue(o.Contains("Очередь (элементов: 12):"));
        }

        [TestMethod]
        public void Task1_Dequeue_RemovesFirst()
        {
            string o = RunTask1("2\n4\n0\n");
            Assert.IsTrue(o.Contains("Удалён объект: "));
            Assert.IsTrue(o.Contains("Очередь (элементов: 7):"));
        }

        [TestMethod]
        public void Task1_Dequeue_ReturnsFirstElement()
        {
            // известный студент добавляется в конец; после 8 удалений он остаётся единственным
            string o = RunTask1(AddKnownStudent + "2\n2\n2\n2\n2\n2\n2\n2\n4\n0\n");
            Assert.IsTrue(o.Contains("Очередь (элементов: 1):"));
            Assert.IsTrue(o.Contains("1. " + KnownStudentLine));
        }

        [TestMethod]
        public void Task1_Dequeue_EmptyQueue()
        {
            string o = RunTask1("2\n2\n2\n2\n2\n2\n2\n2\n2\n4\n0\n");   // 9-й вызов - очередь уже пуста
            Assert.AreEqual(1, Count(o, "Очередь пуста."));
            Assert.IsTrue(o.Contains("  пусто"));
        }

        [TestMethod]
        public void Task1_RemoveByName_Existing()
        {
            string o = RunTask1(AddKnownStudent + "3\n" + KnownName + "\n4\n0\n");
            Assert.IsTrue(o.Contains("Удалено объектов: 1"));
            Assert.IsTrue(o.Contains("Очередь (элементов: 8):"));
        }

        [TestMethod]
        public void Task1_RemoveByName_RemovesAllWithSameName()
        {
            string o = RunTask1(AddKnownStudent + AddKnownStudent + "3\n" + KnownName + "\n4\n0\n");
            Assert.IsTrue(o.Contains("Удалено объектов: 2"));
            Assert.IsTrue(o.Contains("Очередь (элементов: 8):"));
        }

        [TestMethod]
        public void Task1_RemoveByName_NotFound()
        {
            string o = RunTask1("3\nНет Такого Человека\n4\n0\n");
            Assert.IsTrue(o.Contains("Объект не найден."));
            Assert.IsTrue(o.Contains("Очередь (элементов: 8):"));
        }

        [TestMethod]
        public void Task1_Query1_CountByType()
        {
            // начальный набор: персона - 1, студенты - 3, сотрудники - 2, преподаватели - 2
            string o = RunTask1("5\n1\n1\n5\n1\n2\n5\n1\n3\n5\n1\n4\n0\n");
            Assert.IsTrue(o.Contains("вида «персона»: 1"));
            Assert.IsTrue(o.Contains("вида «студент»: 3"));
            Assert.IsTrue(o.Contains("вида «сотрудник»: 2"));
            Assert.IsTrue(o.Contains("вида «преподаватель»: 2"));
        }

        [TestMethod]
        public void Task1_Query1_ExactTypeIsCounted()
        {
            // сотрудник и преподаватель - разные виды: добавление преподавателя не меняет число сотрудников
            string o = RunTask1("1\n4\n2\n5\n1\n3\n5\n1\n4\n0\n");
            Assert.IsTrue(o.Contains("вида «сотрудник»: 2"));
            Assert.IsTrue(o.Contains("вида «преподаватель»: 3"));
        }

        [TestMethod]
        public void Task1_Query2_PrintsObjectsOfType()
        {
            string o = RunTask1("5\n2\n4\n0\n");
            Assert.AreEqual(2, Count(o, "Преподаватель: "));
            Assert.AreEqual(0, Count(o, "Студент: "));
        }

        [TestMethod]
        public void Task1_Query2_PrintsStudents()
        {
            string o = RunTask1("5\n2\n2\n0\n");
            Assert.AreEqual(3, Count(o, "Студент: "));
        }

        [TestMethod]
        public void Task1_Query3_StudentsOfCourse()
        {
            string o = RunTask1(AddKnownStudent + "5\n3\n3\n0\n");
            Assert.IsTrue(o.Contains("  " + KnownName));
        }

        [TestMethod]
        public void Task1_Query3_OtherCourseNotListed()
        {
            string o = RunTask1(AddKnownStudent + "5\n3\n6\n0\n");
            Assert.IsFalse(o.Contains("  " + KnownName));
        }

        [TestMethod]
        public void Task1_Clone_ShallowFollowsOriginalDeepDoesNot()
        {
            string o = RunTask1("6\n0\n");
            Assert.IsTrue(Line(o, "Исходная очередь:").Contains("(изменено)"));
            Assert.IsTrue(Line(o, "Поверхностная копия:").Contains("(изменено)"));
            Assert.IsFalse(Line(o, "Глубокая копия (Clone):").Contains("(изменено)"));
        }

        [TestMethod]
        public void Task1_Clone_OriginalIsRestored()
        {
            string o = RunTask1("6\n4\n0\n");
            Assert.IsFalse(o.Substring(o.IndexOf("Очередь (элементов:", StringComparison.Ordinal)).Contains("(изменено)"));
        }

        [TestMethod]
        public void Task1_Clone_EmptyQueue()
        {
            string o = RunTask1("2\n2\n2\n2\n2\n2\n2\n2\n6\n0\n");
            Assert.IsTrue(o.Contains("Очередь пуста."));
        }

        [TestMethod]
        public void Task1_SortByName()
        {
            string o = RunTask1("7\n1\n0\n");
            List<Tuple<string, int>> list = Parse(o, "Очередь после сортировки");
            Assert.AreEqual(8, list.Count);
            for (int i = 1; i < list.Count; i++)
                Assert.IsTrue(string.Compare(list[i - 1].Item1, list[i].Item1) <= 0);
        }

        [TestMethod]
        public void Task1_SortByAge()
        {
            string o = RunTask1("7\n2\n0\n");
            List<Tuple<string, int>> list = Parse(o, "Очередь после сортировки");
            Assert.AreEqual(8, list.Count);
            for (int i = 1; i < list.Count; i++)
                Assert.IsTrue(list[i - 1].Item2 <= list[i].Item2);
        }

        [TestMethod]
        public void Task1_Sort_KeepsAllElements()
        {
            string o = RunTask1("7\n1\n4\n0\n");
            Assert.AreEqual(2, Count(o, "(элементов: 8):"));   // после сортировки и при обычном выводе
        }

        [TestMethod]
        public void Task1_Find_Existing()
        {
            string o = RunTask1(AddKnownStudent + "8\n" + KnownName + "\n0\n");
            Assert.IsTrue(o.Contains("Найден (позиция в очереди 9): " + KnownStudentLine));
            Assert.IsTrue(o.Contains("Contains для копии найденного объекта: True"));
        }

        [TestMethod]
        public void Task1_Find_NotFound()
        {
            string o = RunTask1("8\nНет Такого Человека\n0\n");
            Assert.IsTrue(o.Contains("Объект не найден."));
            Assert.IsFalse(o.Contains("Найден"));
        }

        [TestMethod]
        public void Task1_InvalidMenuInput_IsRepeated()
        {
            string o = RunTask1("abc\n99\n4\n0\n");
            Assert.AreEqual(2, Count(o, "Введите число от 0 до 8."));
            Assert.IsTrue(o.Contains("Очередь (элементов: 8):"));
        }

        [TestMethod]
        public void Task1_EndOfInput_Throws()
        {
            SetInput("");
            Assert.IsTrue(Throws<EndOfStreamException>(() => Capture(() => new Task1Queue().Run())));
        }

        // ====================================================================
        // Задание 2. Task2Dictionary
        // ====================================================================

        [TestMethod]
        public void Task2_InitialDictionaryHasEightElements()
        {
            string o = RunTask2("3\n0\n");
            Assert.IsTrue(o.Contains("Словарь (элементов: 8):"));
            Assert.IsTrue(o.Contains("[1] "));
            Assert.IsTrue(o.Contains("[8] "));
        }

        [TestMethod]
        public void Task2_AddRandomObject_GetsNextKey()
        {
            string o = RunTask2("1\n2\n2\n3\n0\n");
            Assert.IsTrue(o.Contains("Объект добавлен с ключом 9."));
            Assert.IsTrue(o.Contains("Словарь (элементов: 9):"));
        }

        [TestMethod]
        public void Task2_AddKeyboardObject()
        {
            string o = RunTask2(AddKnownStudent + "3\n0\n");
            Assert.IsTrue(o.Contains("[9] " + KnownStudentLine));
        }

        [TestMethod]
        public void Task2_Keys_AreNotReusedAfterRemove()
        {
            // удаляем ключ 8, затем добавляем объект: ключ должен быть 9, а не 8
            string o = RunTask2("2\n8\n1\n2\n2\n0\n");
            Assert.IsTrue(o.Contains("Объект добавлен с ключом 9."));
        }

        [TestMethod]
        public void Task2_Remove_ExistingKey()
        {
            string o = RunTask2("2\n1\n3\n0\n");
            Assert.IsTrue(o.Contains("Удалён объект: "));
            Assert.IsTrue(o.Contains("Словарь (элементов: 7):"));
            Assert.IsFalse(o.Contains("  [1] "));
        }

        [TestMethod]
        public void Task2_Remove_MissingKey()
        {
            string o = RunTask2("2\n99\n3\n0\n");
            Assert.IsTrue(o.Contains("Элемента с таким ключом нет."));
            Assert.IsTrue(o.Contains("Словарь (элементов: 8):"));
        }

        [TestMethod]
        public void Task2_Remove_SameKeyTwice()
        {
            string o = RunTask2("2\n3\n2\n3\n0\n");
            Assert.AreEqual(1, Count(o, "Удалён объект: "));
            Assert.AreEqual(1, Count(o, "Элемента с таким ключом нет."));
        }

        [TestMethod]
        public void Task2_Query1_CountByType()
        {
            string o = RunTask2("4\n1\n1\n4\n1\n2\n4\n1\n3\n4\n1\n4\n0\n");
            Assert.IsTrue(o.Contains("вида «персона»: 1"));
            Assert.IsTrue(o.Contains("вида «студент»: 3"));
            Assert.IsTrue(o.Contains("вида «сотрудник»: 2"));
            Assert.IsTrue(o.Contains("вида «преподаватель»: 2"));
        }

        [TestMethod]
        public void Task2_Query2_PrintsKeysAndObjects()
        {
            string o = RunTask2("4\n2\n4\n0\n");
            Assert.AreEqual(2, Count(o, "Преподаватель: "));
            Assert.AreEqual(2, Regex.Matches(o, @"\[\d+\] Преподаватель: ").Count);
        }

        [TestMethod]
        public void Task2_Query3_StudentsOfCourse()
        {
            string o = RunTask2(AddKnownStudent + "4\n3\n3\n0\n");
            Assert.IsTrue(o.Contains("  [9] " + KnownName));
        }

        [TestMethod]
        public void Task2_Clone_ShallowFollowsOriginalDeepDoesNot()
        {
            string o = RunTask2("5\n0\n");
            Assert.IsTrue(Line(o, "Исходный словарь:").Contains("(изменено)"));
            Assert.IsTrue(Line(o, "Поверхностная копия:").Contains("(изменено)"));
            Assert.IsFalse(Line(o, "Глубокая копия (Clone):").Contains("(изменено)"));
        }

        [TestMethod]
        public void Task2_Clone_OriginalIsRestored()
        {
            string o = RunTask2("5\n3\n0\n");
            Assert.IsFalse(o.Substring(o.IndexOf("Словарь (элементов:", StringComparison.Ordinal)).Contains("(изменено)"));
        }

        [TestMethod]
        public void Task2_Clone_EmptyDictionary()
        {
            string o = RunTask2("2\n1\n2\n2\n2\n3\n2\n4\n2\n5\n2\n6\n2\n7\n2\n8\n5\n0\n");
            Assert.IsTrue(o.Contains("Словарь пуст."));
        }

        [TestMethod]
        public void Task2_SortByName()
        {
            string o = RunTask2("6\n1\n0\n");
            List<Tuple<string, int>> list = Parse(o, "Элементы после сортировки");
            Assert.AreEqual(8, list.Count);
            for (int i = 1; i < list.Count; i++)
                Assert.IsTrue(string.Compare(list[i - 1].Item1, list[i].Item1) <= 0);
        }

        [TestMethod]
        public void Task2_SortByAge()
        {
            string o = RunTask2("6\n2\n0\n");
            List<Tuple<string, int>> list = Parse(o, "Элементы после сортировки");
            Assert.AreEqual(8, list.Count);
            for (int i = 1; i < list.Count; i++)
                Assert.IsTrue(list[i - 1].Item2 <= list[i].Item2);
        }

        [TestMethod]
        public void Task2_Sort_DoesNotChangeDictionary()
        {
            string o = RunTask2("6\n1\n3\n0\n");
            Assert.IsTrue(o.Contains("Словарь (элементов: 8):"));
        }

        [TestMethod]
        public void Task2_Find_ByKey()
        {
            string o = RunTask2("7\n1\n1\n0\n");
            Assert.IsTrue(o.Contains("Найден: "));
        }

        [TestMethod]
        public void Task2_Find_ByMissingKey()
        {
            string o = RunTask2("7\n1\n99\n0\n");
            Assert.IsTrue(o.Contains("Элемента с таким ключом нет."));
        }

        [TestMethod]
        public void Task2_Find_ByName()
        {
            string o = RunTask2(AddKnownStudent + "7\n2\n" + KnownName + "\n0\n");
            Assert.IsTrue(o.Contains("Найден [9]: " + KnownStudentLine));
            Assert.IsTrue(o.Contains("ContainsValue для копии найденного объекта: True"));
        }

        [TestMethod]
        public void Task2_Find_ByMissingName()
        {
            string o = RunTask2("7\n2\nНет Такого Человека\n0\n");
            Assert.IsTrue(o.Contains("Объект не найден."));
        }

        [TestMethod]
        public void Task2_InvalidMenuInput_IsRepeated()
        {
            string o = RunTask2("x\n8\n3\n0\n");
            Assert.AreEqual(2, Count(o, "Введите число от 0 до 7."));
        }

        [TestMethod]
        public void Task2_EndOfInput_Throws()
        {
            SetInput("");
            Assert.IsTrue(Throws<EndOfStreamException>(() => Capture(() => new Task2Dictionary().Run())));
        }

        // ====================================================================
        // Задание 3. TestCollections
        // ====================================================================

        // Все пять видов поиска для копии (а не ссылки) элемента
        static bool[] SearchAll(TestCollections tc, Student original)
        {
            Student copy = new Student(original);
            Assert.IsFalse(ReferenceEquals(copy, original));
            return new bool[]
            {
                tc.ListContains(copy),
                tc.StringListContains(copy.ToString()),
                tc.DictContainsKey(copy.BasePerson),
                tc.StringDictContainsKey(copy.BasePerson.ToString()),
                tc.DictContainsValue(copy)
            };
        }

        static void AssertCounts(TestCollections tc, int n)
        {
            Assert.AreEqual(n, tc.Count);
            Assert.AreEqual(n, tc.CountList);
            Assert.AreEqual(n, tc.CountStringList);
            Assert.AreEqual(n, tc.CountDict);
            Assert.AreEqual(n, tc.CountStringDict);
        }

        [TestMethod]
        public void Tc_Constructor_1000Elements_InAllFourCollections()
        {
            AssertCounts(new TestCollections(1000), 1000);
        }

        [TestMethod]
        public void Tc_Constructor_ZeroElements()
        {
            AssertCounts(new TestCollections(0), 0);
        }

        [TestMethod]
        public void Tc_Constructor_SmallCount()
        {
            AssertCounts(new TestCollections(1), 1);
            AssertCounts(new TestCollections(7), 7);
        }

        [TestMethod]
        public void Tc_Constructor_ElementsAreStudents_WithBasePerson()
        {
            TestCollections tc = new TestCollections(50);
            Student s = tc.Central;
            Assert.AreEqual(typeof(Student), s.GetType());
            Assert.AreEqual(typeof(Person), s.BasePerson.GetType());
        }

        [TestMethod]
        public void Tc_FirstCentralLast_AreDifferentElements()
        {
            TestCollections tc = new TestCollections(100);
            Assert.IsFalse(tc.First.Equals(tc.Central));
            Assert.IsFalse(tc.Central.Equals(tc.Last));
            Assert.IsFalse(tc.First.Equals(tc.Last));
        }

        [TestMethod]
        public void Tc_EveryCollection_ContainsFirstCentralLast_ByContent()
        {
            TestCollections tc = new TestCollections(1000);
            foreach (Student s in new Student[] { tc.First, tc.Central, tc.Last })
            {
                bool[] r = SearchAll(tc, s);
                for (int i = 0; i < r.Length; i++)
                    Assert.IsTrue(r[i], "Элемент не найден поиском №" + i);
            }
        }

        [TestMethod]
        public void Tc_AbsentElement_NotFoundAnywhere()
        {
            TestCollections tc = new TestCollections(1000);
            Student absent = new Student("Отсутствующий Студент Студентович", true, 99, 1, 0);
            bool[] r = SearchAll(tc, absent);
            for (int i = 0; i < r.Length; i++)
                Assert.IsFalse(r[i], "Отсутствующий элемент найден поиском №" + i);
        }

        [TestMethod]
        public void Tc_ChangedCopy_NotFound()
        {
            TestCollections tc = new TestCollections(100);
            Student copy = new Student(tc.Central);
            copy.Course = copy.Course == 6 ? 5 : copy.Course + 1;
            Assert.IsFalse(tc.ListContains(copy));
            Assert.IsFalse(tc.StringListContains(copy.ToString()));
            Assert.IsFalse(tc.DictContainsValue(copy));
            Assert.IsTrue(tc.DictContainsKey(copy.BasePerson));   // ключ (ФИО, пол, возраст) не изменился
        }

        [TestMethod]
        public void Tc_DictContainsKey_StudentInsteadOfPerson_NotFound()
        {
            TestCollections tc = new TestCollections(100);
            Assert.IsFalse(tc.DictContainsKey(new Student(tc.First)));
        }

        [TestMethod]
        public void Tc_Add_NewStudent_AddedToAllCollections()
        {
            TestCollections tc = new TestCollections(100);
            Student s = new Student(KnownName, true, 20, 3, 4);
            Assert.IsTrue(tc.Add(s));
            AssertCounts(tc, 101);
            Assert.IsTrue(ReferenceEquals(s, tc.Last));      // добавляется в конец
            bool[] r = SearchAll(tc, s);
            for (int i = 0; i < r.Length; i++) Assert.IsTrue(r[i]);
        }

        [TestMethod]
        public void Tc_Add_Duplicate_Rejected()
        {
            TestCollections tc = new TestCollections(100);
            Assert.IsFalse(tc.Add(new Student(tc.First)));
            AssertCounts(tc, 100);
        }

        [TestMethod]
        public void Tc_Add_SamePersonOtherCourse_Rejected()
        {
            // ключ словаря - подобъект Person, поэтому студент с теми же ФИО, полом и возрастом отклоняется
            TestCollections tc = new TestCollections(100);
            Student f = tc.First;
            Student other = new Student(f.Name, f.Male, f.Age, f.Course == 6 ? 1 : f.Course + 1, f.Rating);
            Assert.IsFalse(tc.Add(other));
            AssertCounts(tc, 100);
        }

        [TestMethod]
        public void Tc_Remove_Existing_RemovedFromAllCollections()
        {
            TestCollections tc = new TestCollections(100);
            Student probe = new Student(tc.Central);
            Assert.IsTrue(tc.Remove(probe));          // удаляется по содержимому
            AssertCounts(tc, 99);
            bool[] r = SearchAll(tc, probe);
            for (int i = 0; i < r.Length; i++)
                Assert.IsFalse(r[i], "Удалённый элемент найден поиском №" + i);
        }

        [TestMethod]
        public void Tc_Remove_First_And_Last()
        {
            TestCollections tc = new TestCollections(100);
            Student first = tc.First, last = tc.Last;
            Assert.IsTrue(tc.Remove(first));
            Assert.IsTrue(tc.Remove(last));
            AssertCounts(tc, 98);
            Assert.IsFalse(tc.First.Equals(first));
            Assert.IsFalse(tc.Last.Equals(last));
        }

        [TestMethod]
        public void Tc_Remove_Absent_ReturnsFalse()
        {
            TestCollections tc = new TestCollections(100);
            Assert.IsFalse(tc.Remove(new Student("Отсутствующий Студент Студентович", true, 99, 1, 0)));
            AssertCounts(tc, 100);
        }

        [TestMethod]
        public void Tc_Remove_Twice_SecondReturnsFalse()
        {
            TestCollections tc = new TestCollections(100);
            Student s = new Student(tc.Central);
            Assert.IsTrue(tc.Remove(s));
            Assert.IsFalse(tc.Remove(s));
            AssertCounts(tc, 99);
        }

        [TestMethod]
        public void Tc_AddAfterRemove_Works()
        {
            TestCollections tc = new TestCollections(100);
            Student s = new Student(tc.Central);
            tc.Remove(s);
            Assert.IsTrue(tc.Add(s));
            AssertCounts(tc, 100);
            Assert.IsTrue(SearchAll(tc, s)[4]);
        }

        [TestMethod]
        public void Tc_FindByName()
        {
            TestCollections tc = new TestCollections(100);
            Student s = new Student(KnownName, true, 20, 3, 4);
            tc.Add(s);
            Assert.IsTrue(ReferenceEquals(s, tc.FindByName(KnownName)));
            Assert.IsNull(tc.FindByName("Нет Такого Студента"));
        }

        [TestMethod]
        public void Tc_PrintCounts()
        {
            TestCollections tc = new TestCollections(100);
            string o = Capture(() => tc.PrintCounts());
            Assert.IsTrue(o.Contains("List<Student>: 100, List<string>: 100, Dictionary<Person,Student>: 100, Dictionary<string,Student>: 100"));
        }

        [TestMethod]
        public void Tc_RunSearchTest_PrintsAllMeasurements()
        {
            TestCollections tc = new TestCollections(100);
            string o = Capture(() => tc.RunSearchTest());
            Assert.IsTrue(o.Contains("Элементов в коллекциях: 100."));
            Assert.AreEqual(4, Count(o, "Элемент: "));
            Assert.IsTrue(o.Contains("Элемент: Первый"));
            Assert.IsTrue(o.Contains("Элемент: Центральный"));
            Assert.IsTrue(o.Contains("Элемент: Последний"));
            Assert.IsTrue(o.Contains("Элемент: Не входит в коллекцию"));
            Assert.AreEqual(4, Count(o, "List<Student>.Contains"));
            Assert.AreEqual(4, Count(o, "List<string>.Contains"));
            Assert.AreEqual(4, Count(o, "Dict<Person,Student>.ContainsKey"));
            Assert.AreEqual(4, Count(o, "Dict<string,Student>.ContainsKey"));
            Assert.AreEqual(4, Count(o, "Dict<Person,Student>.ContainsValue"));
        }

        [TestMethod]
        public void Tc_RunSearchTest_ResultsMatchPresence()
        {
            // три искомых элемента есть в коллекциях (15 поисков), один отсутствует (5 поисков)
            TestCollections tc = new TestCollections(100);
            string o = Capture(() => tc.RunSearchTest());
            Assert.AreEqual(15, Count(o, "(найден)"));
            Assert.AreEqual(5, Count(o, "(нет)"));
        }

        [TestMethod]
        public void Tc_RunSearchTest_TimesAreNonNegativeNumbers()
        {
            TestCollections tc = new TestCollections(100);
            string o = Capture(() => tc.RunSearchTest());
            MatchCollection m = Regex.Matches(o, @"(\d+) \((?:найден|нет)\)");
            Assert.AreEqual(20, m.Count);
        }

        [TestMethod]
        public void Tc_RunSearchTest_DoesNotChangeCollections()
        {
            TestCollections tc = new TestCollections(100);
            Capture(() => tc.RunSearchTest());
            AssertCounts(tc, 100);
        }

        [TestMethod]
        public void Tc_Elements_AreGeneratedRandomly()
        {
            TestCollections a = new TestCollections(50);
            TestCollections b = new TestCollections(50);
            Assert.IsFalse(a.First.Equals(b.First) && a.Central.Equals(b.Central) && a.Last.Equals(b.Last));
        }
    }
}
