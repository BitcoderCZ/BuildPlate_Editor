using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPlus;

namespace BuildPlate_Editor
{
    class Program
    {
        public static Window Window;

        static void Main(string[] args)
        {
            if (!File.Exists(Environment.CurrentDirectory + "/texturesPath.txt")) {
                Console.WriteLine("texturesPath.txt doen't exist");
                Console.WriteLine($"Extract mce resource pack 2 times and set path to {{path}}/textures/blocks/");
                Console.ReadKey(true);
                return;
            }
            string texturesPath = File.ReadAllText(Environment.CurrentDirectory + "/texturesPath.txt");
            if (texturesPath == string.Empty || !Directory.Exists(texturesPath)) {
                Console.WriteLine($"path inside texturesPath.txt ({texturesPath}) doesn't exist");
                Console.WriteLine($"Extract mce resource pack 2 times and set path to {{path}}/textures/blocks/");
                Console.ReadKey(true);
                return;
            }
            char lastChar = texturesPath[texturesPath.Length - 1];
            if (lastChar != '\\' && lastChar != '/')
                texturesPath += '/';
            World.textureBasePath = texturesPath;
#if DEBUG
            World.targetFilePath = @"C:\Users\Tomas\Desktop\Project Earth\Api\data\buildplates\b22b4ada-49e5-41c9-8bf0-76e36c5ec7b2.json";
#else
            Console.Write("Build plate to edit (.json): ");
            string buildPlate = Console.ReadLine();
            if (!File.Exists(buildPlate)) {
                Console.WriteLine($"build plate \"{buildPlate}\" doesn't exist");
                Console.ReadKey(true);
                return;
            }
            World.targetFilePath = buildPlate;
#endif
            Window = new Window();
            string version;
            try { version = OpenGLHelper.GetVersion(); } catch { version = "Failed to get version"; }
            Console.WriteLine($"OpenGL version: {version}, Min: 4.5.0");
            if (int.TryParse(version.Split(' ')[0].Split('.')[0], out int mainVer))
                if (mainVer < 4)
                    LowVersion();
                else if (int.TryParse(version.Split(' ')[0].Split('.')[1], out int subVer) && mainVer == 4 && subVer < 5)
                    LowVersion();
            Window.Run(60d);
        }

        private static void LowVersion()
        {
            Console.WriteLine("OpenGL version is low. The editor might not work correctly");
            Console.WriteLine("Press ENTER to continue anyway...");
            if (Console.ReadKey(true).Key != ConsoleKey.Enter)
                Environment.Exit(2);
        }
    }
}
