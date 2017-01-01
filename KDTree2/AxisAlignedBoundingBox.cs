using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDTree2
{
	public class AxisAlignedBoundingBox : IIntersector
	{
		/// <summary>
		/// Minimum corner of bounding box
		/// </summary>
		public Vector3 MinCorner	{ 
			get {
				return min_corner_;
			}
			set{
				min_corner_ = value;
				size_ = max_corner_ - min_corner_;
				center_ = (max_corner_ + min_corner_) / 2.0f;
			}
		}
		Vector3 min_corner_ = new Vector3();

		/// <summary>
		/// Maximum corner of bounding box (MinCorner + Size)
		/// </summary>
		public Vector3 MaxCorner
		{
			get
			{
				return max_corner_;
			}
			set
			{
				max_corner_ = value;
				size_ = max_corner_ - min_corner_;
				center_ = (max_corner_ + min_corner_) / 2.0f;
			}
		}
		Vector3 max_corner_ = new Vector3();

		/// <summary>
		/// Center of bounding box (MaxCorner + MinCorner) / 2
		/// </summary>
		public Vector3 Center
		{
			get
			{
				return center_;
			}
		}
		Vector3 center_;

		/// <summary>
		/// Size of bounding box (MaxCorner - MinCorner)
		/// </summary>
		public Vector3 Size
		{
			get
			{
				return size_;
			}
			set
			{
				size_ = value;
				half_size_ = size_ / 2.0f;
				max_corner_ = min_corner_ + Size;
				center_ = (max_corner_ + min_corner_) / 2.0f;
			}
		}
		Vector3 size_ = new Vector3();


		/// <summary>
		/// Half-Size of BoundingBox (Size / 2)
		/// </summary>
		public Vector3 HalfSize
		{
			get
			{
				return half_size_;
			}
			private set
			{
				half_size_ = value;
			}
		}
		Vector3 half_size_ = new Vector3();


		public AxisAlignedBoundingBox(){
			MinCorner = new Vector3();
			MaxCorner = new Vector3();
		}

		public AxisAlignedBoundingBox(Vector3 min, Vector3 max)
		{
			MinCorner = min;
			MaxCorner = max;
		}

		public AxisAlignedBoundingBox(AxisAlignedBoundingBox aabb)
		{
			MinCorner = aabb.MinCorner;
			MaxCorner = aabb.MaxCorner;
		}
		/// <summary>
		/// BoundingBox contains 'point'
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool Contains(Vector3 point){
			return (point.X >= min_corner_.X && point.X <= max_corner_.X &&
					point.Y >= min_corner_.Y && point.Y <= max_corner_.Y &&
					point.Z >= min_corner_.Z && point.Z <= max_corner_.Z);
		}

		/// <summary>
		/// BoundingBox intersects another BoundingBox
		/// </summary>
		/// <param name="aabb"></param>
		/// <returns></returns>
		public bool Intersects(AxisAlignedBoundingBox aabb){
			Vector3 v1 = aabb.MinCorner;
			Vector3 v2 = aabb.MaxCorner;

			Vector3 v3 = MinCorner;
			Vector3 v4 = MaxCorner;

			return ((v4.X >= v1.X) && (v3.X <= v2.X) &&		//x-axis overlap
					(v4.Y >= v1.Y) && (v3.Y <= v2.Y) &&		//y-axis overlap
					(v4.Z >= v1.Z) && (v3.Z <= v2.Z));		//z-axis overlap
		}
		public bool Intersects(AxisAlignedBoundingBox aabb, IntersectionTypes type)
		{
			return Intersects(aabb);
		}

		/// <summary>
		/// Does AxisAlignedBoundingBox intersect BoundingSphere
		/// </summary>
		/// <notes>
		/// Code modified from: http://tog.acm.org/resources/GraphicsGems/gems/BoxSphere.c 
		///	Support for mixed hollow/solid intersections was dropped.  Only hollow-hollow and solid-solid
		///	are supported
		/// </notes>
		/// <param name="intersection_type">Defines intersection method</param>
		/// <returns>True if sphere intersects bounding box, else FALSE</returns>
		public bool Intersects(BoundingSphere bs, IntersectionTypes intersection_type)
		{
			float a, b;
			float r2 = bs.Radius2;
			bool face;

			switch (intersection_type)
			{
				case IntersectionTypes.HOLLOW: // Hollow Box - Hollow Sphere
					{
						float dmin = 0;
						float dmax = 0;
						face = false;
						for (int i = 0; i < 3; i++)
						{
							a = (float)Math.Pow(bs.Center.Cell(i) - MinCorner.Cell(i), 2.0);
							b = (float)Math.Pow(bs.Center.Cell(i) - MaxCorner.Cell(i), 2.0);

							dmax += Math.Max(a, b);
							if (bs.Center.Cell(i) < MinCorner.Cell(i))
							{
								face = true;
								dmin += a;
							}
							else if (bs.Center.Cell(i) > MaxCorner.Cell(i))
							{
								face = true;
								dmin += b;
							}
							else if (Math.Min(a, b) <= r2)
							{
								face = true;
							}
						}
						if (face && (dmin <= r2) && (r2 <= dmax)) return true;
						break;
					}
				case IntersectionTypes.SOLID: // Solid Box - Solid Sphere
					{
						float dmin = 0;
						for (int i = 0; i < 3; i++)
						{
							if (bs.Center.Cell(i) < MinCorner.Cell(i))
							{
								dmin += (float)Math.Pow(bs.Center.Cell(i) - MinCorner.Cell(i), 2.0);
							}
							else if (bs.Center.Cell(i) > MaxCorner.Cell(i))
							{
								dmin += (float)Math.Pow(bs.Center.Cell(i) - MaxCorner.Cell(i), 2.0);
							}
						}
						if (dmin <= r2) return true;
						break;
					}
			}

			return false;
		}
		public bool Intersects(BoundingSphere bs)
		{
			return Intersects(bs, IntersectionTypes.SOLID);
		}
	}
}
