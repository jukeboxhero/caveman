using UnityEngine;
using System.Collections;

public class CavemanController : MonoBehaviour {
	
	CavemanControllerMovement movement = new CavemanControllerMovement();
	CharacterController controller;
	// Does this script currently respond to Input?
	public bool canControl = true;
		
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
		
		if(!canControl)
			h = 0.0f;
		
		movement.isMoving = Mathf.Abs (h) > 0.1;
		
		if (movement.isMoving)
			movement.direction = new Vector3 (h, 0, 0);
		
		if(controller.isGrounded){
			// Smooth the speed based on the current target direction
			var curSmooth = movement.speedSmoothing * Time.deltaTime;
			// Choose target speed
			var targetSpeed = Mathf.Min (Mathf.Abs(h), 1.0f);
			
			targetSpeed *= movement.walkSpeed;
			
			movement.speed = Mathf.Lerp (movement.speed, targetSpeed, curSmooth);
		}else{
			
		}
		
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
		
		// Make sure we don't fall any faster than maxFallSpeed.  This gives our character a terminal velocity.
		movement.verticalSpeed = Mathf.Max (movement.verticalSpeed, -movement.maxFallSpeed);
	}
	
	public void SetControllable (bool controllable) {
		canControl = controllable;
	}
}

public class CavemanControllerMovement {
	public float gravity = 60.0f;
	public float maxFallSpeed = 20.0f;
	
	// The speed when walking 
	public float walkSpeed = 6.0f;
	public float speedSmoothing = 5.0f;
	public float verticalSpeed = 0.0f;
	public Vector3 direction = Vector3.zero;
	public float speed = 0.0f;
	public Vector3 inAirVelocity = Vector3.zero;
	
	public bool isMoving = false;
}