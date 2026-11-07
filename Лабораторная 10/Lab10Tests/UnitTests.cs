using Lab10Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Lab10Tests
{
    [TestClass]
    public class UnitTests
    {
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

        // ---------- Check ----------

        [TestMethod]
        public void Check_Range_InsideAndOutside()
        {
            Assert.AreEqual(5, Check.Range(5, 1, 10, 1));
            Assert.AreEqual(1, Check.Range(0, 1, 10, 1));
            Assert.AreEqual(1, Check.Range(11, 1, 10, 1));
            Assert.AreEqual(4.5, Check.Range(4.5, 0.0, 5.0, 0.0));
            Assert.AreEqual(0.0, Check.Range(6.0, 0.0, 5.0, 0.0));
        }

        [TestMethod]
        public void Check_Text_EmptyAndNormal()
        {
            Assert.AreEqual("abc", Check.Text("abc", "def"));
            Assert.AreEqual("def", Check.Text("", "def"));
            Assert.AreEqual("def", Check.Text("   ", "def"));
            Assert.AreEqual("def", Check.Text(null, "def"));
        }

        // ---------- Свойства ----------

        [TestMethod]
        public void Person_Age_Limits()
        {
            Person p = new Person();
            p.Age = 30;
            Assert.AreEqual(30, p.Age);
            p.Age = 0;
            Assert.AreEqual(1, p.Age);
            p.Age = 200;
            Assert.AreEqual(1, p.Age);
        }

        [TestMethod]
        public void Person_Name_Empty()
        {
            Person p = new Person();
            p.Name = "";
            Assert.AreEqual("Без имени", p.Name);
            p.Name = "Иванов";
            Assert.AreEqual("Иванов", p.Name);
        }

        [TestMethod]
        public void Student_Course_Limits()
        {
            Student s = new Student();
            s.Course = 3;
            Assert.AreEqual(3, s.Course);
            s.Course = 7;
            Assert.AreEqual(1, s.Course);
            s.Course = 0;
            Assert.AreEqual(1, s.Course);
        }

        [TestMethod]
        public void Student_Rating_Limits()
        {
            Student s = new Student();
            s.Rating = 4.5;
            Assert.AreEqual(4.5, s.Rating);
            s.Rating = 6;
            Assert.AreEqual(0.0, s.Rating);
            s.Rating = -1;
            Assert.AreEqual(0.0, s.Rating);
        }

        [TestMethod]
        public void Employee_Experience_And_Position()
        {
            Employee e = new Employee();
            e.Experience = 10;
            Assert.AreEqual(10, e.Experience);
            e.Experience = -5;
            Assert.AreEqual(0, e.Experience);
            e.Experience = 61;
            Assert.AreEqual(0, e.Experience);
            e.Position = null;
            Assert.AreEqual("Не указана", e.Position);
        }

        [TestMethod]
        public void Teacher_Dept_And_Degree()
        {
            Teacher t = new Teacher();
            t.Dept = null;
            Assert.AreEqual("Не указана", t.Dept.Name);
            t.Degree = " ";
            Assert.AreEqual("нет", t.Degree);
        }

        [TestMethod]
        public void Group_Limits()
        {
            Group g = new Group();
            g.Number = 0;
            g.Size = -3;
            Assert.AreEqual(1, g.Number);
            Assert.AreEqual(1, g.Size);
            g.Number = 5;
            Assert.AreEqual(5, g.Number);
        }

        // ---------- Конструкторы ----------

        [TestMethod]
        public void Person_DefaultConstructor()
        {
            Person p = new Person();
            Assert.AreEqual("Без имени", p.Name);
            Assert.IsTrue(p.Male);
            Assert.AreEqual(1, p.Age);
        }

        [TestMethod]
        public void Student_ParameterConstructor()
        {
            Student s = new Student("Иванов", true, 19, 2, 4.5);
            Assert.AreEqual("Иванов", s.Name);
            Assert.AreEqual(19, s.Age);
            Assert.AreEqual(2, s.Course);
            Assert.AreEqual(4.5, s.Rating);
        }

        [TestMethod]
        public void Student_CopyConstructor()
        {
            Student s = new Student("Иванов", true, 19, 2, 4.5);
            Student c = new Student(s);
            Assert.IsTrue(s.Equals(c));
            c.Course = 3;
            Assert.AreEqual(2, s.Course);
        }

        [TestMethod]
        public void Teacher_CopyConstructor_DeptIsIndependent()
        {
            Teacher t = new Teacher("Петров", true, 45, 20, "ИТАС", "к.т.н.");
            Teacher c = new Teacher(t);
            Assert.IsTrue(t.Equals(c));
            Assert.IsFalse(ReferenceEquals(t.Dept, c.Dept));
            Assert.AreEqual("Преподаватель", c.Position);
        }

        // ---------- Equals ----------

        [TestMethod]
        public void Equals_SameFields()
        {
            Person a = new Person("Иванов", true, 30);
            Person b = new Person("Иванов", true, 30);
            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void Equals_DifferentFields()
        {
            Person a = new Person("Иванов", true, 30);
            Person b = new Person("Иванов", true, 31);
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void Equals_DifferentTypes()
        {
            Person p = new Person("Иванов", true, 30);
            Employee e = new Employee("Иванов", true, 30, "Инженер", 5);
            Assert.IsFalse(p.Equals(e));
            Assert.IsFalse(e.Equals(p));
        }

        [TestMethod]
        public void Equals_Null()
        {
            Person p = new Person("Иванов", true, 30);
            Assert.IsFalse(p.Equals(null));
        }

        [TestMethod]
        public void Equals_Student_DifferentCourse()
        {
            Student a = new Student("Иванов", true, 19, 2, 4.5);
            Student b = new Student("Иванов", true, 19, 3, 4.5);
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void Equals_Teacher_DifferentDept()
        {
            Teacher a = new Teacher("Петров", true, 45, 20, "ИТАС", "к.т.н.");
            Teacher b = new Teacher("Петров", true, 45, 20, "Физика", "к.т.н.");
            Assert.IsFalse(a.Equals(b));
        }

        // ---------- Виртуальные и невиртуальные методы ----------

        [TestMethod]
        public void Show_IsVirtual()
        {
            Person p = new Teacher("Петров", true, 45, 20, "ИТАС", "к.т.н.");
            string s = Capture(() => p.Show());
            Assert.IsTrue(s.Contains("Преподаватель"));
            Assert.IsTrue(s.Contains("ИТАС"));
        }

        [TestMethod]
        public void Show_Person()
        {
            Person p = new Person("Иванов", true, 30);
            string s = Capture(() => p.Show());
            Assert.IsTrue(s.Contains("Персона"));
            Assert.IsTrue(s.Contains("Иванов"));
        }

        [TestMethod]
        public void Print_IsNotVirtual()
        {
            Person p = new Student("Иванов", true, 19, 2, 4.5);
            string s = Capture(() => p.Print());
            Assert.IsTrue(s.Contains("[Print Person]"));
            Assert.IsFalse(s.Contains("[Print Student]"));
        }

        [TestMethod]
        public void Print_ThroughOwnType()
        {
            Student st = new Student("Иванов", true, 19, 2, 4.5);
            string s = Capture(() => st.Print());
            Assert.IsTrue(s.Contains("[Print Student]"));
        }

        // ---------- Init (ввод с клавиатуры) ----------

        [TestMethod]
        public void Person_Init()
        {
            Person p = new Person();
            SetInput("Петрова Анна\nж\n30\n");
            p.Init();
            Assert.AreEqual("Петрова Анна", p.Name);
            Assert.IsFalse(p.Male);
            Assert.AreEqual(30, p.Age);
        }

        [TestMethod]
        public void Student_Init()
        {
            Student s = new Student();
            SetInput("Иванов Иван\nм\n20\n2\n4\n");
            s.Init();
            Assert.AreEqual("Иванов Иван", s.Name);
            Assert.IsTrue(s.Male);
            Assert.AreEqual(20, s.Age);
            Assert.AreEqual(2, s.Course);
            Assert.AreEqual(4.0, s.Rating);
        }

        [TestMethod]
        public void Teacher_Init_ThroughInterface()
        {
            IInit x = new Teacher();
            SetInput("Сидоров Сергей\nм\n50\nинженер\n25\nИТАС\nд.т.н.\n");
            x.Init();
            Teacher t = (Teacher)x;
            Assert.AreEqual("Сидоров Сергей", t.Name);
            Assert.AreEqual(25, t.Experience);
            Assert.AreEqual("Преподаватель", t.Position);
            Assert.AreEqual("ИТАС", t.Dept.Name);
            Assert.AreEqual("д.т.н.", t.Degree);
        }

        [TestMethod]
        public void Group_Init()
        {
            Group g = new Group();
            SetInput("5\n25\n");
            g.Init();
            Assert.AreEqual(5, g.Number);
            Assert.AreEqual(25, g.Size);
        }

        // ---------- RandomInit ----------

        [TestMethod]
        public void Student_RandomInit_InRange()
        {
            for (int i = 0; i < 200; i++)
            {
                Student s = new Student();
                s.RandomInit();
                Assert.IsTrue(s.Age >= 17 && s.Age <= 25);
                Assert.IsTrue(s.Course >= 1 && s.Course <= 6);
                Assert.IsTrue(s.Rating >= 2.0 && s.Rating <= 5.0);
                Assert.IsFalse(string.IsNullOrWhiteSpace(s.Name));
            }
        }

        [TestMethod]
        public void Employee_RandomInit_ExperienceLessThanAge()
        {
            for (int i = 0; i < 200; i++)
            {
                Employee e = new Employee();
                e.RandomInit();
                Assert.IsTrue(e.Age >= 22 && e.Age <= 65);
                Assert.IsTrue(e.Experience >= 0 && e.Experience <= e.Age - 22);
            }
        }

        [TestMethod]
        public void Teacher_RandomInit()
        {
            for (int i = 0; i < 100; i++)
            {
                Teacher t = new Teacher();
                t.RandomInit();
                Assert.AreEqual("Преподаватель", t.Position);
                Assert.IsFalse(string.IsNullOrWhiteSpace(t.Dept.Name));
                Assert.IsFalse(string.IsNullOrWhiteSpace(t.Degree));
            }
        }

        [TestMethod]
        public void Group_RandomInit()
        {
            for (int i = 0; i < 100; i++)
            {
                Group g = new Group();
                g.RandomInit();
                Assert.IsTrue(g.Number >= 1 && g.Number <= 9);
                Assert.IsTrue(g.Size >= 10 && g.Size <= 30);
            }
        }

        [TestMethod]
        public void IInit_Array_RandomInit()
        {
            IInit[] arr = { new Person(), new Student(), new Employee(), new Teacher(), new Group() };
            foreach (IInit x in arr) x.RandomInit();
            Assert.IsTrue(((Person)arr[0]).Age >= 18);
            Assert.IsTrue(((Group)arr[4]).Size >= 10);
        }

        // ---------- Сортировка и поиск ----------

        static Person[] NewSample()
        {
            return new Person[]
            {
                new Person("В", true, 30),
                new Person("А", true, 50),
                new Person("Б", true, 20)
            };
        }

        [TestMethod]
        public void CompareTo_ByName()
        {
            Person a = new Person("Анна", false, 20);
            Person b = new Person("Борис", true, 20);
            Assert.IsTrue(a.CompareTo(b) < 0);
            Assert.IsTrue(b.CompareTo(a) > 0);
            Assert.AreEqual(0, a.CompareTo(new Person("Анна", true, 99)));
        }

        [TestMethod]
        public void Sort_ByName_IComparable()
        {
            Person[] a = NewSample();
            Array.Sort(a);
            Assert.AreEqual("А", a[0].Name);
            Assert.AreEqual("Б", a[1].Name);
            Assert.AreEqual("В", a[2].Name);
        }

        [TestMethod]
        public void Sort_ByAge_IComparer()
        {
            Person[] a = NewSample();
            Array.Sort(a, new SortByAge());
            Assert.AreEqual(20, a[0].Age);
            Assert.AreEqual(30, a[1].Age);
            Assert.AreEqual(50, a[2].Age);
        }

        [TestMethod]
        public void Sort_ByNameDesc_IComparer()
        {
            Person[] a = NewSample();
            Array.Sort(a, new SortByNameDesc());
            Assert.AreEqual("В", a[0].Name);
            Assert.AreEqual("Б", a[1].Name);
            Assert.AreEqual("А", a[2].Name);
        }

        [TestMethod]
        public void BinarySearch_ByName()
        {
            Person[] a = NewSample();
            Array.Sort(a);
            Assert.AreEqual(1, Array.BinarySearch(a, new Person("Б", true, 1)));
            Assert.IsTrue(Array.BinarySearch(a, new Person("Г", true, 1)) < 0);
        }

        [TestMethod]
        public void BinarySearch_ByAge_WithComparer()
        {
            Person[] a = NewSample();
            Array.Sort(a, new SortByAge());
            int i = Array.BinarySearch(a, new Person("", true, 30), new SortByAge());
            Assert.AreEqual(1, i);
            Assert.AreEqual("В", a[i].Name);
            Assert.IsTrue(Array.BinarySearch(a, new Person("", true, 99), new SortByAge()) < 0);
        }

        // ---------- Клонирование и поверхностное копирование ----------

        [TestMethod]
        public void Clone_ReturnsSameTypeAndEqualObject()
        {
            Student s = new Student("Иванов", true, 19, 2, 4.5);
            object c = s.Clone();
            Assert.IsTrue(c is Student);
            Assert.IsTrue(s.Equals(c));
            Assert.IsFalse(ReferenceEquals(s, c));
        }

        [TestMethod]
        public void Clone_Teacher_DeepCopy()
        {
            Teacher t = new Teacher("Петров", true, 45, 20, "ИТАС", "к.т.н.");
            Teacher c = (Teacher)t.Clone();
            t.Dept.Name = "ИЗМЕНЕНА";
            Assert.AreEqual("ИТАС", c.Dept.Name);
        }

        [TestMethod]
        public void ShallowCopy_Teacher_SharesDept()
        {
            Teacher t = new Teacher("Петров", true, 45, 20, "ИТАС", "к.т.н.");
            Teacher sh = (Teacher)t.ShallowCopy();
            Assert.IsTrue(ReferenceEquals(t.Dept, sh.Dept));
            t.Dept.Name = "ИЗМЕНЕНА";
            Assert.AreEqual("ИЗМЕНЕНА", sh.Dept.Name);
        }

        [TestMethod]
        public void Department_Equals()
        {
            Assert.IsTrue(new Department("ИТАС").Equals(new Department("ИТАС")));
            Assert.IsFalse(new Department("ИТАС").Equals(new Department("Физика")));
            Assert.IsFalse(new Department("ИТАС").Equals(null));
        }
    }
}