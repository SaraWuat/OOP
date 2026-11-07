using System;
using System.Collections;
using System.Text;

namespace Lab10Lib
{
    // ===================== Часть 3: интерфейс ввода объектов =====================
    public interface IInit
    {
        void Init();        // ввод с клавиатуры
        void RandomInit();  // ввод с помощью ДСЧ
    }

    // Вспомогательные функции ввода (повторяются много раз)
    public static class Input
    {
        public static string Str(string msg)
        {
            Console.Write(msg + ": ");
            string s = Console.ReadLine();
            return s == null ? "" : s;
        }

        public static int Int(string msg)
        {
            int v;
            if (!int.TryParse(Str(msg), out v)) v = 0;
            return v;
        }

        public static double Dbl(string msg)
        {
            double v;
            if (!double.TryParse(Str(msg), out v)) v = 0;
            return v;
        }
    }

    // Проверка ограничений в свойствах (используется во всех классах)
    public static class Check
    {
        // Значение в диапазоне [min; max], иначе значение по умолчанию
        public static int Range(int v, int min, int max, int def)
        {
            return (v >= min && v <= max) ? v : def;
        }

        public static double Range(double v, double min, double max, double def)
        {
            return (v >= min && v <= max) ? v : def;
        }

        // Непустая строка, иначе значение по умолчанию
        public static string Text(string v, string def)
        {
            return string.IsNullOrWhiteSpace(v) ? def : v;
        }
    }

    // ===================== Часть 1: иерархия классов =====================

    // Кафедра (ссылочное поле для демонстрации клонирования)
    public class Department
    {
        public string Name { get; set; }

        public Department(string n) { Name = n; }

        public override bool Equals(object obj)
        {
            Department d = obj as Department;
            return d != null && d.Name == Name;
        }

        public override int GetHashCode() { return Name.GetHashCode(); }
    }

    // Персона - базовый класс иерархии
    public class Person : IInit, IComparable, ICloneable
    {
        protected static Random rnd = new Random();
        private static string[] surnames = { "Иванов", "Петров", "Сидоров", "Смирнов", "Кузнецов", "Попов", "Волков",
            "Соколов", "Лебедев", "Козлов", "Новиков", "Морозов", "Фёдоров", "Михайлов", "Беляев", "Орлов",
            "Никитин", "Захаров", "Зайцев", "Соловьёв", "Борисов", "Яковлев", "Григорьев", "Романов" };
        private static string[] namesM = { "Алексей", "Иван", "Матвей", "Дмитрий", "Сергей", "Андрей", "Николай",
            "Михаил", "Артём", "Максим", "Егор", "Кирилл", "Роман", "Владимир", "Павел" };
        private static string[] namesF = { "Анна", "Мария", "Елена", "Ольга", "Ирина", "Наталья", "Екатерина",
            "Татьяна", "Светлана", "Юлия", "Дарья", "Алина", "Виктория", "Ксения", "Полина" };
        // Отчества в мужской форме (женская получается заменой окончания "ич" на "на")
        private static string[] patr = { "Иванович", "Петрович", "Сергеевич", "Александрович", "Дмитриевич",
            "Андреевич", "Николаевич", "Владимирович", "Михайлович", "Алексеевич", "Олегович", "Игоревич",
            "Викторович", "Павлович" };

        protected string name;  // ФИО
        protected bool male;    // пол: true - мужской
        protected int age;      // возраст

        public string Name
        {
            get { return name; }
            set { name = Check.Text(value, "Без имени"); }
        }

        public bool Male
        {
            get { return male; }
            set { male = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = Check.Range(value, 1, 120, 1); }
        }

        public Person() { name = "Без имени"; male = true; age = 1; }

        public Person(string n, bool m, int a)
        {
            Name = n; Male = m; Age = a;
        }

        public Person(Person p) : this(p.name, p.male, p.age) { }

        // Виртуальный метод: версия выбирается по типу объекта
        // Общая часть описания персоны (используется в Show производных классов)
        protected string Descr()
        {
            return name + ", " + (male ? "М" : "Ж") + ", " + age + " л.";
        }

        public virtual void Show()
        {
            Console.WriteLine("Персона: " + Descr());
        }

        // Невиртуальный метод: версия выбирается по типу ссылки
        public void Print()
        {
            Console.WriteLine("[Print Person] " + name);
        }

        public virtual void Init()
        {
            Name = Input.Str("ФИО");
            Male = Input.Str("Пол (м/ж)").Trim().ToLower().StartsWith("м");
            Age = Input.Int("Возраст");
        }

        public virtual void RandomInit()
        {
            male = rnd.Next(2) == 0;
            string s = surnames[rnd.Next(surnames.Length)] + (male ? "" : "а");
            string[] n = male ? namesM : namesF;
            string o = patr[rnd.Next(patr.Length)];
            if (!male) o = o.Substring(0, o.Length - 2) + "на";
            name = s + " " + n[rnd.Next(n.Length)] + " " + o;
            age = rnd.Next(18, 71);
        }

        public override bool Equals(object obj)
        {
            Person p = obj as Person;
            if (p == null || p.GetType() != GetType()) return false;
            return name == p.name && male == p.male && age == p.age;
        }

        public override int GetHashCode() { return name.GetHashCode() ^ age; }

        // IComparable: сортировка по ФИО
        public int CompareTo(object obj)
        {
            Person p = (Person)obj;
            return string.Compare(name, p.name);
        }

        // Поверхностное копирование
        public Person ShallowCopy() { return (Person)MemberwiseClone(); }

        // Глубокое копирование (ICloneable)
        public virtual object Clone() { return new Person(this); }
    }

    // Студент
    public class Student : Person
    {
        protected int course;      // курс
        protected double rating;   // средний балл

        public int Course
        {
            get { return course; }
            set { course = Check.Range(value, 1, 6, 1); }
        }

        public double Rating
        {
            get { return rating; }
            set { rating = Check.Range(value, 0.0, 5.0, 0.0); }
        }

        public Student() : base() { course = 1; rating = 0; }

        public Student(string n, bool m, int a, int c, double r) : base(n, m, a)
        {
            Course = c; Rating = r;
        }

        public Student(Student s) : base(s) { course = s.course; rating = s.rating; }

        public override void Show()
        {
            Console.WriteLine("Студент: " + Descr());
            Console.WriteLine("         " + course + " курс, балл: " + rating.ToString("F2"));
        }

        public new void Print()
        {
            Console.WriteLine("[Print Student] " + name + ", " + course + " курс");
        }

        public override void Init()
        {
            base.Init();
            Course = Input.Int("Курс (1-6)");
            Rating = Input.Dbl("Средний балл (0-5)");
        }

        public override void RandomInit()
        {
            base.RandomInit();
            age = rnd.Next(17, 26);
            course = rnd.Next(1, 7);
            rating = Math.Round(2 + rnd.NextDouble() * 3, 2);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj)) return false;
            Student s = (Student)obj;
            return course == s.course && rating == s.rating;
        }

        public override int GetHashCode() { return base.GetHashCode() ^ course; }

        public override object Clone() { return new Student(this); }
    }

    // Сотрудник
    public class Employee : Person
    {
        private static string[] positions = { "Инженер", "Лаборант", "Бухгалтер", "Секретарь", "Программист",
            "Электрик", "Механик", "Менеджер", "Администратор", "Технолог", "Аналитик", "Юрисконсульт" };

        protected string position;  // должность
        protected int experience;   // стаж, лет

        public string Position
        {
            get { return position; }
            set { position = Check.Text(value, "Не указана"); }
        }

        public int Experience
        {
            get { return experience; }
            set { experience = Check.Range(value, 0, 60, 0); }
        }

        public Employee() : base() { position = "Не указана"; experience = 0; }

        public Employee(string n, bool m, int a, string pos, int exp) : base(n, m, a)
        {
            Position = pos; Experience = exp;
        }

        public Employee(Employee e) : base(e) { position = e.position; experience = e.experience; }

        public override void Show()
        {
            Console.WriteLine("Сотрудник: " + Descr());
            Console.WriteLine("           " + position + ", стаж: " + experience + " л.");
        }

        public new void Print()
        {
            Console.WriteLine("[Print Employee] " + name + ", " + position);
        }

        public override void Init()
        {
            base.Init();
            Position = Input.Str("Должность");
            Experience = Input.Int("Стаж");
        }

        public override void RandomInit()
        {
            base.RandomInit();
            age = rnd.Next(22, 66);
            position = positions[rnd.Next(positions.Length)];
            experience = rnd.Next(0, age - 21);  // стаж не больше, чем возраст - 22
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj)) return false;
            Employee e = (Employee)obj;
            return position == e.position && experience == e.experience;
        }

        public override int GetHashCode() { return base.GetHashCode() ^ experience; }

        public override object Clone() { return new Employee(this); }
    }

    // Преподаватель (сотрудник, у которого есть кафедра и степень)
    public class Teacher : Employee
    {
        private static string[] depts = { "ИТАС", "Физика", "Математика", "Химия", "Механика",
            "Электротехника", "Экономика" };
        private static string[] degrees = { "нет", "к.т.н.", "д.т.н.", "к.ф.-м.н.", "д.ф.-м.н." };

        protected Department dept;  // кафедра
        protected string degree;    // учёная степень

        public Department Dept
        {
            get { return dept; }
            set { dept = value == null ? new Department("Не указана") : value; }
        }

        public string Degree
        {
            get { return degree; }
            set { degree = Check.Text(value, "нет"); }
        }

        public Teacher() : base()
        {
            position = "Преподаватель";
            dept = new Department("Не указана");
            degree = "нет";
        }

        public Teacher(string n, bool m, int a, int exp, string d, string deg)
            : base(n, m, a, "Преподаватель", exp)
        {
            Dept = new Department(d); Degree = deg;
        }

        // Копирование: кафедра копируется как новый объект
        public Teacher(Teacher t) : base(t)
        {
            dept = new Department(t.dept.Name);
            degree = t.degree;
        }

        public override void Show()
        {
            Console.WriteLine("Преподаватель: " + Descr());
            Console.WriteLine("               стаж: " + experience + " л., степень: " + degree);
            Console.WriteLine("               кафедра " + dept.Name);
        }

        public new void Print()
        {
            Console.WriteLine("[Print Teacher] " + name + ", каф. " + dept.Name);
        }

        public override void Init()
        {
            base.Init();
            position = "Преподаватель";
            Dept = new Department(Input.Str("Кафедра"));
            Degree = Input.Str("Учёная степень");
        }

        public override void RandomInit()
        {
            base.RandomInit();
            position = "Преподаватель";
            dept = new Department(depts[rnd.Next(depts.Length)]);
            degree = degrees[rnd.Next(degrees.Length)];
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj)) return false;
            Teacher t = (Teacher)obj;
            return dept.Equals(t.dept) && degree == t.degree;
        }

        public override int GetHashCode() { return base.GetHashCode() ^ dept.GetHashCode(); }

        public override object Clone() { return new Teacher(this); }
    }

    // Класс вне иерархии, реализующий IInit (часть 3, п. 5)
    public class Group : IInit
    {
        private static Random rnd = new Random();
        private int number;  // номер группы
        private int size;    // число студентов

        public int Number
        {
            get { return number; }
            set { number = Check.Range(value, 1, int.MaxValue, 1); }
        }

        public int Size
        {
            get { return size; }
            set { size = Check.Range(value, 1, int.MaxValue, 1); }
        }

        public void Init()
        {
            Number = Input.Int("Номер группы");
            Size = Input.Int("Число студентов");
        }

        public void RandomInit()
        {
            number = rnd.Next(1, 10);
            size = rnd.Next(10, 31);
        }

        public void Show()
        {
            Console.WriteLine("Группа №" + number + ", студентов: " + size);
        }
    }

    // Компараторы (IComparer) - сортировка по другим критериям
    public class SortByAge : IComparer
    {
        int IComparer.Compare(object a, object b)
        {
            return ((Person)a).Age.CompareTo(((Person)b).Age);
        }
    }

    public class SortByNameDesc : IComparer
    {
        int IComparer.Compare(object a, object b)
        {
            return string.Compare(((Person)b).Name, ((Person)a).Name);
        }
    }
}