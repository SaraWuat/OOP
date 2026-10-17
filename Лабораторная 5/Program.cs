using System;
using System.Text.RegularExpressions;

internal class Program
{
    private const int MaxSize = 20;
    private const int MinRandomValue = 1;
    private const int MaxRandomValue = 9;
    private static readonly char[] WordSeparators = { ' ', ',', ';', ':' };
    private static readonly char[] SentenceTerminators = { '.', '!', '?' };
    private static readonly Random Generator = new Random();
    private static readonly string[] TestTexts =
    {
        "В лесу родилась елочка. В лесу она росла. Зимой и летом стройная, зеленая была.",
        "Раз, два; три: четыре.  Пять   шесть!  Семь восемь девять?",
        "Один.",
        "Привет мир! Как дела, друг? Всё хорошо. Пока!"
    };

    public static void Main()
    {
        var matrix = new int[0, 0];
        var rows = new int[0][];
        var text = string.Empty;
        var choice = 0;
        do
        {
            choice = ReadMenuChoice(
                "ГЛАВНОЕ МЕНЮ",
                "Выход",
                "Двумерный массив",
                "Рваный массив",
                "Строки");
            switch (choice)
            {
                case 1:
                    RunMatrixMenu(ref matrix);
                    break;
                case 2:
                    RunJaggedMenu(ref rows);
                    break;
                case 3:
                    RunTextMenu(ref text);
                    break;
            }
        }
        while (choice != 0);
    }

    private static void RunMatrixMenu(ref int[,] matrix)
    {
        var choice = 0;
        do
        {
            choice = ReadMenuChoice(
                "ДВУМЕРНЫЙ МАССИВ",
                "Назад",
                "Создать массив вручную",
                "Создать массив случайно",
                "Вывести массив",
                "Добавить строку с заданным номером");
            try
            {
                switch (choice)
                {
                    case 1:
                        matrix = CreateMatrix(isManual: true);
                        PrintArray(matrix);
                        break;
                    case 2:
                        matrix = CreateMatrix(isManual: false);
                        PrintArray(matrix);
                        break;
                    case 3:
                        PrintArray(matrix);
                        break;
                    case 4:
                        AddRowInteractively(ref matrix);
                        PrintArray(matrix, title: "Результат");
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                PrintError(ex.Message);
            }
        }
        while (choice != 0);
    }

    private static void RunJaggedMenu(ref int[][] rows)
    {
        var choice = 0;
        do
        {
            choice = ReadMenuChoice(
                "РВАНЫЙ МАССИВ",
                "Назад",
                "Создать массив вручную",
                "Создать массив случайно",
                "Вывести массив",
                "Удалить самую длинную строку");
            try
            {
                switch (choice)
                {
                    case 1:
                        rows = CreateJagged(isManual: true);
                        PrintArray(rows);
                        break;
                    case 2:
                        rows = CreateJagged(isManual: false);
                        PrintArray(rows);
                        break;
                    case 3:
                        PrintArray(rows);
                        break;
                    case 4:
                        DeleteLongestRow(ref rows, out var removedNumber);
                        Console.WriteLine($"Удалена самая длинная строка (номер {removedNumber}).");
                        PrintArray(rows, title: "Результат");
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                PrintError(ex.Message);
            }
        }
        while (choice != 0);
    }

    private static void RunTextMenu(ref string text)
    {
        var choice = 0;
        do
        {
            choice = ReadMenuChoice(
                "СТРОКИ",
                "Назад",
                "Ввести строку с клавиатуры",
                "Выбрать строку из тестовых",
                "Показать строку",
                "Перевернуть каждое нечётное предложение");
            try
            {
                switch (choice)
                {
                    case 1:
                        text = ReadText("Введите строку: ");
                        PrintText(text);
                        break;
                    case 2:
                        text = ChooseTestText();
                        PrintText(text);
                        break;
                    case 3:
                        PrintText(text);
                        break;
                    case 4:
                        ThrowIfEmpty(string.IsNullOrWhiteSpace(text), "Строка пуста, сначала введите её");
                        PrintText(ReverseOddSentences(text), "Результат");
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                PrintError(ex.Message);
            }
        }
        while (choice != 0);
    }

    private static int[,] CreateMatrix(bool isManual)
    {
        var rowsCount = ReadInt($"Количество строк (1..{MaxSize}): ", 1, MaxSize);
        var columnsCount = ReadInt($"Количество столбцов (1..{MaxSize}): ", 1, MaxSize);
        var matrix = new int[rowsCount, columnsCount];
        for (var i = 0; i < rowsCount; i++)
        {
            var row = CreateRow(columnsCount, isManual, $"Строка {i + 1}");
            for (var j = 0; j < columnsCount; j++)
                matrix[i, j] = row[j];
        }
        return matrix;
    }

    private static int[][] CreateJagged(bool isManual)
    {
        var rowsCount = ReadInt($"Количество строк (1..{MaxSize}): ", 1, MaxSize);
        var rows = new int[rowsCount][];
        for (var i = 0; i < rowsCount; i++)
        {
            var length = ReadInt($"Длина строки {i + 1} (1..{MaxSize}): ", 1, MaxSize);
            rows[i] = CreateRow(length, isManual, $"Строка {i + 1}");
        }
        return rows;
    }

    private static int[] CreateRow(int length, bool isManual, string title = "Строка")
    {
        if (isManual)
            return ReadRow(length, title);
        var row = new int[length];
        for (var i = 0; i < length; i++)
            row[i] = Generator.Next(MinRandomValue, MaxRandomValue + 1);
        return row;
    }

    private static int[] ReadRow(int length, string title)
    {
        while (true)
        {
            var parts = ReadText($"{title} (чисел: {length}, через пробел): ")
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != length)
            {
                PrintError($"количество чисел должно быть равно {length}");
                continue;
            }
            var row = new int[length];
            if (TryConvertAll(parts, row))
                return row;
        }
    }

    private static void AddRowInteractively(ref int[,] matrix)
    {
        ThrowIfEmpty(matrix.Length == 0, "Массив пуст, сначала создайте его");
        var maxNumber = matrix.GetLength(0) + 1;
        var rowNumber = ReadInt($"Номер новой строки (1..{maxNumber}): ", 1, maxNumber);
        var isManual = ReadInt("Заполнить строку: 1 - вручную, 2 - случайно: ", 1, 2) == 1;
        var newRow = CreateRow(matrix.GetLength(1), isManual, "Новая строка");
        AddRow(ref matrix, rowNumber, newRow);
    }

    private static void AddRow(ref int[,] matrix, int rowNumber, int[] newRow)
    {
        var rowsCount = matrix.GetLength(0);
        var columnsCount = matrix.GetLength(1);
        if (rowNumber < 1 || rowNumber > rowsCount + 1)
            throw new InvalidOperationException($"номер строки должен быть от 1 до {rowsCount + 1}");
        var rowIndex = rowNumber - 1;
        var result = new int[rowsCount + 1, columnsCount];
        Array.Copy(matrix, 0, result, 0, rowIndex * columnsCount);
        for (var j = 0; j < columnsCount; j++)
            result[rowIndex, j] = newRow[j];
        Array.Copy(
            matrix,
            rowIndex * columnsCount,
            result,
            (rowIndex + 1) * columnsCount,
            (rowsCount - rowIndex) * columnsCount);
        matrix = result;
    }

    private static void DeleteLongestRow(ref int[][] rows, out int removedNumber)
    {
        ThrowIfEmpty(rows.Length == 0, "Нельзя удалить строку, массив пуст");
        var index = FindLongestRowIndex(rows);
        var result = new int[rows.Length - 1][];
        Array.Copy(rows, 0, result, 0, index);
        Array.Copy(rows, index + 1, result, index, rows.Length - index - 1);
        removedNumber = index + 1;
        rows = result;
    }

    private static int FindLongestRowIndex(int[][] rows, int startIndex = 0)
    {
        if (startIndex == rows.Length - 1)
            return startIndex;
        var restIndex = FindLongestRowIndex(rows, startIndex + 1);
        return rows[restIndex].Length > rows[startIndex].Length ? restIndex : startIndex;
    }

    private static string ChooseTestText()
    {
        for (var i = 0; i < TestTexts.Length; i++)
            Console.WriteLine($"{i + 1}. {TestTexts[i]}");
        return TestTexts[ReadInt("Номер тестовой строки: ", 1, TestTexts.Length) - 1];
    }

    private static string ReverseOddSentences(string text, int sentenceNumber = 1)
    {
        if (text.Length == 0)
            return string.Empty;
        var end = FindSentenceEnd(text);
        var sentence = text.Substring(0, end);
        var processed = sentenceNumber % 2 == 1 ? ReverseSentence(sentence) : sentence;
        return processed + ReverseOddSentences(text.Substring(end), sentenceNumber + 1);
    }

    private static int FindSentenceEnd(string text)
    {
        var end = text.IndexOfAny(SentenceTerminators);
        if (end == -1)
            return text.Length;
        while (end + 1 < text.Length && Array.IndexOf(SentenceTerminators, text[end + 1]) >= 0)
            end++;
        return end + 1;
    }

    private static string ReverseSentence(string sentence)
    {
        var body = sentence.TrimEnd(SentenceTerminators);
        var ending = sentence.Substring(body.Length);
        var core = body.Trim(WordSeparators);
        if (core.Length == 0)
            return sentence;
        var leadLength = body.Length - body.TrimStart(WordSeparators).Length;
        var leadGap = body.Substring(0, leadLength);
        var trailGap = body.Substring(leadLength + core.Length);
        var parts = Regex.Split(core, "([ ,;:]+)");
        var wasCapitalized = char.IsUpper(core[0]);
        Array.Reverse(parts);
        if (wasCapitalized && parts.Length > 1)
        {
            parts[0] = ChangeFirstLetter(parts[0], isUpperCase: true);
            parts[parts.Length - 1] = ChangeFirstLetter(parts[parts.Length - 1], isUpperCase: false);
        }
        return leadGap + string.Concat(parts) + trailGap + ending;
    }

    private static string ChangeFirstLetter(string word, bool isUpperCase)
    {
        var letter = isUpperCase ? char.ToUpper(word[0]) : char.ToLower(word[0]);
        return letter + word.Substring(1);
    }

    private static void PrintArray(int[,] matrix, string title = "Двумерный массив")
    {
        if (matrix.Length == 0)
        {
            Console.WriteLine("Массив пуст.");
            return;
        }
        Console.WriteLine(title + ":");
        for (var i = 0; i < matrix.GetLength(0); i++)
        {
            for (var j = 0; j < matrix.GetLength(1); j++)
                Console.Write($"{matrix[i, j],5}");
            Console.WriteLine();
        }
    }

    private static void PrintArray(int[][] rows, string title = "Рваный массив")
    {
        if (rows.Length == 0)
        {
            Console.WriteLine("Массив пуст.");
            return;
        }
        Console.WriteLine(title + ":");
        for (var i = 0; i < rows.Length; i++)
        {
            for (var j = 0; j < rows[i].Length; j++)
                Console.Write($"{rows[i][j],5}");
            Console.WriteLine();
        }
    }

    private static void PrintText(string text, string title = "Строка")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Console.WriteLine("Строка пуста.");
            return;
        }
        Console.WriteLine($"{title}: {text}");
    }

    private static void PrintError(string message)
    {
        Console.WriteLine($"Ошибка: {message}.");
    }

    private static void ThrowIfEmpty(bool isEmpty, string message)
    {
        if (isEmpty)
            throw new InvalidOperationException(message);
    }

    private static int ReadMenuChoice(string title, string exitLabel, params string[] items)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
        for (var i = 0; i < items.Length; i++)
            Console.WriteLine($"{i + 1}. {items[i]}");
        Console.WriteLine($"0. {exitLabel}");
        return ReadInt("Ваш выбор: ", 0, items.Length);
    }

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        var line = Console.ReadLine();
        if (line == null) 
            Environment.Exit(0);
        return line;
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            if (TryConvertToInt(ReadText(prompt), out var value))
                return value;
        }
    }

    private static int ReadInt(string prompt, int minValue, int maxValue)
    {
        while (true)
        {
            var value = ReadInt(prompt);
            if (value >= minValue && value <= maxValue)
                return value;
            PrintError($"число должно быть от {minValue} до {maxValue}");
        }
    }

    private static bool TryConvertAll(string[] parts, int[] values)
    {
        for (var i = 0; i < parts.Length; i++)
        {
            if (!TryConvertToInt(parts[i], out values[i]))
                return false;
        }
        return true;
    }

    private static bool TryConvertToInt(string text, out int value)
    {
        value = 0;
        try
        {
            value = Convert.ToInt32(text);
            return true;
        }
        catch (FormatException)
        {
            PrintError("вместо целого числа введены посторонние символы");
        }
        catch (OverflowException)
        {
            PrintError("число слишком велико");
        }
        return false;
    }
}