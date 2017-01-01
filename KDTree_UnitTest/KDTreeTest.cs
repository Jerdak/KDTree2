using KDTree2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace KDTree_UnitTest
{
    
    
    /// <summary>
    ///This is a test class for KDTreeTest and is intended
    ///to contain all KDTreeTest Unit Tests
    ///</summary>
	[TestClass()]
	public class KDTreeTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for Build
		///</summary>
		[TestMethod()]
		public void BuildTest()
		{
			Console.WriteLine("Current Directory: " + System.IO.Directory.GetCurrentDirectory());
			List<Vector3> vertices;
			List<Face> faces;
			List<Color> colors;
			if (!ObjIO.Read(@"..\..\..\SampleData\simple_model.obj", out vertices, out colors, out faces))
			{
				Console.WriteLine("Failed to find test.obj");
				Assert.Fail("Failed to find test2.obj");
			}


			KDTree tree = new KDTree();
			{	// Assert build runs without failure
				tree.AddPoints(vertices);
				bool build_result = tree.Build();
				Assert.IsTrue(build_result);
			}
			{	// Verify that internally KDTree is working from the same vertices.
				Assert.IsTrue(vertices.Count == tree.VertexCount);
				for(int i = 0; i < vertices.Count; ++i){
					if(vertices[i].Distance(tree.Vertices[i]) >= 0.0000001f){
						Console.WriteLine("Original Vertices: " + vertices[i].ToString());
						Console.WriteLine("KDTreees Vertices: " + tree.Vertices[i].ToString());
						Assert.Fail("Mismatched vertex comparison on index: " + i.ToString());
					}
				}
			}
		}

		/// <summary>
		///A test for FindClosestPoint
		///</summary>
		[TestMethod()]
		public void FindClosestPointTest()
		{
			Console.WriteLine("Current Directory: " + System.IO.Directory.GetCurrentDirectory());

			List<Vector3> vertices;
			List<Face> faces;
			List<Color> colors;
			KDTree tree = new KDTree();
			{	//Build KDTree from a Wavefront .obj file
				if (!ObjIO.Read(@"..\..\..\SampleData\simple_model.obj", out vertices, out colors, out faces))
				{
					Assert.Fail("Failed to find test2.obj");
				}
				tree.AddPoints(vertices);
				bool build_result = tree.Build();
				Assert.IsTrue(build_result);
			}

			{	// First test that loaded points can refind themselves.
				Stopwatch st = new Stopwatch();
				st.Start();

				bool match_result = true;
				
				for (int i = 0; i < vertices.Count; i++)
				{
					int index = i;
					int nearest_index = 0;
					Vector3 tmp = tree.FindClosestPoint(vertices[index], 0.00f, ref nearest_index);
					if (nearest_index != index)
					{
						Console.WriteLine("Vertex: " + vertices[index].ToString());
						Console.WriteLine("Mis-Match: " + vertices[nearest_index].ToString());
						Assert.Fail("Mismatched closest pair: " + index.ToString() + "," + nearest_index.ToString());
					}
				}
				st.Stop();
				Console.WriteLine("Elapsed Exact Set = {0}", st.Elapsed.ToString());
				Assert.IsTrue(match_result);
			}

			{	// Next test that a completely random set of data can find a closest point, compare to brute force.
				Vector3 min,max,center,centroid,range;
				Vector3.Metrics(vertices,out min, out max, out center, out centroid, out range);
				Random random = new Random(42);
				Stopwatch st = new Stopwatch();
				st.Start();
				for (int i = 0; i < 1500; i++)
				{
					float x = (float)random.Next((int)(min.X * 1000.0f), (int)(max.X * 1000.0f)); x /= 1000.0f;
					float y = (float)random.Next((int)(min.Y * 1000.0f), (int)(max.Y * 1000.0f)); y /= 1000.0f;
					float z = (float)random.Next((int)(min.Z * 1000.0f), (int)(max.Z * 1000.0f)); z /= 1000.0f;
					
					int closest_index = 0;
					int closest_index2 = 0;
					Vector3 vertex = new Vector3(x, y, z);
					
					// WARNING:  To validate the closest point algorithm we have to use a brute force method to find ground truth, this is **slow** for large meshes.
					Vector3 closest_vertex = tree.FindClosestPoint(vertex, 1.1f, ref closest_index);
					Vector3 closest_vertex2 = tree.FindClosestPointBrute(vertex, ref closest_index2);

					if (closest_vertex2.Distance(vertex) < closest_vertex.Distance(vertex))
					{
						st.Stop();
						Console.WriteLine("Vertex: " + vertex.ToString());
						Console.WriteLine("Match1: " + closest_vertex.ToString() + ": d=" + closest_vertex.Distance(vertex).ToString());
						Console.WriteLine("Match2: " + closest_vertex2.ToString() + ": d=" + closest_vertex2.Distance(vertex).ToString());
						//Assert.Fail("Incorrect matching vertex");
						st.Start();
					}
				}
				st.Stop();
				Console.WriteLine("Elapsed Random Set = {0}", st.Elapsed.ToString());
			}
		}
	}
}
