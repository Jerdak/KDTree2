using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDTree2
{
	public class KDTree
	{
		List<Vector3> vertices_ = new List<Vector3>();
		public List<Vector3> Vertices { get{ return vertices_;} private set { vertices_ = value; } }

		KDTreeNode root_ = new KDTreeNode();
		public KDTreeNode Root { get { return root_; } private set { root_ = value; } }

		/// <summary>
		/// Number of nodes in the tree (Currently Unused)
		/// </summary>
		public int NodeCount { get; set; }

		/// <summary>
		/// Return the number of stored vertices.
		/// </summary>
		public int VertexCount { get { return Vertices.Count; } }

		/// <summary>
		/// Maximum allowable node size (# of vertices in a node)
		/// </summary>
		public int MaxNodeSize { get; set; }

		/// <summary>
		/// Maximum allowable recursion depth for building the KDTree
		/// </summary>
		public int MaxNodeDepth { get; set; }

		public KDTree(){
			NodeCount = 0;
			MaxNodeSize = 100;
			MaxNodeDepth = 25;
			
		}

		/// <summary>
		/// Add a new vertex/point to the KDTree build process.  Points must be added before calling KDTree.Build()
		/// </summary>
		/// <param name="vertex">3D vertex</param>
		public void AddPoint(Vector3 vertex) {
			Vertices.Add(vertex);
		}

		/// <summary>
		/// Add a new vertex/point to the KDTree build process.  Points must be added before calling KDTree.Build()
		/// </summary>
		/// <param name="x">Vector3.x</param>
		/// <param name="y">Vector3.y</param>
		/// <param name="z">Vector3.z</param>
		public void AddPoint(float x, float y, float z)
		{
			Vertices.Add(new Vector3(x,y,z));
		}

		/// <summary>
		/// Add a new list of vertices/points to the KDTree build process.  Points must be added before calling KDTree.Build()
		/// </summary>
		/// <param name="vertex">List of 3D vertices</param>
		public void AddPoints(List<Vector3> vertices)
		{
			Vertices = Vertices.Concat(vertices).ToList();
		}

		/// <summary>
		/// Add a new list of vertices/points to the KDTree build process.  Points must be added before calling KDTree.Build()
		/// </summary>
		/// <param name="vertex">List of 3D vertices stored as a 1D array w/ every 3 elements defining a vertex.</param>
		public void AddPoints(List<float> vertices)
		{
			int element_size = 3;
			if (vertices.Count % element_size != 0) return;	//vertex count must be a multiple of element_size
			for(int i = 0; i < vertices.Count; i+=element_size){
				float x = vertices[i];
				float y = vertices[i+1];
				float z = vertices[i+2];
				Vertices.Add(new Vector3(x, y, z));
			}
			
		}
		/// <summary>
		/// Build KDTree
		/// </summary>
		/// <notes>
		/// Points are added via KDTree.AddPoint(s) methods, the Build process uses
		/// those points to create the final tree.  If new points are added after a
		/// tree has been built, a new tree must be created.
		/// </notes>
		/// <returns></returns>
		public bool Build() {
			if(VertexCount <= 0) {
				Console.WriteLine("[Warning] - No vertices added to KDTree, aborting build.");
				return false;
			}
			Root = new KDTreeNode();// { SplitAxis = largest_split_axis, AABB = new AxisAlignedBoundingBox(min,max), MidPoint = center, Parent = null};
			
			{	// Fill the Root
				int index = 0;
				foreach (var vertex in Vertices ){
					root_.AddVertex(index, vertex);
					index++;
				}
			}

			SplitNode(Root, 0);
			return true;
		}

		/// <summary>
		/// Recursively split node across its pivot axis.
		/// </summary>
		/// <param name="node">KDTreeNode to split</param>
		/// <param name="depth">Current recursion depth, set lower if you get stack overflow</param>
		/// <returns>True if split was a success or max_depth/max_node_size criterion met</returns>
		bool SplitNode(KDTreeNode node, int depth){
			if (depth >= MaxNodeDepth) return true;
			if (node.Count <= MaxNodeSize) return true;

			foreach(var index in node.Indices){
				Vector3 vertex = Vertices[index];

				KDTreeNode child = node.GetSplitNode(vertex);
				child.AddVertex(index, vertex);
			}

			// TODO:  Do we need to check if either child is empty?  Since we're calculating the split axis
			//		  using raw data it's unlikely.
			node.Clear();
			node.Child(0).Build();
			node.Child(1).Build();

			SplitNode(node.Child(0), depth + 1);
			SplitNode(node.Child(1), depth + 1);
			return true;
		}

		/// <summary>
		/// Iterate through System.IO.Collections.Generic.List<int> to find closest matching vertex
		/// </summary>
		/// <param name="indices">List of integer indices that index vertices</param>
		/// <param name="vertex">Vertex to match</param>
		/// <returns>Closest stored 'vertex' to given 'vertex'</returns>
		Vector3 GetClosestVertexFromIndices(List<int> indices, Vector3 vertex, ref int nearest_index)
		{
			int min_index = -1;
			float min_dist = 0.0f;

			foreach (var index in indices)
			{
				Vector3 tmp_vertex = Vertices[index];
				if (min_index == -1)
				{
					min_dist = tmp_vertex.Distance(vertex);
					min_index = index;
				}
				else
				{
					float tmp_dist = tmp_vertex.Distance(vertex);
					if (tmp_dist < min_dist)
					{
						min_dist = tmp_dist;
						min_index = index;
					}
				}
			}
			nearest_index = min_index;
			return Vertices[min_index];
		}
		/// <summary>
		/// Find closest point using an axis aligned search boundary
		/// </summary>
		/// <param name="node"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		Vector3 FindClosestPoint(KDTreeNode node, Vector3 vertex, AxisAlignedBoundingBox search_bounds, ref int nearest_index)
		{
			int tmp_index = -1;
			if(node.IsLeaf){
				tmp_index = -1;
				Vector3 result = GetClosestVertexFromIndices(node.Indices, vertex, ref tmp_index);
				nearest_index = tmp_index;
				return result;
			}

			tmp_index = -1;
			KDTreeNode near_child = node.GetSplitNode(vertex);
			Vector3 near_result = FindClosestPoint(near_child, vertex, search_bounds, ref tmp_index);
			Vector3 ret = near_result;
			float near_distance = near_result.Distance(vertex);
			nearest_index = tmp_index;
			
			KDTreeNode far_child = near_child.Sibling;
		
			if(far_child.AABB.Intersects(search_bounds)){
				Vector3 far_result = FindClosestPoint(far_child, vertex, search_bounds, ref tmp_index);
				float far_distance = far_result.Distance(vertex);
				if (far_distance < near_distance) {
					nearest_index = tmp_index;
					ret = far_result;
				}
			}
			return ret;
		}

		/// <summary>
		/// Find closest point using a centered bounding sphere
		/// </summary>
		/// <param name="node"></param>
		/// <param name="vertex"></param>
		/// <param name="search_bounds"></param>
		/// <param name="nearest_index"></param>
		/// <returns></returns>
		Vector3 FindClosestPoint(KDTreeNode node, Vector3 vertex, BoundingSphere search_bounds, ref int nearest_index)
		{
			int tmp_index = -1;
			if (node.IsLeaf)
			{
				tmp_index = -1;
				Vector3 result = GetClosestVertexFromIndices(node.Indices, vertex, ref tmp_index);
				nearest_index = tmp_index;
				return result;
			}

			tmp_index = -1;
			KDTreeNode near_child = node.GetSplitNode(vertex);
			Vector3 near_result = FindClosestPoint(near_child, vertex, search_bounds, ref tmp_index);
			Vector3 ret = near_result;
			float near_distance = near_result.Distance(vertex);
			nearest_index = tmp_index;

			KDTreeNode far_child = near_child.Sibling;

			if (far_child.AABB.Intersects(search_bounds))
			{
				Vector3 far_result = FindClosestPoint(far_child, vertex, search_bounds, ref tmp_index);
				float far_distance = far_result.Distance(vertex);
				if (far_distance < near_distance)
				{
					nearest_index = tmp_index;
					ret = far_result;
				}
			}
			return ret;
		}

		/// <summary>
		/// Find the closest matching vertex
		/// </summary>
		/// <param name="vertex">Vertex to find closest match.</param>
		/// <param name="distance">Search distance.</param>
		/// <param name="nearest_index">Index of matching vertex in the KDTree vertex array</param>
		/// <returns>Nearest matching vertex</returns>
		public Vector3 FindClosestPoint(Vector3 vertex, float distance, ref int nearest_index)
		{
			if(root_ == null){
				Console.WriteLine("Null root, Build() must be called before using the KDTree.");
				return new Vector3(0, 0, 0);
			}
			Vector3 distance_vector = new Vector3(distance,distance,distance);
			AxisAlignedBoundingBox search_bounds = new AxisAlignedBoundingBox(vertex - distance_vector, vertex + distance_vector);
			return FindClosestPoint(Root, vertex, search_bounds, ref nearest_index);
		}

		/// <summary>
		/// FInd the closest matching point using a full For-loop search: O(n)
		/// </summary>
		/// <param name="vertex">Vertex to match</param>
		/// <param name="nearest_index">Index of matching vertex in the KDTree vertex array</param>
		/// <returns>Nearest matching vertex</returns>
		public Vector3 FindClosestPointBrute(Vector3 vertex, ref int nearest_index){
			float min_dist = 0.0f;
			int min_index = -1;
			for (int j = 0; j < Vertices.Count; j++)
			{
				Vector3 tmp_vertex = Vertices[j];
				float distance = tmp_vertex.Distance(vertex);
				if (min_index == -1)
				{
					min_dist = distance;
					min_index = j;
				}
				else
				{
					if (distance < min_dist)
					{
						min_dist = distance;
						min_index = j;
					}
				}
			}
			nearest_index = min_index;
			return Vertices[min_index];
		}
		/// <summary>
		/// Purge vertex memory and root
		/// </summary>
		public void Clear() {
			Vertices.Clear();
			root_ = null;
		}
	}
}
