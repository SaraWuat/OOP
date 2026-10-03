using System;

internal class Program
{
    private const double RangeStart = 0.1;
    private const double RangeEnd = 1.0;
    private const int PointsCount = 10; // число точек x (k = 10)
    private const int TermsCount = 10; // число слагаемых для SN (n = 10)
    private const double Epsilon = 0.0001;
    private const int MaxIterations = 1000; // защита от бесконечного цикла
    private static readonly double LnThree = Math.Log(3);

    public static void Main()
    {
        var step = (RangeEnd - RangeStart) / (PointsCount - 1); // шаг
        Console.WriteLine("Вычисление функции y = 3^x");
        for (var pointIndex = 0; pointIndex < PointsCount; pointIndex++)
        {
            var argument = RangeStart + pointIndex * step; // текущий X
            var sumByCount = CalculateSumByCount(argument); // Sn
            var sumByEpsilon = CalculateSumByEpsilon(argument); // до точности эпсилон
            var exactValue = GetExactValue(argument); // значение функции
            PrintRow(argument, sumByCount, sumByEpsilon, exactValue);
        }
    }

    private static double CalculateSumByCount(double argument)
    {
        var sum = 1.0; // первое слагаемое ряда Маклорена
        var term = 1.0;
        for (var index = 0; index < TermsCount; index++)
        {
            term = GetNextTerm(term, argument, index); // новое слагаемое
            sum += term;
        }
        return sum; // накопленная сумма
    }

    private static double CalculateSumByEpsilon(double argument)
    {
        var sum = 1.0;
        var term = 1.0;
        var index = 0; // счётчик слагаемых
        while (Math.Abs(term) >= Epsilon && index < MaxIterations)
        {
            term = GetNextTerm(term, argument, index);
            sum += term;
            index++;
        }
        return sum; // накопленная сумма
    }

    private static double GetNextTerm(double term, double argument, int index)
    {
        return term * LnThree * argument / (index + 1); // новое слагаемое по рекуррентной формуле
    }

    private static double GetExactValue(double argument)
    {
        return Math.Pow(3, argument); // значение функции в точке x
    }

    private static void PrintRow(
        double argument,
        double sumByCount,
        double sumByEpsilon,
        double exactValue)
    {
        Console.WriteLine(
            $"X={argument:F1}\tSN={sumByCount:F8}\tSE={sumByEpsilon:F8}\tY={exactValue:F8}");
    }
}