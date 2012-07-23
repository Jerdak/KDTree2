using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDTree2
{
	public class KDTreeNode
	{
		/// <summary>
		/// Minimum vertex in this node
		/// </summary>
		Vector3 min_						= new Vector3();
		
		/// <summary>
		/// Maximum vertex in this node
		/// </summary>
		Vector3 max_						= new Vector3();
		
		/// <summary>
		/// Center of this node's vertices
		/// </summary>
		Vector3 center_						= new Vector3();
		
		/// <summary>
		/// Range of this node's vertices
		/// </summary>
		Vector3 range_						= new Vector3();

		/// <summary>
		/// Child nodes
		/// </summary>
		KDTreeNode[] children_				= new KDTreeNode[2];				//Node children.
			
		/// <summary>
		/// Axis across which the data are split.  
		/// </summary>
		public Vector3.Axis SplitAxis		{ get { return split_axis_; } set { split_axis_ = value; } }
		Vector3.Axis split_axis_ = Vector3.Axis.X;
		
		/// <summary>
		/// Node axis aligned bounding box, bounds vertex data.
		/// </summary>
		public AxisAlignedBoundingBox AABB { get { if (!IsBuilt)Build(); return aabb_; } set { aabb_ = value; } }
		AxisAlignedBoundingBox aabb_ = new AxisAlignedBoundingBox();
		
		/// <summary>
		/// Node split pivot.  Will be the center of the data
		/// </summary>
		public Vector3 Pivot				{ get { return pivot_; } set { pivot_ = value; } }
		Vector3 pivot_ = new Vector3();
		
		/// <summary>
		/// Node parent.
		/// </summary>
		public KDTreeNode Parent			{ get { return parent_; } set { parent_ = value; } }
		KDTreeNode parent_ = null;
		
		/// <summary>
		/// Node sibling.
		/// </summary>
		public KDTreeNode Sibling { get { return sibling_; } set { sibling_ = value; } }
		KDTreeNode sibling_ = null;

		/// <summary>
		/// Buffered list of indexes.  Values stored here will index in to the KDTree.vertices List
		/// </summary>
		public List<int> Indices			{ get { return indices_; } private set { indices_ = value; }}
		List<int> indices_ = new List<int>();

		/// <summary>
		/// Number of indices
		/// </summary>
		public int Count					{ get { return Indices.Count; } }

		/// <summary>
		/// True iff node contains more than 0 indices
		/// </summary>
		public bool IsLeaf					{ get { return (Indices.Count>0);} }

		/// <summary>
		/// True iff node has been built, used internall for lazy initialization of certain class members. 
		/// </summary>
		bool IsBuilt					{ get; set; }

		public KDTreeNode(){
			IsBuilt = false;
		}
		
		public void Clear() {
			Indices.Clear();
		}

		/// <summary>
		/// Add a new vertex, and its index, to node.
		/// </summary>
		/// <param name="index">Index in to KDTree's vertices List</param>
		/// <param name="vertex">Actual vertex from KDTree's vertex list</param>
		public void AddVertex(int index, Vector3 vertex)
		{
			if(Count == 0){
				min_ = max_ = vertex;
			}
			min_ = Vector3.Min(min_, vertex);
			max_ = Vector3.Max(max_, vertex);
			Indices.Add(index);
			IsBuilt = false;
		}

		/// <summary>
		/// Utility method to build KDTreeNode variables, as long as min and max
		/// </summary>
		public void Build()
		{
			center_ = (max_ + min_) / 2.0f;
			range_ = max_ - min_;

			SplitAxis = range_.LargestAxis();
			Pivot = center_;
			AABB = new AxisAlignedBoundingBox(min_, max_);

			IsBuilt = true;
		}

		/// <summary>
		/// Get child node.
		/// </summary>
		/// <param name="index">Child index, must been either 0 or 1</param>
		/// <returns>Child node if 'index' is correct, otherwise null.</returns>
		public KDTreeNode Child(int index) { 
			if(index != 0 && index != 1) 
				return null;  
			return children_[index]; 
		}
			
		/// <summary>
		/// Return stored index.
		/// </summary>
		/// <param name="index">Index in to indices_ to return, must be between [0,indices_.Count]</param>
		/// <returns>Stored index value if 'index' is correct, else -1</returns>
		public int Index(int index) {
			if (index < 0 || index >= indices_.Count) return -1;
			return indices_[index]; 
		}

		/// <summary>
		/// Return the KDTreeNode that 'vertex' belongs to according to the split axis.
		/// </summary>
		/// <param name="vertex">3D vertex to bin</param>
		/// <returns>KDTreeNode containing 'vertex'</returns>
		public KDTreeNode GetSplitNode(Vector3 vertex) {
			if (!IsBuilt) Build();
			if (children_[0] == null || children_[1] == null) {
				children_[0] = new KDTreeNode { Parent = this };
				children_[1] = new KDTreeNode { Parent = this };

				children_[0].Sibling = children_[1];
				children_[1].Sibling = children_[0];
			}
		

			float mid_value = Pivot.Cell((int)SplitAxis);
			float pt_value = vertex.Cell((int)SplitAxis);

			KDTreeNode child = (pt_value >= mid_value) ? children_[0] : children_[1];

			return child;
		}
	}
}
