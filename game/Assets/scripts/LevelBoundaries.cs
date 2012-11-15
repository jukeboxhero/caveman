// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class LevelBoundaries : MonoBehaviour {
// Size of the level
public Rect bounds;
public float fallOutBuffer= 5.0f;
public float colliderThickness= 10.0f;

// Sea Green For the Win!
private Color sceneViewDisplayColor= new Color (0.20f, 0.74f, 0.27f, 0.50f);

static private LevelBoundaries instance;

static LevelBoundaries GetInstance (){
	if (!instance) {
		instance = FindObjectOfType(typeof(LevelBoundaries)) as LevelBoundaries;
		if (!instance)
			Debug.LogError("There needs to be one active LevelAttributes script on a GameObject in your scene.");
	}
	return instance;
}

void  OnDisable (){
	instance = null;
}

void  OnDrawGizmos (){
	Gizmos.color = sceneViewDisplayColor;
	Vector3 lowerLeft= new Vector3 (bounds.xMin, bounds.yMax, 0);
	Vector3 upperLeft= new Vector3 (bounds.xMin, bounds.yMin, 0);
	Vector3 lowerRight= new Vector3 (bounds.xMax, bounds.yMax, 0);
	Vector3 upperRight= new Vector3 (bounds.xMax, bounds.yMin, 0);
	
	Gizmos.DrawLine (lowerLeft, upperLeft);
	Gizmos.DrawLine (upperLeft, upperRight);
	Gizmos.DrawLine (upperRight, lowerRight);
	Gizmos.DrawLine (lowerRight, lowerLeft);
}

void  Start (){
	var createdBoundaries = new GameObject ("Created Boundaries");
	createdBoundaries.transform.parent = transform;
	
	var leftBoundary = new GameObject ("Left Boundary");
	leftBoundary.transform.parent = createdBoundaries.transform;
	var boxCollider = leftBoundary.AddComponent ("BoxCollider") as BoxCollider;
	boxCollider.size = new Vector3 (colliderThickness, bounds.height + colliderThickness * 2.0f + fallOutBuffer, colliderThickness);
	boxCollider.center = new Vector3 (bounds.xMin - colliderThickness * 0.5f, bounds.y + bounds.height * 0.5f - fallOutBuffer * 0.5f, 0.0f);
	
	var rightBoundary = new GameObject ("Right Boundary");
	rightBoundary.transform.parent = createdBoundaries.transform;
	boxCollider = rightBoundary.AddComponent ("BoxCollider") as BoxCollider;
	boxCollider.size = new Vector3 (colliderThickness, bounds.height + colliderThickness * 2.0f + fallOutBuffer, colliderThickness);
	boxCollider.center = new Vector3 (bounds.xMax + colliderThickness * 0.5f, bounds.y + bounds.height * 0.5f - fallOutBuffer * 0.5f, 0.0f);
	
	var topBoundary = new GameObject ("Top Boundary");
	topBoundary.transform.parent = createdBoundaries.transform;
	boxCollider = topBoundary.AddComponent ("BoxCollider") as BoxCollider;
	boxCollider.size = new Vector3 (bounds.width + colliderThickness * 2.0f, colliderThickness, colliderThickness);
	boxCollider.center = new Vector3 (bounds.x + bounds.width * 0.5f, bounds.yMax + colliderThickness * 0.5f, 0.0f);
	
	var bottomBoundary = new GameObject ("Bottom Boundary (Including Fallout Buffer)");
	bottomBoundary.transform.parent = createdBoundaries.transform;
	boxCollider = bottomBoundary.AddComponent ("BoxCollider") as BoxCollider;
	boxCollider.size = new Vector3 (bounds.width + colliderThickness * 2.0f, colliderThickness, colliderThickness);
	boxCollider.center = new Vector3 (bounds.x + bounds.width * 0.5f, bounds.yMin - colliderThickness * 0.5f - fallOutBuffer, 0.0f);
}
}