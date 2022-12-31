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
		public const int WorldSizeInChunks = 5;

		public static readonly Vector3[] voxelVerts = new Vector3[8] {
			new Vector3(0.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 1.0f, 0.0f),
			new Vector3(0.0f, 1.0f, 0.0f),
			new Vector3(0.0f, 0.0f, 1.0f),
			new Vector3(1.0f, 0.0f, 1.0f),
			new Vector3(1.0f, 1.0f, 1.0f),
			new Vector3(0.0f, 1.0f, 1.0f)
		};

		public static readonly Vector3i[] faceChecks = new Vector3i[6] {
			new Vector3i(0, 0, -1),
			new Vector3i(0, 0, 1),
			new Vector3i(0, 1, 0),
			new Vector3i(0, -1, 0),
			new Vector3i(-1, 0, 0),
			new Vector3i(1, 0, 0)
		};

		public static readonly int[,] voxelTris = new int[6, 4] {
			// 0 1 2 2 1 3
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
			const float first1 = 1f / 3f;
			const float first2 = 2f / 3f;

			public static readonly Vector2[][] voxelUvs = new Vector2[][] {
				new Vector2[] { // Back Face
					new Vector2(0.5f, first1),
					new Vector2(0.5f, first2),
					new Vector2(0.25f, first1),
					new Vector2(0.25f, first2)
				},
				new Vector2[] { // Front Face
					new Vector2(0.75f, first1),
					new Vector2(0.75f, first2),
					new Vector2(1.0f, first1),
					new Vector2(1.0f, first2)
				},
				new Vector2[] { // Top Face
					new Vector2(0.5f, 0f),
					new Vector2(0.5f, first1),
					new Vector2(0.25f, 0f),
					new Vector2(0.25f, first1)
				},
				new Vector2[] { // Bottom Face
					new Vector2(0.5f, first2),
					new Vector2(0.5f, 1f),
					new Vector2(0.25f, first2),
					new Vector2(0.25f, 1f)
				},
				new Vector2[] { // Left Face
					new Vector2(0.25f, first1),
					new Vector2(0.25f, first2),
					new Vector2(0.0f, first1),
					new Vector2(0.0f, first2)
				},
				new Vector2[] { // Right Face
					new Vector2(0.75f, first1),
					new Vector2(0.75f, first2),
					new Vector2(0.5f, first1),
					new Vector2(0.5f, first2)
				},
			};
			public static readonly int[,] tris = new int[6, 4] {
				// 0 1 2 2 1 3
				{0, 3, 1, 2}, // Back Face
				{5, 6, 4, 7}, // Front Face
				{3, 7, 2, 6}, // Top Face
				{1, 5, 0, 4}, // Bottom Face
				{4, 7, 0, 3}, // Left Face
				{1, 2, 5, 6} // Right Face
			};
		}
	}
}
