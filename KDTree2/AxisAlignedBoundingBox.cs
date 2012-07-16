using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDTree2
{
	public class AxisAlignedBoundingBox
	{
		public Vector3 MinCorner	{ 
			get {
				return min_corner_;
			}
			set{
				min_corner_ = value;
				size_ = max_corner_ - min_corner_;
			}
		}
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
			}
		}
		public Vector3 MidPoint
		{
			get
			{
				return (max_corner_ + min_corner_) / 2.0f;
			}
		}
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
			}
		}
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

		Vector3 min_corner_ = new Vector3();
		Vector3 max_corner_ = new Vector3();
		Vector3 size_		= new Vector3();
		Vector3 half_size_	= new Vector3();

		public AxisAlignedBoundingBox(){
			MinCorner = new Vector3();
			MaxCorner = new Vector3();
		}

		public AxisAlignedBoundingBox(Vector3 min, Vector3 max)
		{
			MinCorner = min;
			MaxCorner = max;
		}

		public bool Contains(Vector3 point){
			return (point.X >= min_corner_.X && point.X <= max_corner_.X &&
					point.Y >= min_corner_.Y && point.Y <= max_corner_.Y &&
					point.Z >= min_corner_.Z && point.Z <= max_corner_.Z);
		}

		public bool Intersects(AxisAlignedBoundingBox aabb){
			Vector3 v1 = aabb.MinCorner;
			Vector3 v2 = aabb.MaxCorner;

			Vector3 v3 = MinCorner;
			Vector3 v4 = MaxCorner;

			return ((v4.X >= v1.X) && (v3.X <= v2.X) &&		//x-axis overlap
					(v4.Y >= v1.Y) && (v3.Y <= v2.Y) &&		//y-axis overlap
					(v4.Z >= v1.Z) && (v3.Z <= v2.Z));		//z-axis overlap
		}
	}
}
