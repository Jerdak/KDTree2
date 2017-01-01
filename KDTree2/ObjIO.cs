using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KDTree2
{
	public class ObjIO
	{
		public static bool Read(string file_name, out List<Vector3> vertices, out List<Color> colors, out List<Face> faces)
		{
			vertices = new List<Vector3>();
			colors = new List<Color>();
			faces = new List<Face>();
			try {
				using( StreamReader sr = new StreamReader(file_name)){
					while(!sr.EndOfStream){
						string line = sr.ReadLine().Trim();
						string[] tokens = line.Split(' ');

						if(tokens.Length > 0 && tokens[0].Length > 0){
							
							switch (tokens[0][0]) {
								case 'v':
								case 'V':
									{
										if(tokens.Length == 4 ){
											Vector3 tmp = new Vector3();
											tmp.X = (float)Convert.ToDouble(tokens[1]);
											tmp.Y = (float)Convert.ToDouble(tokens[2]);
											tmp.Z = (float)Convert.ToDouble(tokens[3]);
											vertices.Add(tmp);
										} else if(tokens.Length == 6 ){
											Color tmp = new Color();
											tmp.R = (float)Convert.ToDouble(tokens[1]);
											tmp.G = (float)Convert.ToDouble(tokens[2]);
											tmp.B = (float)Convert.ToDouble(tokens[3]);
											colors.Add(tmp);
										}
										else {
											Console.WriteLine("Ill formed vertex: " + line);
										}
									}
								break;
								case 'f':
								case 'F':
									{
										Face tmp = new Face();
										
										for(int i = 1; i < tokens.Length; i++){
										
											tmp.Indices.Add(Convert.ToInt32(tokens[i]));
										}
										faces.Add(tmp);
									}
									//TODO: Add these
								break;
							};
						}
					}
				}
			}catch (Exception ex){
				Console.WriteLine(ex.Message + " , " + ex.StackTrace);
				return false;
			}
			return true;
		}

		public static bool Write(string file_name, List<Vector3> vertices, List<Color> colors, List<Face> faces)
		{
			using (StreamWriter sw = new StreamWriter(file_name)){
				if(vertices.Count != colors.Count){
					Console.WriteLine("[Warning] - Can't export obj color, color array length doesn't match vertex array length");
					foreach (var vertex in vertices){
						sw.WriteLine("v " + vertex.X.ToString() + " " + vertex.Y.ToString() + " " + vertex.Z.ToString());
					}
				} else {
					for(int i = 0; i < vertices.Count; i++)
					{
						Vector3 vertex = vertices[i];
						Color color = colors[i];
						sw.WriteLine("v " + vertex.X.ToString() + " " + vertex.Y.ToString() + " " + vertex.Z.ToString() + " "
										  + color.R.ToString() + " " + color.G.ToString() + " " + color.B.ToString());
					}
				}
				foreach (var face in faces)
				{
					sw.Write("f ");
					foreach(var index in face.Indices){
						sw.Write(index.ToString() + " ");
					}
					sw.WriteLine("");
				}
			}
			return true;
		}
	}
}
