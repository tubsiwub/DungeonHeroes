using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour 
{
	float horizontalMovement, verticalMovement;
	public float camMoveSpeed;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		horizontalMovement = Input.GetAxisRaw ("Horizontal");
		verticalMovement = Input.GetAxisRaw ("Vertical");

		transform.position += new Vector3 (horizontalMovement, 0, verticalMovement) * camMoveSpeed * Time.deltaTime;
	}
}
