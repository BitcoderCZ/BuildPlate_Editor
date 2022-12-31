using BuildPlate_Editor.Maths;
using Microsoft.Win32;
using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
    public static class Util
    {
        public const float PI = (float)System.Math.PI;

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

        public static void SkyboxTex(int _tex, Vector3 size, ref List<Vertex> verts, ref List<uint> tris)
        {
            uint tex = (uint)_tex;
            for (int p = 0; p < 6; p++) {
                uint firstVertIndex = (uint)verts.Count;
                verts.Add(new Vertex(VoxelData.voxelVerts[VoxelData.SkyBox.tris[p, 3]] * size - size / 2f, VoxelData.SkyBox.voxelUvs[p][0], tex));
                verts.Add(new Vertex(VoxelData.voxelVerts[VoxelData.SkyBox.tris[p, 2]] * size - size / 2f, VoxelData.SkyBox.voxelUvs[p][1], tex));
                verts.Add(new Vertex(VoxelData.voxelVerts[VoxelData.SkyBox.tris[p, 1]] * size - size / 2f, VoxelData.SkyBox.voxelUvs[p][2], tex));
                verts.Add(new Vertex(VoxelData.voxelVerts[VoxelData.SkyBox.tris[p, 0]] * size - size / 2f, VoxelData.SkyBox.voxelUvs[p][3], tex));
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
    }
}
