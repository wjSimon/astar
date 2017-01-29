using UnityEngine;
using System.Collections;

public class Node {

	public Vector3 position;
	public bool walkable;

	public int g;
	public int h;

	//public int x;
	//public int y;
	//checked the stupid GetIndexOfNode() with this, obsolete but left in just in case

	public Node parent;

	public Node(Vector3 _position, bool _walkable)// int _x, int _y)
	{
		position = _position;
		walkable = _walkable;

		//x = _x;
		//y = _y;
	}

	//YOU HAPPY CASPAR??
	public int f{
		get{
			return g + h;
		}
	}
}
