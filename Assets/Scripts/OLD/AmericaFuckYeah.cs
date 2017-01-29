using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmericaFuckYeah : MonoBehaviour
{
    //Use this for initialization
    Mesh navmesh;
    public List<Vector3> centroids = new List<Vector3>();
    public List<Segment> triangles = new List<Segment>();
    public Material red;
    public Material marker;
	public GameObject container;

    void Start()
    {
        navmesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = navmesh.vertices;
        int[] tris = navmesh.triangles;

        //Debug.Log(vertices.Length + " " + tris.Length);

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 a = vertices[tris[i]];
            Vector3 b = vertices[tris[i + 1]];
            Vector3 c = vertices[tris[i + 2]];

            Vector3 center = (a + b + c) / 3;
            centroids.Add(center);
            triangles.Add(new Segment(a, b, c));
        }

        for (int i = 0; i < centroids.Count; i++)
        {
            GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            o.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            o.transform.position = centroids[i];
            o.transform.SetParent(transform.GetChild(0));
            o.GetComponent<MeshRenderer>().material = red;
            if(i == 0 && i == 8)
            {
                o.GetComponent<MeshRenderer>().material = marker;
            }
        }

        for(int i = 0; i < triangles.Count; i++) {
            for(int j = 0; j < triangles.Count; j++)
            {
                if(i == j) { continue; }

                if(triangles[i].IsAdjacent(triangles[j]))
                {
                    triangles[i].AddNeighbour(triangles[j]);
                }
            }

            //Debug.Log(triangles[i].NeighbourCount);
        }

        //Debug.Log(centroids.Count);
        //Debug.Log(triangles.Count);
    }

    // Update is called once per frame
    void Update()
    {
		/*
        for (int i = 0; i < centroids.Count; i++)
        {
            if (i < centroids.Count - 1)
            { Debug.DrawLine(centroids[i], centroids[i + 1]); }
            else
            { Debug.DrawLine(centroids[i], centroids[0]); }
        }
        /**/

		for (int i = 0; i < triangles.Count; i++) {
			//DrawTriangle (triangles [i]);
		}
    }

	void DrawTriangle(Segment triangle)
	{
		for (int i = 0; i < triangle.Count-1; i++) {
			Debug.DrawLine (triangle.vertices [i], triangle.vertices [i + 1]);
			DrawLine (triangle.vertices [i], triangle.vertices [i + 1], Color.red);
		}

		Debug.DrawLine(triangle.vertices[triangle.Count-1], triangle.vertices[0]);
		DrawLine(triangle.vertices[triangle.Count-1], triangle.vertices[0], Color.red);

	}

	void DrawLine(Vector3 start, Vector3 end, Color color, int index = -1)
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
		lr.transform.SetParent (container.transform);
		lr.name = "line" + (index > -1 ? index.ToString() : " ");
	}

    public Segment GetSegmentByCenter(Vector3 center)
    {
        for(int i = 0; i < triangles.Count; i++)
        {
            if(triangles[i].CompareWithVector(center))
            {
                return triangles[i];
            }
        }

        return null;
    }
}
