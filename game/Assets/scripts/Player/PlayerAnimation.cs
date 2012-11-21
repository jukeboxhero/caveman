using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {
	
	float landBounceTime = 6.0f;
	private AnimationState lastJump;
	public float hangTimeUntilFallingAnimation = 0.05f;
	
	// Use this for initialization
	void Start () {
		animation.AddClip(animation["attack1"].clip, "attack1UpperBody");
		animation["attack1UpperBody"].AddMixingTransform(transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1"));
		
		// By default loop all animations
		animation.wrapMode = WrapMode.Loop;
	
		// The jump animation is clamped and overrides all others
		var jump = animation["jump"];
		jump.layer = 1;
		jump.enabled = false;
		jump.wrapMode = WrapMode.Clamp;
		
		animation["attack1UpperBody"].wrapMode = WrapMode.Clamp;
		animation["attack1UpperBody"].layer = 1;
		
		var attack1 = animation["attack1"];
		attack1.layer = 1;
		attack1.wrapMode = WrapMode.Clamp;
		
		var death = animation["death"];
		death.layer = 1;
		death.wrapMode = WrapMode.ClampForever;
		
		var fall = animation["fall"];
		fall.wrapMode = WrapMode.ClampForever;
		
		animation.Stop();
		
	}
	
	// Update is called once per frame
	void Update () {
		CavemanController controller = GetComponent("CavemanController") as CavemanController;
		if (controller.GetHangTime() < hangTimeUntilFallingAnimation) {
			float currentSpeed = controller.GetSpeed();
		
			// Switch between idle and walk
			if (currentSpeed > 0.1f){
				if (Input.GetButton ("Fire2") && controller.canControl){
					animation.CrossFade("run");
				}else{
					animation.CrossFade("walk");
				}
			}else{
				animation.CrossFade("idle", 0.5f);
			}
		}else{
			animation.CrossFade("fall");
		}
	}
	void DidJump () {
		// We want to play the jump animation queued,
		// so that we can play the jump animation multiple times, overlaying each other
		// We dont want to rewind the same animation to avoid sudden jerks!
		animation.CrossFadeQueued("jump", 0.3f, QueueMode.PlayNow);
		
		//animation.PlayQueued ("jumpFall");
	}
	
	void DidLand () {
		
	}
	
	void DidAttack1(){
		// We are running so play it only on the upper body
		if (animation["walk"].weight > 0.5 || animation["run"].weight > 0.5 || animation["jump"].weight > 0.5)
			animation.CrossFadeQueued("attack1UpperBody", 0.3f, QueueMode.PlayNow);
		// We are in idle so play it on the fully body
		else
			animation.CrossFadeQueued("attack1", 0.3f, QueueMode.PlayNow);
	}
	
	void DeathAnim(){
		animation.Play ("death");
	}
}
