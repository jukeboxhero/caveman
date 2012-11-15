using UnityEngine;
using System.Collections;

public class CavemanController : MonoBehaviour {
	
	CavemanControllerMovement movement = new CavemanControllerMovement();
	CavemanControllerJump jump = new CavemanControllerJump();
	CharacterController controller;
	// Does this script currently respond to Input?
	public bool canControl = true;
	
	public Transform spawnPoint;
		
	void Awake(){
		controller = gameObject.GetComponent ("CharacterController") as CharacterController;
	}
	
	void Spawn () {
		// reset the character's speed
		movement.verticalSpeed = 0.0f;
		movement.speed = 0.0f;
		
		// reset the character's position to the spawnPoint
		transform.position = spawnPoint.position;
		
	}
	
	void OnDeath () {
		var camera = GameObject.Find("Main Camera");
		var cameraScript = camera.GetComponent("ScrollingCamera") as ScrollingCamera;
		cameraScript.Stop();
		//Spawn ();
	}
	
	// Use this for initialization
	void Start () {

	}
	
	void FixedUpdate () {
		// Make sure we are absolutely always in the 2D plane.
		Vector3 position = transform.position;
		position.z = 0;
		transform.position = position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Jump") && canControl) {
			jump.lastButtonTime = Time.time;
		}
		
		MotionControl();
		
		// gravity
		ApplyGravity();
		
		// jumping
		AddJumpControl();
		
		// Save lastPosition for velocity calculation.
		Vector3 lastPosition = transform.position;
		
		// Calculate actual motion
		var currentMovementOffset = movement.direction * movement.speed + new Vector3 (0, movement.verticalSpeed, 0) + movement.inAirVelocity;
		
		
		// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this
		currentMovementOffset *= Time.deltaTime;
		
	   	// Move our character!
		movement.collisionFlags = controller.Move (currentMovementOffset);
		
		// Set rotation to the move direction	
		if (movement.direction.sqrMagnitude > 0.01)
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (movement.direction), Time.deltaTime * movement.rotationSmoothing);
		
		// Calculate the velocity based on the current and previous position.  
		// This means our velocity will only be the amount the character actually moved as a result of collisions.
		movement.velocity = (transform.position - lastPosition) / Time.deltaTime;
		
			// We are in jump mode but just became grounded
		if (controller.isGrounded) {
			movement.inAirVelocity = Vector3.zero;
			if (jump.jumping) {
				jump.jumping = false;
				SendMessage ("DidLand", SendMessageOptions.DontRequireReceiver);
	
				var jumpMoveDirection = movement.direction * movement.speed + movement.inAirVelocity;
				if (jumpMoveDirection.sqrMagnitude > 0.01)
					movement.direction = jumpMoveDirection.normalized;
			}
		}	
		
		//attacking
	}
	
	void MotionControl(){
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
			//in air controls
			movement.hangTime += Time.deltaTime;
			if (movement.isMoving) {
				movement.inAirVelocity += new Vector3 (Mathf.Sign(h), 0, 0) * Time.deltaTime * movement.inAirControlAcceleration;
			}
		}	
	}
	
	void AddJumpControl(){
		// Prevent jumping too fast after each other
		if (jump.lastTime + jump.repeatTime > Time.time)
			return;
		
		if(controller.isGrounded){
			if(jump.enabled && (Time.time < jump.lastButtonTime + jump.timeout)){
				movement.verticalSpeed = CalculateJumpVerticalSpeed (jump.height);
				SendMessage ("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void ApplyGravity(){
		var jumpButton = Input.GetButton ("Jump");
		
		if (!canControl)
			jumpButton = false;
			
		// When we reach the apex of the jump we send out a message
		if (jump.jumping && !jump.reachedApex && movement.verticalSpeed <= 0.0) {
			jump.reachedApex = true;
			SendMessage ("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
		}
		
		// * When jumping up we don't apply gravity for some time when the user is holding the jump button
		//   This gives more control over jump height by pressing the button longer
		var extraPowerJump =  jump.jumping && movement.verticalSpeed > 0.0 && jumpButton && transform.position.y < jump.lastStartHeight + jump.extraHeight && !IsTouchingCeiling ();
		
		if(extraPowerJump){
			return;
		}else if(controller.isGrounded){
			movement.verticalSpeed = -movement.gravity * Time.deltaTime;
		}else{
			movement.verticalSpeed -= movement.gravity * Time.deltaTime;		
		}
		
		
		// Make sure we don't fall any faster than maxFallSpeed.  This gives our character a terminal velocity.
		movement.verticalSpeed = Mathf.Max (movement.verticalSpeed, -movement.maxFallSpeed);
	}
	
	void DidJump () {
		jump.jumping = true;
		jump.reachedApex = false;
		jump.lastTime = Time.time;
		jump.lastStartHeight = transform.position.y;
		jump.lastButtonTime = -10;
	}
	
	public void SetControllable (bool controllable) {
		canControl = controllable;
	}
	
	float CalculateJumpVerticalSpeed ( float targetJumpHeight ) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
	}
	
	
	//helpers
	public bool IsTouchingCeiling () {
		return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}
	public Vector3 GetVelocity () {
		return movement.velocity;
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
	public Vector3 velocity;
	public float hangTime = 0.0f;
	public Vector3 inAirVelocity = Vector3.zero;
	public float inAirControlAcceleration = 0.1f;
	public float rotationSmoothing = 0.1f;
	
	public bool isMoving = false;
	
	public CollisionFlags collisionFlags; 
}

public class CavemanControllerJump{
	public bool enabled = true;
	public bool jumping = false;
	public bool reachedApex = false;
	
	public float lastTime = -1.0f;
	public float repeatTime = 0.05f;
	public float lastButtonTime = -10.0f;
	public float timeout = 0.15f;
	
	public float height = 1.0f;
	public float extraHeight = 4.1f;
	public float lastStartHeight = 1.0f;
}