using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavigationMesh))]
public class UnitController : MonoBehaviour {
	public GameObject target = null;
	private NavigationMesh navMesh = null;
	private PathVisualizer visualizer = null;
	// Use this for initialization
	void Start () {
		if (target == null) { throw new System.Exception("YOU NEED A TARGET YOU FUCKER!");  }

		navMesh = this.GetComponent<NavigationMesh>();
		visualizer = GetComponent<PathVisualizer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			
			RaycastHit info = new RaycastHit();

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info))
			{
				if (navMesh.Contains(target.transform.position) && navMesh.Contains(info.point)){
					List<Vector3> path = navMesh.CalculatePath(target.transform.position, info.point);
					visualizer.Path = path;
					target.GetComponent<Actor>().Path = path;
					//Debug.LogWarning("Historiosaurus Rex");
				}
			}
		}
	}
}
