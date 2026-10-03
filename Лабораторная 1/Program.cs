using System;
public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Задача 1 ===");
        RunTask1();
        Console.WriteLine("\n=== Задача 2 ===");
        RunTask2();
        Console.WriteLine("\n=== Задача 3 ===");
        RunTask3();
    }

    private static double ReadDouble(string prompt)
    {
        double value;
        bool isParsed;
        do
        {
            Console.Write(prompt);
            isParsed = double.TryParse(Console.ReadLine(), out value);
            if (!isParsed)
            {
                Console.WriteLine("Некорректный ввод, ожидается число. Повторите ввод.");
            }
        } while (!isParsed);
        return value;
    }

    private static void RunTask1()
    {
        double initialN = ReadDouble("n?");
        double initialM = ReadDouble("m?");
        double n = initialN;
        double m = initialM;
        double result1 = m - ++n;
        Console.WriteLine($"m={m}  n={n}  m-++n={result1}");
        n = initialN;
        m = initialM;
        bool result2 = m++ > --n;
        Console.WriteLine($"m={m}  n={n}  m++>--n={result2}");
        n = initialN;
        m = initialM;
        bool result3 = m-- < ++n;
        Console.WriteLine($"m={m}  n={n}  m--<++n={result3}");
        double x = ReadDouble("x?");
        if (x >= -2 && x <= 0)
        {
            double result4 = Math.Asin(Math.Abs(x + 1));
            Console.WriteLine($"x={x}  arcsin(|x+1|)={result4}");
        }
        else
        {
            Console.WriteLine("arcsin(|x+1|): Нельзя вычислить (|x+1| > 1)");
        }
    }

    private static void RunTask2()
    {
        double x1 = ReadDouble("X1? ");
        double y1 = ReadDouble("Y1? ");
        bool inTriangle = (x1 >= 0)
                           && (y1 <= 5 - 0.5 * x1)
                           && (y1 >= -5 + 0.5 * x1);
        bool inCircle = Math.Pow(x1 - 5, 2) + Math.Pow(y1, 2) <= 25;
        bool inRegion = inTriangle || inCircle;
        Console.WriteLine($"Точка ({x1}; {y1}) принадлежит области: {inRegion}");
    }

    private static void RunTask3()
    {
        double aDouble = 1000;
        double bDouble = 0.0001;
        double numeratorDouble = Math.Pow(aDouble - bDouble, 3)
                                 - (Math.Pow(aDouble, 3) + 3 * aDouble * Math.Pow(bDouble, 2));
        double denominatorDouble = -3 * Math.Pow(aDouble, 2) * bDouble - Math.Pow(bDouble, 3);
        double resultDouble = numeratorDouble / denominatorDouble;
        float aFloat = 1000f;
        float bFloat = 0.0001f;
        float numeratorFloat = (float)(Math.Pow(aFloat - bFloat, 3)
                                       - (Math.Pow(aFloat, 3) + 3 * aFloat * Math.Pow(bFloat, 2)));
        float denominatorFloat = (float)(-3 * Math.Pow(aFloat, 2) * bFloat - Math.Pow(bFloat, 3));
        float resultFloat = numeratorFloat / denominatorFloat;
        Console.WriteLine($"double: a={aDouble}, b={bDouble}, результат={resultDouble}");
        Console.WriteLine($"float:  a={aFloat}, b={bFloat}, результат={resultFloat}");
        Console.WriteLine("Ожидаемое (математически точное) значение: 1");
    }
}