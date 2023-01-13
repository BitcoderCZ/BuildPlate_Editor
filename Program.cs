using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SystemPlus;

namespace BuildPlate_Editor
{
    class Program
    {
        public static Window Window;
        public static string baseDir;

        static void Main(string[] args)
        {
            // this will run on console close
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            // Get base path (.exe location)
            string myExecutable = Assembly.GetEntryAssembly().Location;

            if (args != null && args.Length > 0 && args[0] == "setJsonDefault") {
                Util.SetAssociationWithExtension(".json", "Json", myExecutable, "BuildPlate");
                Util.SetAssociationWithExtension(".plate", "Plate", myExecutable, "BuildPlate");
                Util.SetAssociationWithExtension(".plate64", "Plate64", myExecutable, "BuildPlate");
                Console.WriteLine("Set .json as default, To Apply: Select .json file, click \"Open with\", \"Choose another app\", " +
                    "Select BuildPlate_Editor, Check \"Always use this app...\"");
                Console.WriteLine("You can also do this for .plate and .plate64");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            baseDir = Path.GetDirectoryName(myExecutable) + "\\";
            Console.WriteLine($"Base directory: {baseDir}");

#if DEBUG == false
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

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
            World.targetFilePath = @"C:\Users\Tomas\Desktop\Project Earth\Api\data\buildplates\buildplate (1).json";
            //World.targetFilePath = @"C:\Users\Tomas\Desktop\Project Earth\Api\data\buildplates\411398a6-5810-43a0-824c-27a9077a9ac3.json"; // fancy
            //World.targetFilePath = @"C:\Users\Tomas\Desktop\Project Earth\Api\data\buildplates\c0eb3037-94c1-4a85-b0d7-7b8375c6f1b1.json"; // redstone
            //World.targetFilePath = @"C:\Users\Tomas\Desktop\Project Earth\Api\data\buildplates\c0f771f0-a5d6-4acc-bd35-055e52548a20.json"; // igloo super fancy secret base
            //World.targetFilePath = @"C:\Users\Tomas\Desktop\Project Earth\Api\data\buildplates\2cb8bda2-c49b-4ead-887f-593d37bc2784.json";
#else
            Console.Write("Build plate to edit (.json): ");
            // json - api, plate - data only, plate64 - base64 encoded data
            if (args != null && args.Length > 0 && File.Exists(string.Join(" ", args)) && string.Join(" ", args).Split('.').Length > 1) {
                string truePath = new FileInfo(string.Join(" ", args)).FullName;
                Console.WriteLine(truePath);
                string extension = truePath.Split('.').Last();
                if (extension == "json" || extension == "plate" || extension == "plate64") {
                    World.targetFilePath = truePath;
                    goto check;
                }
            }

            string buildPlate = Console.ReadLine();
            if (!File.Exists(buildPlate)) {
                Console.WriteLine($"build plate \"{buildPlate}\" doesn't exist");
                Console.ReadKey(true);
                return;
            } else if (Path.GetExtension(buildPlate) == ".json" || Path.GetExtension(buildPlate) == ".plate" 
                || Path.GetExtension(buildPlate) == ".plate64") {
                Console.WriteLine($"\"{Path.GetExtension(buildPlate)}\" isn't valid buildplate extension, valid: .json, .plate, .plate64");
                Console.ReadKey(true);
                return;
            }
            World.targetFilePath = buildPlate;
#endif
            check:
            // check json is buildplate
            try {
                BuildPlate.Load(World.targetFilePath);
            } catch {
                Console.WriteLine($"Couldn't parse \"{World.targetFilePath}\", Make sure it's valid build plate file.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                return;
            }

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
                Util.Exit(EXITCODE.OpenGL_LowVersion);
        }

        // Handle on close event
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2) { // on exit
                Util.Exit(EXITCODE.Normal);
            }
            return false;
        }
        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
                                               
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
