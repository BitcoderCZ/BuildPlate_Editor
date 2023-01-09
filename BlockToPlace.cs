using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public static class BlockToPlace
    {
        public static bool ShouldBeTakingInput = false;
        private static string inputed;

        public static Thread loopThread;

        public const int MaxResults = 8;
        public const int MinCorrect = 1;
        public const float SkipPenalty = -0.125f;

        private static int startTop;

        private static string[] autocomplete;

        public static void Init()
        {
            string[] lines = File.ReadAllLines(Program.baseDir + "Data\\Blocks.txt");
            autocomplete = new string[lines.Length];

            Parallel.For(0, lines.Length, Util.DefaultParallelOptions, (int i) =>
            {
                autocomplete[i] = lines[i].Split(',')[1].Split(':')[1];
            });

            loopThread = new Thread(() => Loop());
            loopThread.Start();
        }

        private static void Loop()
        {
            string bestAutoComp = string.Empty;
            int autoSelected = 0;

            string[] autocomp = new string[0];

            while (true) {
                start:
                if (!ShouldBeTakingInput)
                    Thread.Sleep(1);

                // get input
                ConsoleKeyInfo info = Console.ReadKey(true);
                ConsoleKey key = info.Key;

                if (key == ConsoleKey.Enter) {
                    ShouldBeTakingInput = false;
                    for (int i = 0; i < MaxResults + 2; i++) {
                        Console.SetCursorPosition(0, startTop + i);
                        Console.WriteLine(new string(' ', Console.WindowWidth));
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, startTop);
                    Console.WriteLine($"Selected: {inputed}");
                    Console.ResetColor();
                    goto start;
                }
                else if (key == ConsoleKey.Escape) {
                    ShouldBeTakingInput = false;
                    for (int i = 0; i < MaxResults + 2; i++) {
                        Console.SetCursorPosition(0, startTop + i);
                        Console.WriteLine(new string(' ', Console.WindowWidth));
                    }
                    inputed = string.Empty;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, startTop);
                    Console.WriteLine($"Selected: None");
                    Console.ResetColor();
                    goto start;
                } else if (key == ConsoleKey.UpArrow) {
                    autoSelected--;
                    if (autoSelected < 0)
                        autoSelected = 0;
                } else if (key == ConsoleKey.DownArrow) {
                    autoSelected++;
                    if (autoSelected >= autocomp.Length)
                        autoSelected = autocomp.Length - 1;
                }
                else if (key == ConsoleKey.Backspace && inputed.Length > 0) {
                    inputed = inputed.Substring(0, inputed.Length - 1);
                    autoSelected = 0;
                }
                else if (key == ConsoleKey.Tab && bestAutoComp != string.Empty) {
                    inputed = bestAutoComp;
                    autoSelected = 0;
                }
                else {
                    inputed += info.KeyChar;
                    autoSelected = 0;
                }

                // clear last render
                for (int i = 0; i < MaxResults + 2; i++) {
                    Console.SetCursorPosition(0, startTop + i);
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                }

                // get/set autocomplete
                autocomp = GetAutoComplete();
                if (autocomp.Length > 0)
                    bestAutoComp = autocomp[Math.Min(autoSelected, autocomp.Length - 1)];
                else
                    bestAutoComp = string.Empty;

                // render
                Console.SetCursorPosition(0, startTop);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(inputed);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(GetRest(bestAutoComp));
                Console.ResetColor();

                for (int i = 0; i < autocomp.Length; i++) {
                    Console.SetCursorPosition(0, startTop + 1 + i);
                    if (i != autoSelected)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(autocomp[i] + new string(' ', Console.WindowWidth - autocomp[i].Length));
                    Console.ResetColor();
                }
            }
        }

        private static string GetRest(string auto)
        {
            if (auto.Length < 1 || inputed.Length < 1)
                return string.Empty;

            bool s = false;
            int inpIndex = 0;
            for (int j = 0; j < auto.Length; j++) {
                if (s == false && auto[j].ToLower() == inputed[inpIndex].ToLower()) {
                    inpIndex++;
                    s = true;
                }
                else if (s == true && auto[j].ToLower() != inputed[inpIndex].ToLower()) {
                    break;
                }
                else if (s == true && auto[j].ToLower() == inputed[inpIndex].ToLower())
                    inpIndex++;

                if (inpIndex >= inputed.Length)
                    break;
            }

            if (inpIndex < auto.Length)
                return auto.Substring(inpIndex);
            else
                return string.Empty;
        }

        private static string[] GetAutoComplete()
        {
            if (inputed.Length < 1)
                return new string[0];

            List<(string text, float correct)> _res = new List<(string text, float correct)>();

            for (int i = 0; i < autocomplete.Length; i++) {
                string auto = autocomplete[i];
                bool s = false;
                float correct = 0f;
                int inpIndex = 0;
                for (int j = 0; j < auto.Length; j++) {
                    if (s == false && auto[j].ToLower() == inputed[inpIndex].ToLower()) {
                        correct++;
                        inpIndex++;
                        s = true;
                    }
                    else if (s == true && auto[j].ToLower() != inputed[inpIndex].ToLower()) {
                        break;
                    }
                    else if (s == true && auto[j].ToLower() == inputed[inpIndex].ToLower()) {
                        correct++;
                        inpIndex++;
                    }
                    else
                        correct += SkipPenalty;

                    if (inpIndex >= inputed.Length)
                        break;
                }

                if (correct >= MinCorrect || correct >= inputed.Length)
                    _res.Add((auto, correct));
            }

            _res.Sort(((string text, float correct) a, (string text, float correct) b) =>
            {
                int compRes = a.correct.CompareTo(b.correct);
                if (compRes != 0)
                    return compRes;
                else
                    return a.text.ToLower().CompareTo(b.text.ToLower());
            });

            _res.Reverse();

            string[] res = new string[Math.Min(MaxResults, _res.Count)];

            for (int i = 0; i < res.Length; i++)
                res[i] = _res[i].text;

            return res;
        }

        public static string TakeInput()
        {
            inputed = string.Empty;
            startTop = Console.CursorTop;
            Util.SetConsoleForeground();
            ShouldBeTakingInput = true;
            while (ShouldBeTakingInput) Thread.Sleep(1);
            Util.SetOpenTKForeground();
            return inputed;
        }
    }
}
