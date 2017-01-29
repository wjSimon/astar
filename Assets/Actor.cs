using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour
{
	List<Vector3> path;
	public float speed;
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (path != null && path.Count > 0)
		{
			transform.position = Vector3.MoveTowards(transform.position, path[0], 0.1f * Time.deltaTime);

			if (Vector3.Distance(transform.position, path[0]) <= 0.01f)
			{
				path.RemoveAt(0);
			}
		}
	}

	public List<Vector3> Path
	{
		get
		{
			return path;
		}
		set
		{
			path = value;
		}
	}
}
