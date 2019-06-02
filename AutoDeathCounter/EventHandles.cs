using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AutoDeathCounter
{
    internal static class EventHandles
    {
        private static readonly string FilePath = Path.Combine(Environment.CurrentDirectory, "output.txt");

        internal static void MainThread_OnHPChange(int currentHP, int lastHP)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"currentHP: {currentHP}");
            Console.ForegroundColor = ConsoleColor.Gray;

            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "outputHP.txt"), $"{currentHP}");
        }

        internal static void MainThread_OnDeath()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"OnDeath");
            Console.ForegroundColor = ConsoleColor.Gray;

            if (File.Exists(FilePath))
            {
                var content = File.ReadAllText(FilePath);
                File.WriteAllText(FilePath, $"Deaths: {GetNumber(content)}");
            }
            else
            {
                GenerateNewFile();
            }
        }

        internal static void MainThread_OnRespawn()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"OnRespawn");
            Console.ForegroundColor = ConsoleColor.Gray;

            if (File.Exists(FilePath))
            {
                var content = File.ReadAllText(FilePath);
                File.WriteAllText(FilePath, $"Live #{GetNumber(content) + 1}");
            }
            else
            {
                GenerateNewFile();
            }
        }

        private static int GetNumber(string content)
        {
            var match = Regex.Match(content, @"\d+");
            if (match.Success)
            {
                if (int.TryParse(match.Value, out int val))
                {
                    return val;
                }
            }
            return 0;
        }

        private static void GenerateNewFile()
        {
            File.WriteAllText(FilePath, $"Deaths: 0");
        }
    }
}