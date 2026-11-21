using Lab10Lib;

namespace Lab11
{
    // Вспомогательные методы: ввод с проверкой, создание объектов иерархии
    public static class Helper
    {
        // Ввод целого числа из диапазона [min; max] с повтором при ошибке
        public static int ReadInt(string msg, int min, int max)
        {
            while (true)
            {
                Console.Write(msg + ": ");
                string s = Console.ReadLine();
                if (s == null) throw new EndOfStreamException();   // конец ввода
                int v;
                if (int.TryParse(s, out v) && v >= min && v <= max) return v;
                Console.WriteLine("Введите число от " + min + " до " + max + ".");
            }
        }

        // Название вида объекта для вывода
        public static string TypeTitle(Type t)
        {
            if (t == typeof(Student)) return "студент";
            if (t == typeof(Employee)) return "сотрудник";
            if (t == typeof(Teacher)) return "преподаватель";
            return "персона";
        }

        // Выбор вида объекта: 1 - Person, 2 - Student, 3 - Employee, 4 - Teacher
        public static Type ReadType()
        {
            Console.WriteLine("Вид объекта: 1 - персона, 2 - студент, 3 - сотрудник, 4 - преподаватель");
            switch (ReadInt("Выбор", 1, 4))
            {
                case 2: return typeof(Student);
                case 3: return typeof(Employee);
                case 4: return typeof(Teacher);
                default: return typeof(Person);
            }
        }

        public static Person Create(Type t)
        {
            if (t == typeof(Student)) return new Student();
            if (t == typeof(Employee)) return new Employee();
            if (t == typeof(Teacher)) return new Teacher();
            return new Person();
        }

        // Объект выбранного вида: данные вводятся с клавиатуры (Init) или формируются датчиком (RandomInit)
        public static Person ReadPerson()
        {
            Type t = ReadType();
            Person p = Create(t);
            Console.WriteLine("1 - ввод с клавиатуры, 2 - случайные данные");
            if (ReadInt("Выбор", 1, 2) == 1) p.Init(); else p.RandomInit();
            return p;
        }

        // Начальный набор из 8 случайных объектов: персона, 3 студента, 2 сотрудника, 2 преподавателя
        public static Person[] StartSet()
        {
            Type[] types = { typeof(Person), typeof(Student), typeof(Student), typeof(Student),
                             typeof(Employee), typeof(Employee), typeof(Teacher), typeof(Teacher) };
            Person[] a = new Person[types.Length];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = Create(types[i]);
                a[i].RandomInit();
            }
            return a;
        }
    }
}
