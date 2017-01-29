using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathVisualizer : MonoBehaviour {

	public List<Vector3> path;
	public GameObject container;
	public Material material;
	public string pathname;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void VisualizePath(Vector3[] path)
	{

	}

	public List<Vector3> Path
	{
		get
		{
			return path;
		}
		set
		{
			if (container != null)
			{
				Destroy(container);
			}

			path = value;

			container = new GameObject(pathname + "Container");
			LineRenderer renderer = container.AddComponent<LineRenderer>();
			renderer.material = material;
			renderer.SetWidth(0.01f, 0.02f);
			renderer.SetVertexCount(path.Count);
			renderer.SetPositions(path.ToArray());
			renderer.useWorldSpace = false;

			container.transform.position = new Vector3(container.transform.position.x, container.transform.position.y + 0.1f, container.transform.position.z);
		}
	}
}
