using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDTree2
{
	public class Face
	{
		public List<int> Indices { get; set; }

		public Face(){
			Indices = new List<int>();
		}
	}
}
