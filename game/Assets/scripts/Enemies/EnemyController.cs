using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	
	public float strength = 100.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider obj){
		if(obj.tag == "Player"){
			var controller = obj.gameObject.GetComponent("CavemanController");
			controller.SendMessage("Damage", strength);	
		}
	}
}
