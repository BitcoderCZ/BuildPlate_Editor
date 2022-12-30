﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using SystemPlus;

namespace BuildPlate_Editor
{
    class Program
    {
        public static Window Window;
        public static string baseDir;

        // todo: check json is buildplate
        static void Main(string[] args)
        {
            // Get base path (.exe location)
            string myExecutable = Assembly.GetEntryAssembly().Location;

            if (args != null && args.Length > 0 && args[0] == "setJsonDefault") {
                Util.SetAssociationWithExtension(".json", "Json", myExecutable, "BuildPlate");
                Console.WriteLine("Set .json as default, To Apply: Select .json file, click \"Open with\", \"Choose another app\", " +
                    "Select BuildPlate_Editor, Check \"Always use this app...\"");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            baseDir = Path.GetDirectoryName(myExecutable) + "\\";
            Console.WriteLine($"Base directory: {baseDir}");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (File.Exists(baseDir + "askedForDefault") || args != null && args.Length > 0)
                goto skipAks;

            Console.Write("Would you like to set this as default for .json files? (Y/N):");
            char typed = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (typed == 'y' || typed == 'Y') {
                // Make this app default for .json files 
                try {
                    // Launch again with admin, so it can edit registry
                    using (Process configTool = new Process()) {
                        configTool.StartInfo.FileName = myExecutable;
                        configTool.StartInfo.Arguments = "setJsonDefault";
                        configTool.StartInfo.Verb = "runas";
                        configTool.Start();
                        configTool.WaitForExit();
                    }
                    Console.WriteLine("Set this as default for .json");
                } catch (Exception ex) {
                    Console.WriteLine($"Failed to edit registry: {ex}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    goto skipAks;
                }
            }
            else
                Console.WriteLine("Ok :(, I won't aks again");

            File.WriteAllBytes(baseDir + "askedForDefault", new byte[0]);

            skipAks:

            if (!File.Exists(baseDir + "texturesPath.txt")) {
                Console.WriteLine("texturesPath.txt doen't exist");
                Console.WriteLine($"Extract mce resource pack 2 times and set path to {{path}}/textures/blocks/");
                Console.ReadKey(true);
                return;
            }
            string texturesPath = File.ReadAllText(baseDir + "texturesPath.txt");
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
            //World.targetFilePath = @"C:\Users\Tomas\Desktop\Project Earth\Api\data\buildplates\7cd6d53b-1715-4b22-9a99-d6d43edd61df.json";
            World.targetFilePath = @"C:\Users\Tomas\Desktop\Project Earth\Api\data\buildplates\00d1fa99-7acf-449d-bb4f-8d11127bd6e3.json";
#else
            Console.Write("Build plate to edit (.json): ");
            if (args != null && args.Length > 0 && File.Exists(args[0])) {
                World.targetFilePath = args[0];
                Console.WriteLine(args[0]);
            }
            else {
                string buildPlate = Console.ReadLine();
                if (!File.Exists(buildPlate)) {
                    Console.WriteLine($"build plate \"{buildPlate}\" doesn't exist");
                    Console.ReadKey(true);
                    return;
                }
                World.targetFilePath = buildPlate;
            }
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

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Console.WriteLine($"CurrentDomain_UnhandledException: {ex.Message}");
            Console.WriteLine($"Source: {ex.Source}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
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
