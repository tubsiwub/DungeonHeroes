using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour 
{
	void Update () 
	{
		transform.position = (Vector3)AstarPath.active.GetNearest (transform.position).node.position;
	}
}
