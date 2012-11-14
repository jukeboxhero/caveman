using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollingCamera : MonoBehaviour {
	
	Transform target;
	
	
	void Alive(){
		
	}
	
	void Start () {
		target = GameObject.Find("Player").transform;
	
	}
	
	void Update(){
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		var goalPosition = GetGoalPosition ();
		transform.LookAt(target);
		transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
		
	}
	
	void SetTarget (Transform newTarget) {
		
	}
	
	Vector3 GetGoalPosition() {
		return Vector3.zero;
	}
}
