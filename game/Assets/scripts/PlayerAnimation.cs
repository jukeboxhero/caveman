using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {
	
	float landBounceTime = 6.0f;
	private AnimationState lastJump;
	
	// Use this for initialization
	void Start () {
		// By default loop all animations
		animation.wrapMode = WrapMode.Loop;
	
		// The jump animation is clamped and overrides all others
		var jump = animation["jump"];
		jump.layer = 1;
		jump.enabled = false;
		jump.wrapMode = WrapMode.Clamp;
	}
	
	// Update is called once per frame
	void Update () {
		CavemanController controller = GetComponent("CavemanController") as CavemanController;
		float currentSpeed = controller.GetSpeed();
	
		// Switch between idle and walk
		if (currentSpeed > 0.1f)
			if (Input.GetButton ("Fire2") && controller.canControl){
				animation.CrossFade("run");
			}else{
				animation.CrossFade("walk");
			}
		else
			animation.CrossFade("idle");
	
		// When we jump we want the character start animate the landing bounce, exactly when he lands. So we do this:
		// - pause animation (setting speed to 0) when we are jumping and the animation time is at the landBounceTime
		// - When we land we set the speed back to 1
		if (controller.IsJumping())
		{
			if (lastJump.time > landBounceTime)
				lastJump.speed = 0;
		}
	}
	void DidJump () {
		// We want to play the jump animation queued,
		// so that we can play the jump animation multiple times, overlaying each other
		// We dont want to rewind the same animation to avoid sudden jerks!
		lastJump = animation.CrossFadeQueued("jump", 0.3f, QueueMode.PlayNow);
	}
	
	void DidLand () {
		lastJump.speed = 1;
	}
}
