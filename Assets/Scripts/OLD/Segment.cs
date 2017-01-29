using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Segment
{
	public readonly List<Vector3> vertices = new List<Vector3>();
	public readonly Vector3 CENTER;
	private List<Segment> neighbours = new List<Segment>();
	public string name;

	public float minHeight;
	public float maxHeight;

	public float g = 0;
	public float h = 0;
	public float f = 0;

	public Segment pathfindingParent;

	public static List<Vector3> Combine(List<Vector3> a, List<Vector3> b) {
		List<Vector3> newVerts = new List<Vector3>(); //verts for new segment

		bool completed = false; //so we can only go through B once, not for every shared vertex in A

		for (int i = 0; i < a.Count; i++)
		{
			Vector3 vA = a[i];

			if (!IsInList(vA, newVerts)) { newVerts.Add(vA); }
			if (!completed && IsInList(a[i < a.Count ? i + 1 : 0], b)) //check if net vertex from current is a shared vertex, if not we add current and move to next vertex, same check
			{
				int index;
				IsInList(a[i < a.Count ? i + 1 : 0], b, out index); //if next vertex is in list, grab index of that vertex in listB

				//rotate the list towards the value we got through our index
				for (int x = 0; x < index; x++)
				{
					b.Add(b[0]);
					b.RemoveAt(0);
				}

				//add each B vertex not added by A yet
				foreach (Vector3 vB in b)
				{
					if (!IsInList(vB, newVerts)) { newVerts.Add(vB); }
				}

				completed = true;
			}
		}

		return newVerts;
	}

	public static bool IsConvex(List<Vector3> vertices)
	{
		if (vertices.Count < 4)
			return true;

		bool sign = false;
		int n = vertices.Count;
		for (int i = 0; i < n; i++)
		{
			float dx1 = vertices[((i + 2) % n)].x - vertices[((i + 1) % n)].x;
			float dy1 = vertices[((i + 2) % n)].y - vertices[((i + 1) % n)].y;
			float dx2 = vertices[(i)].x - vertices[((i + 1) % n)].x;
			float dy2 = vertices[(i)].y - vertices[((i + 1) % n)].y;

			float zcrossproduct = dx1 * dy2 - dy1 * dx2;
			if (i == 0)
				sign = zcrossproduct > 0;
			else
			{
				if (sign != (zcrossproduct > 0))
					return false;
			}
		}

		return true;
	}

	public static Segment Combine(Segment a, Segment b)
	{
		//PSEUDOCODE FOR COMBINE
		// create vertecies A
		// create vettices B

		// remove verticies that would be inside the polygon
		// calculate number of equal vertices
		// if number larger than 2 remove all vertices that in both lists and are folloed by another pair that is in both lists

		// itterating though all vertices of A
		// add to new list if not already in there
		// if vertice is in B
		// create new list order for B
		// add and remove index 0 until vertice equal vertice in forund
		// itterate through all of B and add vertice to new list if not already in there
		// add remianing vertices of A if not already in there

		//calculate new centroid
		//create new segment
		//return segment

		// add remaining vertices

		List<Vector3> newVerts = Combine(a.vertices, b.vertices);
		//Arithmetic mean approximation, this is a poor formula for complex shapes
		return new Segment(newVerts.ToArray()); //new segment with approximated cente, rearranged vertices/vertex indices
	}
	public static bool ContainsPoint(Segment seg, Vector3 point)
	{
		Vector3[] segverts = seg.vertices.ToArray();
		Vector2[] polyPoints = new Vector2[segverts.Length];
		Vector2 p = new Vector2(point.x, point.z);

		for (int i = 0; i < segverts.Length; i++)
		{
			polyPoints[i] = new Vector2(segverts[i].x, segverts[i].z);
		}

        int j = polyPoints.Length - 1;
		bool inside = false;
		for (int i = 0; i < polyPoints.Length; j = i++)
		{
			if (((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) &&
			   (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x))
				inside = !inside;
		}


		//Debug.Log(seg);
		if (point.y < seg.minHeight || point.y > seg.maxHeight) { inside = false; }

		return inside;
	}

	public Segment(params Vector3[] verticies)
	{
		for (int index = 0; index < verticies.Length; index++)
		{
			bool isEqual = false;

			for (int jndex = 0; jndex < this.vertices.Count; jndex++)
			{
				if (verticies[index] == this.vertices[jndex])
				{
					isEqual = true;
				}
			}

			if (!isEqual)
			{
				this.vertices.Add(verticies[index]);
			}
		}

		Vector3 center = Vector3.zero;

		foreach (Vector3 v in verticies)
		{
			center += v;
		}

		center /= verticies.Length;

		name = "無題";
		this.CENTER = center;
		CalculateHeights();
	}

	private void CalculateHeights()
	{
		float min = vertices[0].y;
		float max = vertices[0].y;

		for (int i = 1; i < vertices.Count; i++)
		{
			if (vertices[i].y <= min) { min = vertices[i].y; }
			if (vertices[i].y >= max) { max = vertices[i].y; }
		}

		minHeight = min;
		maxHeight = max;

		//Debug.Log(minHeight + " " + maxHeight + " ");
	}

	public bool IsAdjacent(Segment other)
	{
		if (other != this && !other.Equals(this))
		{
			int equalCounter = 0;

			for (int index = 0; index < this.vertices.Count; index++)
			{
				for (int jndex = 0; jndex < other.vertices.Count; jndex++)
				{
					if (vertices[index] == other.vertices[jndex])
					{
						equalCounter++;
					}
				}

				if (equalCounter > 1)
				{
					return true;
				}
			}
		}

		return false;
	}

	public static bool IsInList(Vector3 v, List<Vector3> list)
	{
		//THIS ONLY WORKS FOR VECTORS LOL ITS A HELPER FUNCTION
		bool isInList = false;

		for (int i = 0; i < list.Count; i++)
		{
			if (Vector3.Distance(v, list[i]) == 0)
			{
				isInList = true;
			}
		}

		return isInList;
	}
	public static bool IsInList(Vector3 v, List<Vector3> list, out int index)
	{
		//Same shit but with an out index, gives you the index on the element with the wanted value in the list
		bool isInList = false;
		index = -1;

		for (int i = 0; i < list.Count; i++)
		{
			if (Vector3.Distance(v, list[i]) == 0)
			{
				isInList = true;
				index = i;
			}
		}

		return isInList;
	}

	public override bool Equals(object obj)
	{
		if(this == obj) {
			return true;
		} else {
			Segment other = obj as Segment;

			if (GetHashCode() == obj.GetHashCode())
			{
				if (other != null)
				{
					if (vertices.Count != other.vertices.Count)
					{
						bool isEqual = true;

						for (int otherIndex = 0; otherIndex < other.vertices.Count; otherIndex++)
						{
							for (int thisIndex = 0; thisIndex < vertices.Count; thisIndex++)
							{
								if (other.vertices[otherIndex] != vertices[thisIndex])
								{
									isEqual = false;
								}
							}
						}

						if (isEqual == true)
						{
							return true;
						}
					}
				}
			}
		}

		

		return false;
	}

	public Vector3 this[int index]
	{
		get
		{
			return vertices[index];
		}
	}

	public int Count
	{
		get
		{
			return vertices.Count;
		}
	}

	public Segment GetNeighbour(int index)
	{
		return neighbours[index];
	}
	public void AddNeighbour(Segment item)
	{
		bool exists = false;

		foreach (Segment segment in neighbours)
		{
			if (segment == item)
			{
				exists = true;
			}
		}

		if (!exists)
		{
			neighbours.Add(item);
		}
	}

	public int NeighbourCount
	{
		get
		{
			return neighbours.Count;
		}
	}

	public void SetName(string name)
	{
		this.name = name;
	}

	public override int GetHashCode()
	{
		int code = 0;

		foreach (Vector3 vertex in vertices)
		{
			code += (int)vertex.x;
			code += (int)vertex.y;
			code += (int)vertex.z;
		}

		return vertices.Count;
	}

	public bool CompareWithVector(Vector3 other)
	{
		bool equal = true;

		if (!(other.x == CENTER.x)) equal = false;
		if (!(other.y == CENTER.y)) equal = false;
		if (!(other.z == CENTER.z)) equal = false;

		return equal;
	}
}
