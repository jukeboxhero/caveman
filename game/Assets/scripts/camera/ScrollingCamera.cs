using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollingCamera : MonoBehaviour {
	
	public Transform target;
	LevelBoundaries levelAttributes;
	Rect levelBounds;
	// How far back should the camera be from the target?
	public float distance = 10.0f;
	// How strict should the camera follow the target?  Lower values make the camera more lazy.
	public float springiness = 4.0f;
	
	void Alive(){
		
	}
	
	void Start () {
		target = CavemanController.current_player;
		levelAttributes = LevelBoundaries.GetInstance ();
		levelBounds = levelAttributes.bounds;
	
	}
	
	void Update(){
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		var goalPosition = GetGoalPosition ();
		transform.position = Vector3.Lerp (transform.position, goalPosition, Time.deltaTime * springiness);	
	}
	
	public void SetTarget (Transform newTarget) {
		target = newTarget;
		
		if (target) {
			var targetRigidbody = target.gameObject.GetComponent ("Rigidbody") as Rigidbody;
			if (targetRigidbody) {
				var savedInterpolationSetting = targetRigidbody.interpolation;
				targetRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			}
		}
	}
	
	public void Stop(){
		target=null;	
	}
	
	Vector3 GetGoalPosition() {
		if(!target){
			return transform.position;	
		}
		
		// Our camera script can take attributes from the target.  If there are no attributes attached, we have
		// the following defaults.
		
		// How high in world space should the camera look above the target?
		float heightOffset = 5.0f;
		// How much should we zoom the camera based on this target?
		float distanceModifier = 1.0f;
		// By default, we won't account for any target velocity in our calculations;
		float velocityLookAhead = 1.0f;
		Vector2 maxLookAhead = new Vector2 (3.0f, 0.0f);
		
		// First do a rough goalPosition that simply follows the target at a certain relative height and distance.
		Vector3 goalPosition = target.position + new Vector3 (0, heightOffset, -distance * distanceModifier);
		
		// Next, we refine our goalPosition by taking into account our target's current velocity.
		// This will make the camera slightly look ahead to wherever the character is going.
		
		// First assume there is no velocity.
		// This is so if the camera's target is not a Rigidbody, it won't do any look-ahead calculations because everything will be zero.
		Vector3 targetVelocity = Vector3.zero;
		
		// If we find a Rigidbody on the target, that means we can access a velocity!
		Rigidbody targetRigidbody = target.gameObject.GetComponent ("Rigidbody") as Rigidbody;
		if (targetRigidbody)
			targetVelocity = targetRigidbody.velocity;
		
		// If we find a PlatformerController on the target, we can access a velocity from that!
		CavemanController targetCavemanController = target.gameObject.GetComponent ("CavemanController") as CavemanController;
		if (targetCavemanController)
			targetVelocity = targetCavemanController.GetVelocity ();
		
		// If you've had a physics class, you may recall an equation similar to: position = velocity * time;
		// Here we estimate what the target's position will be in velocityLookAhead seconds.
		var lookAhead = targetVelocity * velocityLookAhead;
		
		// We clamp the lookAhead vector to some sane values so that the target doesn't go offscreen.
		// This calculation could be more advanced (lengthy), taking into account the target's viewport position,
		// but this works pretty well in practice.
		lookAhead.x = Mathf.Clamp (lookAhead.x, -maxLookAhead.x, maxLookAhead.x);
		lookAhead.y = Mathf.Clamp (lookAhead.y, -maxLookAhead.y, maxLookAhead.y);
		// We never want to take z velocity into account as this is 2D.  Just make sure it's zero.
		lookAhead.z = 0.0f;
			
		goalPosition += lookAhead;
		
		Vector3 clampOffset = Vector3.zero;
		
		var cameraPositionSave = transform.position;
		transform.position = goalPosition;
		
		var targetViewportPosition = camera.WorldToViewportPoint (target.position);
		var upperRightCameraInWorld = camera.ViewportToWorldPoint (new Vector3 (1.0f, 1.0f, targetViewportPosition.z));
		transform.position = goalPosition;
		var lowerLeftCameraInWorld = camera.ViewportToWorldPoint (new Vector3 (0.0f, 0.0f, targetViewportPosition.z));
		
		
		clampOffset.x = Mathf.Max ((levelBounds.xMin - lowerLeftCameraInWorld.x), 0.0f);
		clampOffset.y = Mathf.Max ((levelBounds.yMin - lowerLeftCameraInWorld.y), 0.0f);
		
		// Now we apply our clamping to our goalPosition.  Now our camera won't go past the right and top boundaries of the level!
		goalPosition += clampOffset;
		
		transform.position = cameraPositionSave;
		
		return goalPosition;
	}
}
