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

		/// <summary>
		/// Return the Vector3 axis value at 'index'
		/// </summary>
		/// <param name="index">Index in to cell array.  0 = X, 1 = Y, 2 = Z</param>
		/// <returns>Single value at axis[index]</returns>
		public float Cell(int index) {
			return cell_[index];
		}

		/// <summary>
		/// Return the Vector3 axis value at 'axis
		/// </summary>
		/// <param name="axis">Axis enumeration</param>
		/// <returns>Single value at axis[index]</returns>
		public float Cell(Axis axis)
		{
			return Cell((int)axis);
		}
		/// <summary>
		/// Distance from Vector3 to Vector3
		/// </summary>
		/// <returns>Distance</returns>
		public float Distance(Vector3 right){
			Vector3 tmp = right - this;
			return tmp.Length();
		}
		
		/// <summary>
		/// Length (magnitude) of Vector3
		/// </summary>
		/// <returns></returns>
		public float Length(){
			return (float)Math.Sqrt(SqrLength());
		}

		/// <summary>
		/// Squared length of Vector3.
		/// </summary>
		/// <returns></returns>
		public float SqrLength()
		{
			return X * X + Y * Y + Z * Z;
		}

		/// <summary>
		/// Get the largest axis 
		/// </summary>
		/// <returns>Largest enumerated axis</returns>
		public Axis LargestAxis()
		{
			if (X > Y && Y > Z) { return Axis.X; }
			else if (Y > Z) { return Axis.Y; }
			else { return Axis.Z; ; }
		}
		
		/// <summary>
		/// Get the largest axis and the value of that axis
		/// </summary>
		/// <param name="value">Value of the largest axis</param>
		/// <returns>Largest enumerated axis</returns>
		public Axis LargestAxis(out float value)
		{
			Axis return_axis = LargestAxis();
			value = Cell((int)return_axis);
			return return_axis;
		}

		/// <summary>
		/// Get the largest axis and the value of that axis
		/// </summary>
		/// <param name="value">Value of the largest axis</param>
		/// <param name="axis"> Largest enumerated axis</param>
		public void LargestAxis(out Axis axis, out float value)
		{
			axis = LargestAxis();
			value = Cell((int)axis);
		}
		
		/// <summary>
		/// Find the maximum Vector3 of 2 vectors. Max value is compared along all axes 
		/// </summary>
		/// <example>Max((1,0,10),(2,0,0)) = (2,0,10)</example>
		/// <returns>Max Vector3 across all axes</returns>
		public static Vector3 Max(Vector3 left, Vector3 right) {
			float x = (left.X > right.X) ? left.X : right.X;
			float y = (left.Y > right.Y) ? left.Y : right.Y;
			float z = (left.Z > right.Z) ? left.Z : right.Z;

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Find the maximum Vector3 of 2 vectors. Max value is compared along all axes 
		/// </summary>
		/// <example>Max((1,0,10),(2,0,0)) = (2,0,10)</example>
		/// <returns>Max Vector3 across all axes</returns>
		public Vector3 Max(Vector3 right)
		{
			float x = (X > right.X) ? X : right.X;
			float y = (Y > right.Y) ? Y : right.Y;
			float z = (Z > right.Z) ? Z : right.Z;

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Find the minimum Vector3 of 2 vectors. Min value is compared along all axes 
		/// </summary>
		/// <example>Min((1,0,10),(2,0,0)) = (1,0,0)</example>
		/// <returns>Min Vector3 across all axes</returns>
		public static Vector3 Min(Vector3 left, Vector3 right)
		{
			float x = (left.X < right.X) ? left.X : right.X;
			float y = (left.Y < right.Y) ? left.Y : right.Y;
			float z = (left.Z < right.Z) ? left.Z : right.Z;

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Find the minimum Vector3 of 2 vectors. Min value is compared along all axes 
		/// </summary>
		/// <example>Min((1,0,10),(2,0,0)) = (1,0,0)</example>
		/// <returns>Min Vector3 across all axes</returns>
		public Vector3 Min(Vector3 right)
		{
			float x = (X < right.X) ? X : right.X;
			float y = (Y < right.Y) ? Y : right.Y;
			float z = (Z < right.Z) ? Z : right.Z;

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Find a series of metrics for the give Vector3 list.  
		/// </summary>
		/// <returns>TRUE if metrics were calculated, FALSE if not.</returns>
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

		/// <summary>
		/// Convert Vector3 to readable string.
		/// </summary>
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
