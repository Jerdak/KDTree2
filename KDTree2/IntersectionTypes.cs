using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDTree2
{
	public enum IntersectionTypes { HOLLOW, SOLID };

	// Avoid using Visitor pattern here, no need to separate intersector classes from intersection logic.  
	interface IIntersector {
		bool Intersects(AxisAlignedBoundingBox aabb, IntersectionTypes type);
		bool Intersects(BoundingSphere bs, IntersectionTypes type);
	};
}
