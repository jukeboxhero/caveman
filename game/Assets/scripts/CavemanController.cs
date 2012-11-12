using UnityEngine;
using System.Collections;

public class CavemanController : MonoBehaviour {
	
	CavemanControllerMovement movement = new CavemanControllerMovement();
	CharacterController controller;
		
	void Awake(){
		controller = gameObject.GetComponent ("CharacterController") as CharacterController;
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// left and right motion & control
		var h = Input.GetAxisRaw ("Horizontal");
		Debug.Log(h);
		
		// gravity
		ApplyGravity();
		
		// jumping
		
		// Calculate actual motion
		var currentMovementOffset = movement.direction * movement.speed + new Vector3 (0, movement.verticalSpeed, 0) + movement.inAirVelocity;
		
		// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
		currentMovementOffset *= Time.deltaTime;
		
	   	// Move our character!
		controller.Move (currentMovementOffset);
		
		
		
		//attacking
	}
	
	
	void ApplyGravity(){
		movement.verticalSpeed -= movement.gravity * Time.deltaTime;	
	}
}

public class CavemanControllerMovement {
	public float gravity = 60.0f;
	public float verticalSpeed = 0.0f;
	public Vector3 direction = Vector3.zero;
	public float speed = 0.0f;
	public Vector3 inAirVelocity = Vector3.zero;
}