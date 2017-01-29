using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavigationMesh))]
public class NavigationMeshVisualizer : MonoBehaviour
{

	public Material lineColor = null;
	private GameObject container = null;
	private NavigationMesh navMesh = null;

	// Use this for initialization
	void Start()
	{
		container = new GameObject("NavigationMesh - Wireframe");
		navMesh = this.GetComponent<NavigationMesh>();

		if (navMesh != null)
		{
			// register navMeshchangeListener
			navMesh.navigationMeshChangeHandler += OnNavigationMeshChange;
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void OnNavigationMeshChange()
	{
		// calculate new visualization

		GameObject.Destroy(container);

		container = new GameObject("NavigationMesh - Wireframe");
		/*
		for (int i = 0; i < navMesh.Count; i++)
		{
			GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			o.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
			o.transform.position = navMesh.Get(i).CENTER;
			o.transform.SetParent(transform);
			o.GetComponent<MeshRenderer>().material = lineColor;
		}
		/**/
		for (int i = 0; i < navMesh.navmeshNodes.Count; i++)
		{
			GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			o.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
			o.transform.position = navMesh.navmeshNodes[i].position;
			o.transform.SetParent(transform);
			o.GetComponent<MeshRenderer>().material = lineColor;
		}

		for (int index = 0; index < navMesh.Count; index++)
		{
			DrawSegment(navMesh.Get(index));
		}

		DrawNavGraph();
	}

	private void DrawSegment(Segment segment)
	{
		for (int i = 0; i < segment.Count - 1; i++)
		{
			Debug.DrawLine(segment.vertices[i], segment.vertices[i + 1]);
			DrawLine(segment.vertices[i], segment.vertices[i + 1], Color.red);
		}

		Debug.DrawLine(segment.vertices[segment.Count - 1], segment.vertices[0]);
		DrawLine(segment.vertices[segment.Count - 1], segment.vertices[0], Color.red);
	}

	private void DrawLine(Vector3 start, Vector3 end, Color color, int index = -1)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.SetColors(color, color);
		lr.SetWidth(0.01f, 0.01f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		lr.transform.SetParent(container.transform);
		lr.name = "line" + (index > -1 ? index.ToString() : " ");
	}

	private void DrawNavGraph()
	{
		/*
		List<Segment> closedList = new List<Segment>();

		for (int i = 0; i < navMesh.Count; i++)
		{ 
			for(int j = 0; j < navMesh.Get(i).NeighbourCount; j++)
			{
				if (!closedList.Contains(navMesh.Get(i).GetNeighbour(j)))
				{
					DrawLine(navMesh.Get(i).CENTER, navMesh.Get(i).GetNeighbour(j).CENTER, Color.black);
					if (j != 0) { closedList.Add(navMesh.Get(i).GetNeighbour(j)); }
				}
			}
		}
		/**/

		List<Connection> closedList = new List<Connection>();

		for (int i = 0; i < navMesh.navmeshNodes.Count; i++)
		{
			for (int j = 0; j < navMesh.navmeshNodes[i].neighbours.Count; j++)
			{
				if (!closedList.Contains(navMesh.navmeshNodes[i].neighbours[j]))
				{
					DrawLine(navMesh.navmeshNodes[i].position, navMesh.navmeshNodes[i].neighbours[j].position, Color.black);
					if (j != 0) { closedList.Add(navMesh.navmeshNodes[i].neighbours[j]); }
				}
			}
		}
	}

	void OnDestroy()
	{
		navMesh.navigationMeshChangeHandler -= OnNavigationMeshChange;
	}
}
