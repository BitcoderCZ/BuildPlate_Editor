using BuildPlate_Editor.Maths;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildPlate_Editor
{
	public static class VoxelData
	{
		public const int ChunkWidth = 16;
		public const int ChunkHeight = 16;
		public const int ChunkLayerLength = ChunkWidth * ChunkWidth;

		public static readonly Vector3[] voxelVerts = new Vector3[8] {
			new Vector3(0.0f, 0.0f, 0.0f), // 0
			new Vector3(1.0f, 0.0f, 0.0f), // 1
			new Vector3(1.0f, 1.0f, 0.0f), // 2
			new Vector3(0.0f, 1.0f, 0.0f), // 3
			new Vector3(0.0f, 0.0f, 1.0f), // 4
			new Vector3(1.0f, 0.0f, 1.0f), // 5
			new Vector3(1.0f, 1.0f, 1.0f), // 6
			new Vector3(0.0f, 1.0f, 1.0f) //  7
		};

		public static readonly Vector3i[] faceChecks = new Vector3i[6] {
			new Vector3i(0, 0, -1),
			new Vector3i(0, 0, 1),
			new Vector3i(0, 1, 0),
			new Vector3i(0, -1, 0),
			new Vector3i(-1, 0, 0),
			new Vector3i(1, 0, 0)
		};

		public static readonly uint[] voxelLines = new uint[] {
			0, 1,
			0, 3,
			0, 4,
			6, 5,
			6, 2,
			6, 7,
			2, 3,
			2, 1,
			5, 1,
			5, 4,
			2, 3,
			2, 1,
			7, 3,
			7, 4,
		};

		public static readonly int[,] voxelTris = new int[6, 4] {
			{0, 3, 1, 2}, // Back Face
			{5, 6, 4, 7}, // Front Face
			{3, 7, 2, 6}, // Top Face
			{1, 5, 0, 4}, // Bottom Face
			{4, 7, 0, 3}, // Left Face
			{1, 2, 5, 6} // Right Face
		};

		public static readonly Vector2[] voxelUvs = new Vector2[4] {
			new Vector2(0.0f, 0.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(1.0f, 0.0f),
			new Vector2(1.0f, 1.0f)
		};

		public static class Slab
		{
			public static readonly Vector3[] bottomVerts = new Vector3[8] {
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.5f, 0.0f),
				new Vector3(0.0f, 0.5f, 0.0f),
				new Vector3(0.0f, 0.0f, 1.0f),
				new Vector3(1.0f, 0.0f, 1.0f),
				new Vector3(1.0f, 0.5f, 1.0f),
				new Vector3(0.0f, 0.5f, 1.0f)
			};
			public static readonly Vector3[] topVerts = new Vector3[8] {
				new Vector3(0.0f, 0.5f, 0.0f),
				new Vector3(1.0f, 0.5f, 0.0f),
				new Vector3(1.0f, 1.0f, 0.0f),
				new Vector3(0.0f, 1.0f, 0.0f),
				new Vector3(0.0f, 0.5f, 1.0f),
				new Vector3(1.0f, 0.5f, 1.0f),
				new Vector3(1.0f, 1.0f, 1.0f),
				new Vector3(0.0f, 1.0f, 1.0f)
			};
			public static readonly Vector2[] bottomUVs = new Vector2[4] {
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 0.5f),
				new Vector2(1.0f, 0.0f),
				new Vector2(1.0f, 0.5f)
			};
			public static readonly Vector2[] topUVs = new Vector2[4] {
				new Vector2(0.0f, 0.5f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 0.5f),
				new Vector2(1.0f, 1.0f)
			};
		}
		public static class Trapdoor
		{
			public static readonly Vector3[] verts = new Vector3[8] {
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.2f, 0.0f),
				new Vector3(0.0f, 0.2f, 0.0f),
				new Vector3(0.0f, 0.0f, 1.0f),
				new Vector3(1.0f, 0.0f, 1.0f),
				new Vector3(1.0f, 0.2f, 1.0f),
				new Vector3(0.0f, 0.2f, 1.0f)
			};
			public static readonly Vector2[] uVs = new Vector2[4] {
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 0.2f),
				new Vector2(1.0f, 0.0f),
				new Vector2(1.0f, 0.2f)
			};
		}
		public static class Torch // width = 0,125, height = 0,625, light part start = 0,5
		{
			public static readonly Vector3[] verts = new Vector3[8] {
				new Vector3(0.4375f, 0.0f, 0.4375f),
				new Vector3(0.5625f, 0.0f, 0.4375f),
				new Vector3(0.5625f, 0.625f, 0.4375f),
				new Vector3(0.4375f, 0.625f, 0.4375f),
				new Vector3(0.4375f, 0.0f, 0.5625f),
				new Vector3(0.5625f, 0.0f, 0.5625f),
				new Vector3(0.5625f, 0.625f, 0.5625f),
				new Vector3(0.4375f, 0.625f, 0.5625f)
			};
			public static readonly Vector2[] uVsSide = new Vector2[4] {
				new Vector2(0.4375f, 0.0f),
				new Vector2(0.4375f, 0.625f),
				new Vector2(0.5625f, 0.0f),
				new Vector2(0.5625f, 0.625f)
			};
			public static readonly Vector2[] uVsTop = new Vector2[4] {
				new Vector2(0.4375f, 0.5f),
				new Vector2(0.4375f, 0.625f),
				new Vector2(0.5625f, 0.5f),
				new Vector2(0.5625f, 0.625f)
			};
			public static readonly Vector2[] uVsBottom = new Vector2[4] {
				new Vector2(0.4375f, 0.0f),
				new Vector2(0.4375f, 0.125f),
				new Vector2(0.5625f, 0.0f),
				new Vector2(0.5625f, 0.125f)
			};
		}
		public static class Stair
		{
			public static readonly Vector3[] zVerts = new Vector3[8] {
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.5f, 0.0f),
				new Vector3(0.0f, 0.5f, 0.0f),
				new Vector3(0.0f, 0.0f, 0.5f),
				new Vector3(1.0f, 0.0f, 0.5f),
				new Vector3(1.0f, 0.5f, 0.5f),
				new Vector3(0.0f, 0.5f, 0.5f)
			};
			public static readonly Vector3[] xVerts = new Vector3[8] {
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(0.5f, 0.0f, 0.0f),
				new Vector3(0.5f, 0.5f, 0.0f),
				new Vector3(0.0f, 0.5f, 0.0f),
				new Vector3(0.0f, 0.0f, 1.0f),
				new Vector3(0.5f, 0.0f, 1.0f),
				new Vector3(0.5f, 0.5f, 1.0f),
				new Vector3(0.0f, 0.5f, 1.0f)
			};
			public static readonly Vector3[] smallVerts = new Vector3[8] {
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(0.5f, 0.0f, 0.0f),
				new Vector3(0.5f, 0.5f, 0.0f),
				new Vector3(0.0f, 0.5f, 0.0f),
				new Vector3(0.0f, 0.0f, 0.5f),
				new Vector3(0.5f, 0.0f, 0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(0.0f, 0.5f, 0.5f)
			};
			public static readonly Vector2[] zUvs = new Vector2[4] {
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 0.5f),
				new Vector2(1.0f, 0.0f),
				new Vector2(1.0f, 0.5f)
			};
			public static readonly Vector2[] xUvs = new Vector2[4] {
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(0.5f, 0.0f),
				new Vector2(0.5f, 1.0f)
			};
			public static readonly Vector2[] smallUvs = new Vector2[4] {
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 0.5f),
				new Vector2(0.5f, 0.0f),
				new Vector2(0.5f, 0.5f)
			};
		}
		public static class Repeater
		{
			public static readonly Vector3[] verts = new Vector3[8] {
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.125f, 0.0f),
				new Vector3(0.0f, 0.125f, 0.0f),
				new Vector3(0.0f, 0.0f, 1.0f),
				new Vector3(1.0f, 0.0f, 1.0f),
				new Vector3(1.0f, 0.125f, 1.0f),
				new Vector3(0.0f, 0.125f, 1.0f)
			};
			public static readonly Vector2[] sideUvs = new Vector2[4] {
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 0.125f),
				new Vector2(1.0f, 0.0f),
				new Vector2(1.0f, 0.125f)
			};
		}
		public static class SkyBox
		{
			public static Vector3[] verts = new Vector3[] {
				// positions          
				new Vector3(-1.0f,  1.0f, -1.0f),
				new Vector3(-1.0f, -1.0f, -1.0f),
				new Vector3(1.0f, -1.0f, -1.0f),
				new Vector3(1.0f, -1.0f, -1.0f),
				new Vector3(1.0f,  1.0f, -1.0f),
				new Vector3(-1.0f,  1.0f, -1.0f),

				new Vector3(-1.0f, -1.0f,  1.0f),
				new Vector3(-1.0f, -1.0f, -1.0f),
				new Vector3(-1.0f,  1.0f, -1.0f),
				new Vector3(-1.0f,  1.0f, -1.0f),
				new Vector3(-1.0f,  1.0f,  1.0f),
				new Vector3(-1.0f, -1.0f,  1.0f),

				new Vector3(1.0f, -1.0f, -1.0f),
				new Vector3(1.0f, -1.0f,  1.0f),
				new Vector3(1.0f,  1.0f,  1.0f),
				new Vector3(1.0f,  1.0f,  1.0f),
				new Vector3(1.0f,  1.0f, -1.0f),
				new Vector3(1.0f, -1.0f, -1.0f),

				new Vector3(-1.0f, -1.0f,  1.0f),
				new Vector3(-1.0f,  1.0f,  1.0f),
				new Vector3(1.0f,  1.0f,  1.0f),
				new Vector3(1.0f,  1.0f,  1.0f),
				new Vector3(1.0f, -1.0f,  1.0f),
				new Vector3(-1.0f, -1.0f,  1.0f),

				new Vector3(-1.0f,  1.0f, -1.0f),
				new Vector3(1.0f,  1.0f, -1.0f),
				new Vector3(1.0f,  1.0f,  1.0f),
				new Vector3(1.0f,  1.0f,  1.0f),
				new Vector3(-1.0f,  1.0f,  1.0f),
				new Vector3(-1.0f,  1.0f, -1.0f),

				new Vector3(-1.0f, -1.0f, -1.0f),
				new Vector3(-1.0f, -1.0f,  1.0f),
				new Vector3(1.0f, -1.0f, -1.0f),
				new Vector3(1.0f, -1.0f, -1.0f),
				new Vector3(-1.0f, -1.0f,  1.0f),
				new Vector3(1.0f, -1.0f,  1.0f)
			};
			public static uint[] tris;

			static SkyBox()
			{
				tris = new uint[verts.Length];
				for (uint i = 0; i < tris.Length; i++)
					tris[i] = i;
			}
		}
		public static class Vine // -z, -x, +z, +x
		{
			const float offset1 = 0.01f;
			const float offset2 = 1f - offset1;

			public static readonly int[,] tris = new int[2, 4] { // vines have only back and front faces
				{0, 3, 1, 2}, // Back Face
				{1, 2, 0, 3}, // Front Face
			};

			public static readonly Vector3[,] verts = new Vector3[4, 4]
			{
				{ new Vector3(0f, 0f, offset2), new Vector3(1f, 0f, offset2), new Vector3(1f, 1f, offset2), new Vector3(0f, 1f, offset2) },
				{ new Vector3(offset1, 0f, 0f), new Vector3(offset1, 0f, 1f), new Vector3(offset1, 1f, 1f), new Vector3(offset1, 1f, 0f) },
				{ new Vector3(0f, 0f, offset1), new Vector3(1f, 0f, offset1), new Vector3(1f, 1f, offset1), new Vector3(0f, 1f, offset1) },
				{ new Vector3(offset2, 0f, 0f), new Vector3(offset2, 0f, 1f), new Vector3(offset2, 1f, 1f), new Vector3(offset2, 1f, 0f) },
			};
		}
		public static class Cactus
		{
			const float offset1 = 0.0625f;
			const float offset2 = 1f - offset1;
			public static readonly Vector3[] verts = new Vector3[] {
				//-x face
				new Vector3(offset1, 0f, 0f),
				new Vector3(offset1, 0f, 1f),
				new Vector3(offset1, 1f, 0f),
				new Vector3(offset1, 1f, 1f),
				//+x face
				new Vector3(offset2, 0f, 0f),
				new Vector3(offset2, 0f, 1f),
				new Vector3(offset2, 1f, 0f),
				new Vector3(offset2, 1f, 1f),
				//-z face
				new Vector3(0f, 0f, offset1),
				new Vector3(1f, 0f, offset1),
				new Vector3(0f, 1f, offset1),
				new Vector3(1f, 1f, offset1),
				//+z face
				new Vector3(0f, 0f, offset2),
				new Vector3(1f, 0f, offset2),
				new Vector3(0f, 1f, offset2),
				new Vector3(1f, 1f, offset2),
				//-y face
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 1f),
				new Vector3(1f, 0f, 0f),
				new Vector3(1f, 0f, 1f),
				//+y face
				new Vector3(0f, 1f, 0f),
				new Vector3(0f, 1f, 1f),
				new Vector3(1f, 1f, 0f),
				new Vector3(1f, 1f, 1f),
			};
			public static readonly int[,] tris = new int[6, 4] {
				{ 0, 1, 2, 3 }, // Left quads
				{ 6, 7, 4, 5, }, // Right quads
				{ 10, 11, 8, 9 }, // Back quads
				{ 12, 13, 14, 15 }, // Front quads
				{ 18, 19, 16, 17 }, // Bottom quads
				{ 20, 21, 22, 23 } // Top quads
			};
			public static readonly Vector2[] uvs = new Vector2[4] {
				new Vector2(0.0f, 0.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
			};
		}
		public static class Button
        {
			public const float Width = 0.375f;
			public const float Heigth = 0.25f;
			public const float Depth = 0.125f;
			public const float WidthH = Width / 2f;
			public const float HeigthH = Heigth / 2f;
			public const float DepthH = Depth / 2f;

			public static readonly Vector3[] verts = new Vector3[8] {
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(Width, 0.0f, 0.0f),
				new Vector3(Width, Heigth, 0.0f),
				new Vector3(0.0f, Heigth, 0.0f),
				new Vector3(0.0f, 0.0f, Depth),
				new Vector3(Width, 0.0f, Depth),
				new Vector3(Width, Heigth, Depth),
				new Vector3(0.0f, Heigth, Depth)
			};

			public static readonly Vector2[,] uvs = new Vector2[,] {
				{ // along z
					new Vector2(0.0f, 0.0f),
					new Vector2(0.0f, Heigth),
					new Vector2(Width, 0.0f),
					new Vector2(Width, Heigth) 
				},
				{ // along y
					new Vector2(0.0f, 0.0f),
					new Vector2(0.0f, Depth),
					new Vector2(Width, 0.0f),
					new Vector2(Width, Depth)
				},
				{ // along x
					new Vector2(0.0f, 0.0f),
					new Vector2(0.0f, Heigth),
					new Vector2(Depth, 0.0f),
					new Vector2(Depth, Heigth)
				},
			};
		}
		public static class Rail
        {
			public const float Y = 0.5f;
			public const float DefaultHeight = 0.05f;
			public static readonly Vector3[] verts = new Vector3[]
			{
				new Vector3(0.0f, Y, 0.0f), // 0
				new Vector3(1.0f, Y, 0.0f), // 1
				new Vector3(0.0f, Y, 1.0f), // 2
				new Vector3(1.0f, Y, 1.0f), // 3
			};
			public static readonly Vector3[] vertsSlope = new Vector3[]
			{
				new Vector3(0.0f, 0.0f, 0.0f), // 0
				new Vector3(1.0f, 0.0f, 0.0f), // 1
				new Vector3(0.0f, 1.0f, 1.0f), // 2
				new Vector3(1.0f, 1.0f, 1.0f), // 3
			};
			public static readonly int[,] tris = new int[2, 4] {
				{0, 2, 1, 3}, // Top Face
				{1, 3, 0, 2}, // Bottom Face
			};
		}
		public static class Cobweb
        {
			public static readonly int[,] tris = new int[,] {
				{0, 3, 5, 6},
				{5, 6, 0, 3},
				{1, 2, 4, 7},
				{4, 7, 1, 2},
			};
		}
		public static class Redstone
		{
			public const float DefaultOffset = 0.01f;
			public const float toDot = 0.3125f;
			public const float toDot2 = 1f - toDot;
			public static readonly Vector3[] verts = new Vector3[]
			{
				new Vector3(0.0f, DefaultOffset, 0.0f), // 0
				new Vector3(1.0f, DefaultOffset, 0.0f), // 1
				new Vector3(0.0f, DefaultOffset, 1.0f), // 2
				new Vector3(1.0f, DefaultOffset, 1.0f), // 3
			};
			public static readonly Vector3[] vertsSide = new Vector3[]
			{
				new Vector3(DefaultOffset, 0.0f, 0.0f),
				new Vector3(DefaultOffset, 1.0f, 0.0f),
				new Vector3(DefaultOffset, 0.0f, 1.0f),
				new Vector3(DefaultOffset, 1.0f, 1.0f)
			};
			public static readonly int[,] tris = new int[2, 4] {
				{0, 2, 1, 3}, // Top Face
				{1, 3, 0, 2}, // Bottom Face
			};
			public static readonly int[,] trisSide = new int[2, 4] {
				{2, 3, 0, 1},
				{0, 1, 2, 3},
			};
			public static readonly Vector2[] uvsSide = new Vector2[4] {
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(0.0f, 1.0f)
			};
		}
		public static class Lever_Base
		{
			public const float X = 0.3125f;
			public const float X2 = 1f - X;
			public const float Y = 0f;
			public const float Y2 = 0.1875f;
			public const float Z = 0.25f;
			public const float Z2 = 1f - Z;
			public static readonly Vector3[] verts = new Vector3[8] {
				new Vector3(X, Y, Z), // 0
				new Vector3(X2, Y, Z), // 1
				new Vector3(X2, Y2, Z), // 2
				new Vector3(X, Y2, Z), // 3
				new Vector3(X, Y, Z2), // 4
				new Vector3(X2, Y, Z2), // 5
				new Vector3(X2, Y2, Z2), // 6
				new Vector3(X, Y2, Z2) //  7
			};
			public static readonly Vector2[,] uvs = new Vector2[3,4] { // Z Y X
				{
					new Vector2(X, Y),
					new Vector2(X, Y2),
					new Vector2(X2, Y),
					new Vector2(X2, Y2)
				},
				{
					new Vector2(X, Z),
					new Vector2(X, Z2),
					new Vector2(X2, Z),
					new Vector2(X2, Z2)
				},
				{
					new Vector2(Z, Y),
					new Vector2(Z, Y2),
					new Vector2(Z2, Y),
					new Vector2(Z2, Y2)
				},
			};
		}
		public static class Lever_Top
		{
			public const float X = 0.4375f;
			public const float X2 = 1f - X;
			public const float Y = 0f;
			public const float Y2 = 0.625f;
			public const float Y3 = Y2 - 0.125f;
			public static readonly Vector3[] verts = new Vector3[8] {
				new Vector3(X, Y, X), // 0
				new Vector3(X2, Y, X), // 1
				new Vector3(X2, Y2, X), // 2
				new Vector3(X, Y2, X), // 3
				new Vector3(X, Y, X2), // 4
				new Vector3(X2, Y, X2), // 5
				new Vector3(X2, Y2, X2), // 6
				new Vector3(X, Y2, X2) //  7
			};
			public static readonly Vector2[,] uvs = new Vector2[3, 4] { // Z Y X
				{
					new Vector2(X, Y),
					new Vector2(X, Y2),
					new Vector2(X2, Y),
					new Vector2(X2, Y2)
				},
				{
					new Vector2(X, Y3),
					new Vector2(X, Y2),
					new Vector2(X2, Y3),
					new Vector2(X2, Y2)
				},
				{
					new Vector2(X, Y),
					new Vector2(X, Y2),
					new Vector2(X2, Y),
					new Vector2(X2, Y2)
				},
			};
		}
	}
}
