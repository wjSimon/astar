using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour
{

    List<PathNode> nodes;

    AmericaFuckYeah plib = new AmericaFuckYeah();
    public List<PathNode> openlist = new List<PathNode>();
    public List<PathNode> closedlist = new List<PathNode>();

    public PathNode current;
    // Use this for initialization
    void Start()
    {
        plib = GameObject.Find("navmesh").GetComponent<AmericaFuckYeah>();
        Debug.Log("Starting PathFinder...");

        CreateNodes(plib.centroids.ToArray());
        openlist.AddRange(nodes);
        //Debug.Log(nodes.Count);

        GetPath(nodes[0], nodes[8]);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public PathNode CalculatePath(PathNode start, PathNode goal)
    {
        PathNode targetNode = null;
        PathNode currentNode = null;
        List<PathNode> toBeChecked = new List<PathNode>();

        Debug.Log("targetnode: " + targetNode);
        //openlist.Clear();
		Debug.Log("openlist: " + openlist.Count);

        toBeChecked.Clear();
        toBeChecked.Add(start);
        openlist.Remove(start);

        while (toBeChecked.Count > 0)
        {
            currentNode = toBeChecked[0];

            if (currentNode == goal)
            {
                targetNode = currentNode;
                Debug.Log("targetnode: " + targetNode);
                break;
            }
            else
            {
                toBeChecked.RemoveAt(0);
                FindSuccessors(currentNode);
                Debug.Log(currentNode.succ.Count);

                foreach (PathNode n in currentNode.succ)
                {
                    n.parent = currentNode;
                    toBeChecked.Add(n);
                    openlist.Remove(n);
                }
            }
        }

        Debug.Log("targetnode: " + targetNode);
        return targetNode;
    }
    public List<Vector3> GetPath(PathNode start, PathNode goal)
    {
        List<Vector3> path = new List<Vector3>();
        PathNode currentNode = CalculatePath(start, goal);

        while (currentNode != null)
        {
            path.Insert(0, currentNode.position);
            currentNode = currentNode.parent;
        }

        Debug.Log("path: " + path.Count);
        foreach(Vector3 v in path) { Debug.Log(v); }
        return path;
    }

    public void FindSuccessors(PathNode node)
    {

        Segment current = plib.GetSegmentByCenter(node.position);
        //Debug.Log(current);

        for (int i = 0; i < current.NeighbourCount; i++)
        {
			if (node.parent != null) {
				if ((!(current.GetNeighbour (i) != plib.GetSegmentByCenter(node.parent.position)))) {
					node.succ.Add (FindNodeInOpenList(current.GetNeighbour (i).CENTER));
					Debug.Log ("??");
				}
			} else 
			{
				//Debug.Log (current.NeighbourCount);
				//throw new System.NotSupportedException("fuck you");
				node.succ.Add(FindNodeInOpenList(current.GetNeighbour(i).CENTER));
			}
        }
    }

    public void CreateNodes(Vector3[] points)
    {
        nodes = new List<PathNode>();

        foreach (Vector3 v in points)
        {
            nodes.Add(new PathNode(v));
        }
    }

    public PathNode FindNode(Vector3 pos)
    {
        foreach (PathNode n in nodes)
        {
			//Debug.Log(Vector3.Distance(n.position, pos));
			if (Vector3.Distance(n.position, pos) <= 0.05f && openlist.Contains(n)) { return n; }
        }

        throw new System.Exception("Node not found");
    }

	public PathNode FindNodeInOpenList(Vector3 pos)
	{
		Debug.Log("FindNodeInOpenList()");
		foreach (PathNode n in openlist)
		{
			float dist = Vector3.Distance (n.position, pos);
			Debug.Log(dist);
			Debug.Log(dist <= 0.05f);
			if (dist <= 0.05f) { return n; }
		}

		return null;
	}
}
