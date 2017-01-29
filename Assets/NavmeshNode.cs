using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavmeshNode {

	public Vector3 position;
	public Segment[] segments = new Segment[2];

	public NavmeshNode (Segment a, Segment b)
	{
		segments [0] = a;
		segments [1] = b;

		CalculatePosition ();
	}

	private void CalculatePosition()
	{
		if (segments.Length < 2) {
			return;
		}

		if (!segments [0].IsAdjacent (segments [1])) {
			return;
		}

		Segment a = segments [0];
		Segment b = segments [1];
		List<Vector3> sharedVerts = new List<Vector3>();
		for (int index = 0; index < a.vertices.Count; index++)
		{
			for (int jndex = 0; jndex < b.vertices.Count; jndex++)
			{
				if (a.vertices[index] == b.vertices[jndex])
				{
					sharedVerts.Add (a.vertices [index]);
				}
			}
		}

		Vector3 center = Vector3.zero;
		foreach (Vector3 v in sharedVerts)
		{
			center += v;
		}

		center /= sharedVerts.Count;

		position = center;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
