using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathNode {

    public Vector3 position;
    public int g = 0;
    public int h = 0;
    public int f;

    public List<PathNode> succ = new List<PathNode>();
    public PathNode parent;

    public PathNode(Vector3 pos)
    {
        position = pos;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
