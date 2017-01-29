using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NavigationMesh : MonoBehaviour
{
	public delegate void NavigationMeshChangeListener();

	public NavigationMeshChangeListener navigationMeshChangeHandler;

	public GameObject navMesh = null;
	private Mesh mesh;
	private readonly List<Segment> SEGMENTS = new List<Segment>();

	public List<Vector3> graphNodes = new List<Vector3>();
	public List<Connection> navmeshNodes = new List<Connection>();

	//Use this for initialization
	void Start()
	{
		mesh = navMesh.GetComponent<MeshFilter>().mesh;

		Vector3[] vertices = mesh.vertices;
		int[] tris = mesh.triangles;

		//Debug.Log(vertices.Length + " " + tris.Length);

		for (int i = 0; i < tris.Length; i += 3)
		{
			Vector3 a = vertices[tris[i]] + navMesh.gameObject.transform.position;
			Vector3 b = vertices[tris[i + 1]] + navMesh.gameObject.transform.position;
			Vector3 c = vertices[tris[i + 2]] + navMesh.gameObject.transform.position;

			Vector3 center = (a + b + c) / 3;
			SEGMENTS.Add(new Segment(a, b, c));
		}

		/*
		for (int i = 0; i < SEGMENTS.Count; i++)
		{
			Debug.Log(SEGMENTS.Count);
			bool hasCombined = false;

			do
			{
				hasCombined = false;

				for (int j = 0; j < SEGMENTS.Count; j++)
				{
					Debug.Log(i + " " + j);
					if (SEGMENTS[i] != SEGMENTS[j] && SEGMENTS[i].IsAdjacent(SEGMENTS[j]) && Segment.IsConvex(Segment.Combine(SEGMENTS[i].vertices, SEGMENTS[j].vertices)))
					{
						Segment newSegment = Segment.Combine(SEGMENTS[i], SEGMENTS[j]);

						SEGMENTS[i] = newSegment;
						SEGMENTS.RemoveAt(j);

						Debug.Log("Combine");

						hasCombined = true;
					}
				}
			} while (hasCombined);
		}
		/**/

		/*
		//Decide which triangles to combine goes here!
		SEGMENTS[0] = Segment.Combine(SEGMENTS[0], SEGMENTS[1]);
		SEGMENTS.RemoveAt(1); //REMEMBER TO REMOVE THE SEGMENTS YOU COMBINED

		SEGMENTS[0] = Segment.Combine(SEGMENTS[0], SEGMENTS[1]);
		SEGMENTS.RemoveAt(1);
		/**/
		//Debug.Log(Segment.IsConvex(SEGMENTS[0].vertices));


		BuildNavGraph(new List<Segment>(), SEGMENTS[0], null);

		for (int i = 0; i < SEGMENTS.Count; i++)
		{
			for (int j = 0; j < SEGMENTS[i].NeighbourCount; j++)
			{
				Connection con = new Connection(SEGMENTS[i], SEGMENTS[i].GetNeighbour(j));
				if (!ConnectionExists(con))
				{
					navmeshNodes.Add(con);
				}

			}
		}

		BuildNavGraph(new List<Connection>(), navmeshNodes[0], null);
		Debug.Log(navmeshNodes.Count);

		navigationMeshChangeHandler();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public bool Contains(Vector3 point)
	{
		if (FindSegment(point) != null)
		{
			return true;
		}
		return false;
	}

	private Segment FindSegment(Vector3 point)
	{
		bool found = false;
		for (int i = 0; i < SEGMENTS.Count; i++)
		{
			found = Segment.ContainsPoint(SEGMENTS[i], point);

			if (found)
			{
				return SEGMENTS[i];
			}
		}

		return null;
	}

	private Connection FindConnection(Vector3 point)
	{
		bool found = false;
		for (int i = 0; i < SEGMENTS.Count; i++)
		{
			found = Segment.ContainsPoint(SEGMENTS[i], point);

			if (found)
			{
				return null;
			}
		}

		return null;
	}

	private void BuildNavGraph(List<Segment> processed, Segment current, Segment last)
	{
		processed.Add(current);

		if (last != null) { current.AddNeighbour(last); }

		for (int index = 0; index < SEGMENTS.Count; index++)
		{
			if (SEGMENTS[index] != last && SEGMENTS[index] != current && current.IsAdjacent(SEGMENTS[index]))
			{
				current.AddNeighbour(SEGMENTS[index]);

				if (!processed.Contains(SEGMENTS[index]))
				{
					BuildNavGraph(processed, SEGMENTS[index], current);
				}
				else
				{
					//Debug.Log("Walalala");
				}
			}
		}
	}

	private void BuildNavGraph(List<Connection> processed, Connection current, Connection last)
	{
		processed.Add(current);

		if (last != null) { current.neighbours.Add(last); }

		for (int index = 0; index < SEGMENTS.Count; index++)
		{
			if (navmeshNodes[index] != last && navmeshNodes[index] != current && current.IsAdjacent(navmeshNodes[index]))
			{
				current.neighbours.Add(navmeshNodes[index]);

				if (!processed.Contains(navmeshNodes[index]))
				{
					BuildNavGraph(processed, navmeshNodes[index], current);
				}
				else
				{
					//Debug.Log("Walalala");
				}
			}
		}
	}
	public bool ConnectionExists(Connection a)
	{
		for (int i = 0; i < navmeshNodes.Count; i++)
		{
			int counter = 0;
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 2; y++)
				{
					if (a.segments[x].CENTER == navmeshNodes[i].segments[y].CENTER)
					{
						counter++;
					}
				}
			}
			if (counter == 2) { return true; }
		}


		return false;
	}
	public List<Vector3> CalculatePath(Vector3 start, Vector3 end)
	{
		// find segment based on start find segment based on end

		List<Segment> openList = new List<Segment>();
		List<Segment> closedList = new List<Segment>();

		Segment s = FindSegment(start);
		Segment e = FindSegment(end);

		//Debug.Log(e.CENTER);

		openList.Add(s); //add start
		while (openList.Count > 0)
		{ //loop while open isnt empty
			Segment current = openList[0]; //current is start

			//sorting the list w/o linq - finding out which has lowest F
			for (int i = 1; i < openList.Count; i++)
			{ // i = 1 because 0 is the default and we dont need to compare it to itself
				if (openList[i].f < current.f)
				{ // || f == f && current.h > [i].h
					current = openList[i]; //current is the lowest f cost in the list, can now continue with current
				}
			}

			openList.Remove(current);
			closedList.Add(current);

			Debug.Log(current.CENTER + " " + e.CENTER);

			//Drawing the path back, blatantly stole that one from ma boi Li Gang
			if (current == e)
			{
				List<Vector3> path = new List<Vector3>();
				Segment tracer = e;
				Segment last;

				while (tracer != s)
				{
					//smart
					tracer = tracer.pathfindingParent;
					last = tracer;
					path.Add(GetConnection(tracer, last).position);
				}

				path.Add(start);
				path.Reverse(); //cus path is end to start, visually it looks the same, but if we would walk the path without, it would be wrong way around
				//path.RemoveAt(1);
				Debug.Log(path.Count);
				GetComponent<PathVisualizer>().VisualizePath(path.ToArray());

				return path;
			}

			Debug.Log(current.NeighbourCount + " " + closedList.Count);
			for (int i = 0; i < current.NeighbourCount; i++)
			{
				Segment seg = current.GetNeighbour(i);

				if (closedList.Contains(seg))
				{
					Debug.Log("cont");
					continue;
				}

				float cost = current.g + CalcDistance(current.CENTER, seg.CENTER); //Cost calculation with Distance, stolen from wikipedia
				if (cost < seg.g || !openList.Contains(seg))
				{

					seg.h = CalcDistance(seg.CENTER, end); // h Cost is distance left to end node
					seg.g = cost; //g Cost is distance from current to this node
					seg.f = seg.h + seg.g;
					seg.pathfindingParent = current; //for tracing path later

					if (!openList.Contains(seg))
					{
						openList.Add(seg); //adds n if its not in open yet so we can sort it on top and continue form here if its the fastest so far
					}
				}
			}
		}

		return null;
	}

	private Connection GetConnection(Segment a, Segment b)
	{
		for (int i = 0; i < navmeshNodes.Count; i++)
		{
			if (navmeshNodes[i].segments.Contains(a) && navmeshNodes[i].segments.Contains(b))
			{
				return navmeshNodes[i];
			}
		}

		return null;
	}

	public List<Vector3> CalculatePathConnections(Vector3 start, Vector3 end)
	{
		// find segment based on start find segment based on end

		List<Connection> openList = new List<Connection>();
		List<Connection> closedList = new List<Connection>();

		Connection s = FindConnection(start);
		Connection e = FindConnection(end);

		//Debug.Log(e.CENTER);

		openList.Add(s); //add start
		while (openList.Count > 0)
		{ //loop while open isnt empty
			Connection current = openList[0]; //current is start

			//sorting the list w/o linq - finding out which has lowest F
			for (int i = 1; i < openList.Count; i++)
			{ // i = 1 because 0 is the default and we dont need to compare it to itself
				if (openList[i].f < current.f)
				{ // || f == f && current.h > [i].h
					current = openList[i]; //current is the lowest f cost in the list, can now continue with current
				}
			}

			openList.Remove(current);
			closedList.Add(current);

			Debug.Log(current.position + " " + e.position);

			//Drawing the path back, blatantly stole that one from ma boi Li Gang
			if (current == e)
			{
				List<Vector3> path = new List<Vector3>();
				Connection tracer = e;

				while (tracer != s)
				{
					//smart
					path.Add(tracer.position);
					tracer = tracer.pathfindingParent;
				}

				path.Add(start);
				path.Reverse(); //cus path is end to start, visually it looks the same, but if we would walk the path without, it would be wrong way around
				Debug.Log(path.Count);
				GetComponent<PathVisualizer>().VisualizePath(path.ToArray());

				return path;
			}

			Debug.Log(current.neighbours.Count + " " + closedList.Count);
			for (int i = 0; i < current.neighbours.Count; i++)
			{
				Connection con = current.neighbours[i];

				if (closedList.Contains(con))
				{
					Debug.Log("cont");
					continue;
				}

				float cost = current.g + CalcDistance(current.position, con.position); //Cost calculation with Distance, stolen from wikipedia
				if (cost < con.g || !openList.Contains(con))
				{

					con.h = CalcDistance(con.position, end); // h Cost is distance left to end node
					con.g = cost; //g Cost is distance from current to this node
					con.f = con.h + con.g;
					con.pathfindingParent = current; //for tracing path later

					if (!openList.Contains(con))
					{
						openList.Add(con); //adds n if its not in open yet so we can sort it on top and continue form here if its the fastest so far
					}
				}
			}
		}

		return null;
	}

	public float CalcDistance(Vector3 a, Vector3 b)
	{
		return Vector3.Distance(a, b);
	}

	public int Count
	{
		get
		{
			return SEGMENTS.Count;
		}
	}

	public Segment Get(int index)
	{
		return SEGMENTS[index];
	}
}
