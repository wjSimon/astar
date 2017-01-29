using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeGrid : MonoBehaviour {

	Node[,] nodegrid;
	public Vector2 gridSize;
	public float nodeRadius;
	float nodeDiameter;
	int width; 
	int height;
	public LayerMask sphereCheckMask;

	// Use this for initialization
	void Start () {
		nodeDiameter = nodeRadius * 2;
		width = Mathf.RoundToInt(gridSize.x / nodeDiameter);
		height = Mathf.RoundToInt(gridSize.y / nodeDiameter);

		//Debug.Log (width + " " + height);
		BuildGrid ();
	}

	void BuildGrid ()
	{
		nodegrid = new Node[width, height];
		Vector3 bottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y/2; //GRID HAS TO BE CENTERED ON GROUND FOR THIS TO WORK

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Vector3 center = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool col = Physics.CheckSphere(center, nodeRadius, sphereCheckMask);
				//nodegrid [x, y] = new Node (center, !col, x, y);
				nodegrid [x, y] = new Node (center, !col);
			}
		}
	}

	public Node PosToNode(Vector3 pos)
	{
		float posX = (pos.x + gridSize.x / 2) / gridSize.x;
		float posY = (pos.z + gridSize.y / 2) / gridSize.y;

		int x = Mathf.RoundToInt ((width-1f) * posX);
		int y = Mathf.RoundToInt ((height-1f) * posY);

		if ((x < 0 || x >= nodegrid.GetLength (0)) || (y < 0 || y >= nodegrid.GetLength(1))) {
			throw new System.OverflowException ("Node position outside of map");
		}

		return nodegrid[x, y];
	}

	//same as in the console thingy just that i found someone who actually knows what he's doing so its a nice loop now instead of   for(int i = 0; i < 8; i++) copy(line[i]); paste(line[i]); ReplaceLineOperators(line);
	public List<Node> FindNeighbours(Node current)
	{
		List<Node> neighbours = new List<Node> ();
		Vector2 nodeIndex = GetIndexOfNode (current);

		//Debug.Log("vector2: " + nodeIndex.x + " " + nodeIndex.y + "storage: " + current.x + " " + current.y); <-- check for the GetIndexOfNode function artifact

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) {
					continue;
				}

				int xPos = (int)nodeIndex.x + x;
				int yPos = (int)nodeIndex.y + y;

				if ((xPos >= 0 && xPos < width) && (yPos >= 0 && yPos < height)) {
					neighbours.Add (nodegrid [xPos, yPos]);
				}
			}
		}			

		return neighbours; //OMG ITS WORKING WHY DIDNT IT WITH THE MESH WTF?
	}

	public Vector2 GetIndexOfNode(Node node)
	{
		//Found some dude doing this with Tuples, stole it cus like it wat you gon' do
		int w = nodegrid.GetLength (0);
		int h = nodegrid.GetLength (1);

		for (int i = 0; i < w; i++) 
		{
			for(int j = 0; j < h; j++)
			{
				if (nodegrid [i, j].Equals (node))
					return new Vector2 (i, j);
			}
		}

		// i dunno man i like these instead of null returns now
		throw new System.Exception ("Node can not be found");
	}

	public List<Node> path = new List<Node>(); //so we can draw it
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));

		if (nodegrid != null) {
			//Node _start = PosToNode (start.position);
			foreach (Node n in nodegrid) {
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				/*
				if (n == PosToNode) {
					Gizmos.color = Color.green;
				}
				/**/

				if (path != null) {
					if (path.Contains (n)) {
						Gizmos.color = Color.yellow;
					}
				}
				/**/

				Gizmos.DrawSphere (n.position, nodeRadius);
			}
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
