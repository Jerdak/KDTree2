using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDTree2
{
	public class Vector3
	{
		public enum Axis { X, Y, Z };
		
		public float X { get { return x_; } set { x_ = value; cell_[0] = value; } }
		public float Y { get { return y_; } set { y_ = value; cell_[1] = value; } }
		public float Z { get { return z_; } set { z_ = value; cell_[2] = value; } }
		
		float x_ = 0.0f;
		float y_ = 0.0f;
		float z_ = 0.0f;

		float[] cell_ = new float[3] {0.0f,0.0f,0.0f};
		public Vector3()
		{
		}

		public Vector3(Vector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		public Vector3(float aX, float aY, float aZ)
		{
			X = aX;
			Y = aY;
			Z = aZ;
		}

		public float Cell(int index) {
			return cell_[index];
		}

		public float Distance(Vector3 right){
			Vector3 tmp = right - this;
			return tmp.Length();
		}
		public float Length(){
			return (float)Math.Sqrt(SqrLength());
		}
		public float SqrLength()
		{
			return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
		}
		public Axis LargestAxis()
		{
			if (X > Y && Y > Z) { return Axis.X; }
			else if (Y > Z) { return Axis.Y; }
			else { return Axis.Z; ; }
		}
		
		public Axis LargestAxis(out float value)
		{
			Axis return_axis = LargestAxis();
			value = Cell((int)return_axis);
			return return_axis;
		}

		public void LargestAxis(out Axis axis, out float value)
		{
			axis = LargestAxis();
			value = Cell((int)axis);
		}
		
		public static Vector3 Max(Vector3 left, Vector3 right) {
			float x = (left.X > right.X) ? left.X : right.X;
			float y = (left.Y > right.Y) ? left.Y : right.Y;
			float z = (left.Z > right.Z) ? left.Z : right.Z;

			return new Vector3(x, y, z);
		}
		public Vector3 Max(Vector3 right)
		{
			float x = (X > right.X) ? X : right.X;
			float y = (Y > right.Y) ? Y : right.Y;
			float z = (Z > right.Z) ? Z : right.Z;

			return new Vector3(x, y, z);
		}
		public static Vector3 Min(Vector3 left, Vector3 right)
		{
			float x = (left.X < right.X) ? left.X : right.X;
			float y = (left.Y < right.Y) ? left.Y : right.Y;
			float z = (left.Z < right.Z) ? left.Z : right.Z;

			return new Vector3(x, y, z);
		}
		public Vector3 Min(Vector3 right)
		{
			float x = (X < right.X) ? X : right.X;
			float y = (Y < right.Y) ? Y : right.Y;
			float z = (Z < right.Z) ? Z : right.Z;

			return new Vector3(x, y, z);
		}

		public static bool Metrics(List<Vector3> vertices, out Vector3 min, out Vector3 max, out Vector3 center, out Vector3 centroid, out Vector3 range){
			{
				min = new Vector3();
				max = new Vector3();
				center = new Vector3();
				centroid = new Vector3();
				range = new Vector3();
			}

			if (vertices.Count <= 0) return false;
			
			bool first_time = true;
			foreach (var vertex in vertices)
			{
				if (first_time) { min = max = vertex; first_time = false; }

				min = Vector3.Min(min, vertex);
				max = Vector3.Max(max, vertex);
				centroid += vertex;
			}
			center = (max + min) / 2.0f;
			centroid /= vertices.Count;
			range = max - min;

			return true;
		}

		public override string ToString(){
			StringBuilder sb = new StringBuilder();
			sb.Append(X);
			sb.Append(" ");
			sb.Append(Y);
			sb.Append(" ");
			sb.Append(Z);

			return sb.ToString();
		}

		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			return new Vector3 { X = left.X + right.X, Y = left.Y + right.Y, Z = left.Z + right.Z };
		}
		
		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			return new Vector3 { X = left.X - right.X, Y = left.Y - right.Y, Z = left.Z - right.Z };
		}
		
		public static Vector3 operator *(Vector3 left, Vector3 right)
		{
			return new Vector3 { X = left.X * right.X, Y = left.Y * right.Y, Z = left.Z * right.Z };
		}
		
		public static Vector3 operator *(Vector3 left, float scalar)
		{
			return new Vector3 { X = left.X * scalar, Y = left.Y * scalar, Z = left.Z * scalar };
		}

		public static Vector3 operator /(Vector3 left, Vector3 right)
		{
			return new Vector3 { X = left.X / right.X, Y = left.Y / right.Y, Z = left.Z / right.Z };
		}

		public static Vector3 operator /(Vector3 left, float scalar)
		{
			return new Vector3 { X = left.X / scalar, Y = left.Y / scalar, Z = left.Z / scalar };
		}

	}
}
