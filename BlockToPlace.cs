using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static int startTop;
        private static int lastNumbLines;

        public static void Init()
        {
            loopThread = new Thread(() => Loop());
            loopThread.Start();
        }

        private static void Loop()
        {
            while (true) {
                start:
                if (!ShouldBeTakingInput)
                    Thread.Sleep(1);

                // get input
                ConsoleKeyInfo info = Console.ReadKey(true);
                ConsoleKey key = info.Key;

                if (key == ConsoleKey.Enter) {
                    ShouldBeTakingInput = false;
                    goto start;
                }
                else if (key == ConsoleKey.Backspace && inputed.Length > 0)
                    inputed = inputed.Substring(0, inputed.Length - 1);
                else
                    inputed += info.KeyChar;

                // clear last render
                Console.CursorTop = startTop;
                Console.CursorLeft = 0;
                for (int i = 0; i < lastNumbLines; i++) {
                    Console.CursorLeft = 0;
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                }

                // render
                Console.CursorTop = startTop;
                Console.CursorLeft = 0;
                Console.WriteLine(inputed);

                lastNumbLines = 1;
            }
        }

        public static string TakeInput()
        {
            inputed = string.Empty;
            startTop = Console.CursorTop;
            lastNumbLines = 0;
            Util.SetConsoleForeground();
            ShouldBeTakingInput = true;
            while (ShouldBeTakingInput) Thread.Sleep(1);
            Util.SetOpenTKForeground();
            return inputed;
        }
    }
}
