using BuildPlate_Editor.Maths;
using Microsoft.Win32;
using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemPlus;

namespace BuildPlate_Editor
{
    public static class Util
    {
        public static readonly int CoreCount = Environment.ProcessorCount;

        public static readonly ParallelOptions DefaultParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = CoreCount };

        public const float PI = (float)Math.PI;

        public static string Base64Encode(string normal)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(normal);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static T JsonDeserialize<T>(string json)
        {
            TextReader reader = new StringReader(json);
            T value = (T)JsonSerializer.CreateDefault().Deserialize(reader, typeof(T));
            reader.Dispose();
            return value;
        }

        public static string JsonSerialize<T>(T value)
        {
            StringBuilder builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            JsonSerializer.CreateDefault().Serialize(writer, value);
            writer.Dispose();
            return builder.ToString();
        }

        public static int Normalized(this int i)
        {
            int value = Math.Min(Math.Max(i, -1), 1);
            if (value == 0)
                value = 1;
            return value;
        }

        // Borrowed from here: https://keithmaggio.wordpress.com/2011/02/15/math-magician-lerp-slerp-and-nlerp/
        public static Vector3 Slerp(Vector3 start, Vector3 end, float percent)
        {
            // Dot product - the cosine of the angle between 2 vectors.
            float dot = Vector3.Dot(start, end);

            // Clamp it to be in the range of Acos()
            // This may be unnecessary, but floating point
            // precision can be a fickle mistress.
            dot = MathPlus.Clamp(dot, -1.0f, 1.0f);

            // Acos(dot) returns the angle between start and end,
            // And multiplying that by percent returns the angle between
            // start and the final result.
            float theta = (float)Math.Acos(dot) * percent;
            Vector3 RelativeVec = end - start * dot;
            RelativeVec.Normalize();

            // Orthonormal basis
            // The final result.
            return (start * (float)Math.Cos(theta)) + (RelativeVec * (float)Math.Sin(theta));
        }

        public static void Sort(ref int smaller, ref int bigger)
        {
            if (smaller > bigger) {
                int i = bigger;
                bigger = smaller;
                smaller = i;
            }
        }

        public static void Sort(ref Vector3i smaller, ref Vector3i bigger)
        {
            Sort(ref smaller.X, ref bigger.X);
            Sort(ref smaller.Y, ref bigger.Y);
            Sort(ref smaller.Z, ref bigger.Z);
        }

        public static int GetRotDiff(int rot1, int rot2)
        {
            while (rot1 < rot2) {
                rot1++;
                rot2++;
                rot1 %= 4;
                rot2 %= 4;
            }
            int diff = rot2 - rot1;
            return diff;
        }

        public static void CubeTex(int _tex, Vector3 pos, Vector3 size, ref List<Vertex> verts, ref List<uint> tris)
        {
            uint tex = (uint)_tex;
            for (int p = 0; p < 6; p++) {
                uint firstVertIndex = (uint)verts.Count;
                verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] * size - size / 2f, VoxelData.voxelUvs[0], tex));
                verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] * size - size / 2f, VoxelData.voxelUvs[1], tex));
                verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] * size - size / 2f, VoxelData.voxelUvs[2], tex));
                verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] * size - size / 2f, VoxelData.voxelUvs[3], tex));
                tris.Add(firstVertIndex);
                tris.Add(firstVertIndex + 1);
                tris.Add(firstVertIndex + 2);
                tris.Add(firstVertIndex + 2);
                tris.Add(firstVertIndex + 1);
                tris.Add(firstVertIndex + 3);
            }
        }

        public static void CubeTex(int _tex, Vector3 pos, Vector3 size, ref List<Vertex> verts, ref List<uint> tris, Vector2[] uvs)
        {
            uint tex = (uint)_tex;
            for (int p = 0; p < 6; p++) {
                uint firstVertIndex = (uint)verts.Count;
                verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] * size - size / 2f, uvs[0], tex));
                verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] * size - size / 2f, uvs[1], tex));
                verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] * size - size / 2f, uvs[2], tex));
                verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] * size - size / 2f, uvs[3], tex));
                tris.Add(firstVertIndex);
                tris.Add(firstVertIndex + 1);
                tris.Add(firstVertIndex + 2);
                tris.Add(firstVertIndex + 2);
                tris.Add(firstVertIndex + 1);
                tris.Add(firstVertIndex + 3);
            }
        }

        public static T2[] ForArray<T1, T2>(this IEnumerator<T1> e, Func<T1, T2> func)
        {
            List<T2> list = new List<T2>();
            while (e.MoveNext())
                list.Add(func(e.Current));
            e.Reset();
            return list.ToArray();
        }

        public static T2[] ForArray<T1, T2>(this List<T1> list, Func<T1, T2> func)
        {
            T2[] array = new T2[list.Count];
            for (int i = 0; i < list.Count; i++)
                array[i] = func(list[i]);
            return array;
        }

        public static T2[] ForArray<T1, T2>(this T1[] _array, Func<T1, T2> func)
        {
            T2[] array = new T2[_array.Length];
            for (int i = 0; i < _array.Length; i++)
                array[i] = func(_array[i]);
            return array;
        }

        public static T[] Cloned<T>(this T[] array)
        {
            T[] newArray = new T[array.Length];
            Array.Copy(array, newArray, array.Length);
            return newArray;
        }

        public static Vector2 Swaped(this Vector2 v) => new Vector2(v.Y, v.X);

        // needs admin
        public static void SetAssociationWithExtension(string Extension, string key, string OpenWith, string FileDescription)
        {
            RegistryKey key1 = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Classes\{Extension}");
            key1.SetValue("", $"{Extension.Replace(".", "")}.Document");
            key1.Flush();
            RegistryKey key2 = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Classes\{key}.Document");
            key2.SetValue("", FileDescription);
            key2.Flush();
            RegistryKey key3 = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Classes\{key}.Document\Shell\Open\Command");
            key3.SetValue("", $"\"{OpenWith}\" %1");
            key3.Flush();
            RegistryKey key4 = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Classes\{key}.Document\Shell\Edit\Command");
            key4.SetValue("", $"\"{OpenWith}\" %1");
            key4.Flush();
        }

        public static void Exit(EXITCODE code, Exception ex = null, string message = null)
        {
            if (code == EXITCODE.Normal)
                Environment.Exit((int)code);

            string exceptionMessage = "None";
            if (ex != null)
                exceptionMessage = $"\n  Type: {ex.GetType()}\n  Source: {ex.Source}\n  StackTrace: {ex.StackTrace}";

            string exitMessage = "None";
            if (message != null)
                exitMessage = message;

            Console.WriteLine($"\nExited with Code: {(int)code}, ExitCodeName: {Enum.GetName(typeof(EXITCODE), code)}");
            if (ex != null)
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception: {exceptionMessage}");
            Console.ResetColor();

            if (message != null)
                Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Message: {exitMessage}");
            Console.ResetColor();

            Console.WriteLine("Press any key to close...");
            Console.ReadKey(true);
            Environment.Exit((int)code);
        }


        public static float Width = 1280f;
        public static float Height = 720f;

        public static Vector2 NormalToGL(float x, float y)
           => new Vector2((x * 2f) - 1f, (y * 2f) - 1f);
        public static Vector2 NormalToGL(Vector2 pos)
            => NormalToGL(pos.X, pos.Y);

        public static Vector2 GLToNormal(float x, float y)
            => new Vector2((x + 1f) / 2f, (y + 1f) / 2f);
        public static Vector2 GLToNormal(Vector2 pos)
            => GLToNormal(pos.X, pos.Y);

        public static Vector2 PixelToNormal(int x, int y)
            => new Vector2((float)x / Width, (float)y / Height);
        public static Vector2 PixelToNormal(Vector2i pos)
            => PixelToNormal(pos.X, pos.Y);
        public static Vector2 PixelToGL(int x, int y)
            => new Vector2(((float)x / Width) * 2f - 1f, ((float)y / Height) * 2f - 1f);
        public static Vector2 PixelToGL(Vector2i pos)
            => PixelToGL(pos.X, pos.Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);


        public static void SetConsoleForeground()
        {
            string originalTitle = Console.Title;
            string uniqueTitle = Guid.NewGuid().ToString();
            Console.Title = uniqueTitle;
            Thread.Sleep(20);
            IntPtr handle = FindWindowByCaption(IntPtr.Zero, uniqueTitle);

            if (handle == IntPtr.Zero) {
                Console.WriteLine("Oops, cant find main window.");
                return;
            }
            Console.Title = originalTitle;
            int c = 0;
            while (true) {
                Thread.Sleep(20);
                if (SetForegroundWindow(handle))
                    return;
                c++;
                if (c > 100)
                    return;
            }
        }
        public static void SetOpenTKForeground()
        {
            string originalTitle = Program.Window.Title;
            string uniqueTitle = Guid.NewGuid().ToString();
            Program.Window.Title = uniqueTitle;
            Thread.Sleep(20);
            IntPtr handle = FindWindowByCaption(IntPtr.Zero, uniqueTitle);

            if (handle == IntPtr.Zero) {
                Console.WriteLine("Oops, cant find main window.");
                return;
            }
            Program.Window.Title = originalTitle;
            int c = 0;
            while (true) {
                Thread.Sleep(20);
                if (SetForegroundWindow(handle))
                    return;
                c++;
                if (c > 100)
                    return;
            }
        }

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);
        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        public static List<IntPtr> GetRootWindowsOfProcess(int pid)
        {
            List<IntPtr> rootWindows = GetChildWindows(IntPtr.Zero);
            List<IntPtr> dsProcRootWindows = new List<IntPtr>();
            foreach (IntPtr hWnd in rootWindows) {
                uint lpdwProcessId;
                GetWindowThreadProcessId(hWnd, out lpdwProcessId);
                if (lpdwProcessId == pid)
                    dsProcRootWindows.Add(hWnd);
            }
            return dsProcRootWindows;
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try {
                Win32Callback childProc = new Win32Callback(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null) {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }
    }
}
