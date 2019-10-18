using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour {

	Queue<Vector3> waypoints = new Queue<Vector3>();

	int currentWaypoint;
	Vector3 targetWaypoint;

	Vector3 currentPosition;
	float offset;

	public float speed = 4f;
	public float rotationSpeed = 4f;

	CharacterController cc;
	float distanceToStop = 0.1f;

	Animator anim;


	public event Action reachedWaypoint;

	void Start () {
		cc = GetComponent<CharacterController> ();
		currentPosition = roundPosition (transform.position);
		anim = GetComponent<Animator> ();
	}

	Vector3 moveVector;

	void Update(){

		transform.eulerAngles = new Vector3(0,transform.eulerAngles.y, 0);

		//REeset the MoveVector
		moveVector = Vector3.zero;

		//Check if cjharacter is grounded
		if (cc.isGrounded == false) {
			//Add our gravity Vecotr
			moveVector += Physics.gravity;
		} else {
			if (offset == 0) {
				offset = currentPosition.y - transform.position.y;
			}
		}

		//Apply our move Vector , remeber to multiply by Time.delta
		cc.Move(moveVector * Time.deltaTime);
	}

	// Update is called once per frame
//	void Update () {
//
//		// check if we have somewere to walk
//		if (waypoints.Count != 0) {
//			anim.SetBool ("isRunning", true);
//			Vector3 walkPosition = currentPosition;
//			walkPosition.y = transform.position.y;
//			walkTo (walkPosition);
//			Debug.Log (walkPosition);
//			if (Vector3.Distance (transform.position, walkPosition) < distanceToStop) {
//				if (reachedWaypoint != null) {
//					reachedWaypoint();
//				}
//				if (currentPosition == waypoints.Peek ()) {
//					waypoints.Dequeue ();
//				}
//				if (waypoints.Count != 0) {
//					currentPosition = waypoints.Peek ();
//				}
//				if (waypoints.Count == 0) {
//					anim.SetBool ("isRunning", false);
//				}
//			}
//		}
//
//	}

	Vector3 destination = Vector3.zero;

	void FixedUpdate ()
	{
		if (waypoints.Count != 0) {
			enableCharacterController ();
			anim.SetBool ("isRunning", true);
			destination = waypoints.Peek ();
			destination.y = destination.y - offset;
			MoveTowardsTarget (destination);
			RotateTowardsTarget (destination);
			if (Vector3.Distance (transform.position, destination) < distanceToStop) {
				if (reachedWaypoint != null) {
					reachedWaypoint ();
				}
				currentPosition = waypoints.Dequeue ();
				if (waypoints.Count == 0) {
					anim.SetBool ("isRunning", false);
					disableCharchterController ();
				}
			}
		}

		if (transform.position.y < -10f) {
			GameManager.GetInstance ().restartLevel ();
		}

	}

	void MoveTowardsTarget(Vector3 target) {
		CharacterController cc = GetComponent<CharacterController>();
		Vector3 offset = target - transform.position;
		//Get the difference.
		if(offset.magnitude > .1f) {
			//If we're further away than .1 unit, move towards the target.
			//The minimum allowable tolerance varies with the speed of the object and the framerate. 
			// 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
			offset = offset.normalized * speed;
			//normalize it and account for movement speed.
			cc.Move(offset * Time.deltaTime);
			//actually move the character.
		}
	}

	void RotateTowardsTarget(Vector3 target){
		Vector3 diff = destination - transform.position;
		transform.forward = Vector3.RotateTowards(transform.forward,diff, rotationSpeed*Time.deltaTime, 0.0f);
	}

	void walkTo(Vector3 destination){
		// rotate towards the target
		Vector3 diff = destination - transform.position;
		diff.y = 0f;
		transform.forward = Vector3.RotateTowards(transform.forward,diff, speed*2*Time.deltaTime, 0.0f);

		Vector3 direction = (diff).normalized;
		Vector3 movePos = transform.position + direction * speed * Time.deltaTime;
		Debug.Log ("MovePosition: "+movePos);
		Debug.Log ("MoveDirection: " + direction);
		Debug.Log("PlayerPos: "+transform.localPosition);
		cc.Move(movePos);
	}

	public void setNewWaypoints(Vector3[] newWaypoints){
		waypoints = new Queue<Vector3> (newWaypoints);
	}

	public Vector3 getPosition(){
		return currentPosition;
	}
	public void setPosition(Vector3 position){
		currentPosition = position;
	}

	public bool isPositionInWalkingPath(Vector3 position){
		return waypoints.Contains (position) || position == currentPosition;
	}

	public void roundPosition (){
		transform.position = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y) - 0.48f, Mathf.Round (transform.position.z));
	}
	public Vector3 roundPosition(Vector3 pos){
		return new Vector3 (Mathf.Round (pos.x), Mathf.Round (pos.y), Mathf.Round (pos.z));
	}

	void OnTriggerEnter(Collider other){

		if(other.gameObject.tag == "SlidingElement"){
			SimpleMovableElement element = other.gameObject.GetComponentInParent<SimpleMovableElement>();
			transform.parent = element.transform;
			element.SetPlayerOnTop (true);
		}

		if(other.gameObject.tag == "EndScreen"){
			int clicks = GameManager.GetInstance ().GetTotalClicks ();
			UIManager.GetInstance ().showEndPanel (clicks);
		}

		if (other.gameObject.tag == "Gate") {
			Gate gate = other.gameObject.GetComponentInParent<Gate>();
			if (gate != null) {
				gate.TryToOpen ();
			}
		}
			
		if (other.gameObject.tag == "LevelEntry") {
			LevelEntryDoor door = other.gameObject.GetComponentInParent<LevelEntryDoor>();
			if (door != null) {
				door.startLevel ();
			}
		}

		if (other.gameObject.tag == "LevelDoor") {
			LevelDoor door = other.gameObject.GetComponentInParent<LevelDoor>();
			if (door != null) {
				door.startLevel ();
			}
		}

		if (other.gameObject.tag == "Key") {
			Key key = other.gameObject.GetComponent<Key> ();
			if (key != null) {
				key.setFound ();
				key.GetComponent<PathElement> ().SetActive (false);
			}
		}
	}

	void OnTriggerStay(Collider other){
		if(other.gameObject.tag == "SlidingElement"){
			SimpleMovableElement element = other.gameObject.GetComponentInParent<SimpleMovableElement>();
			transform.parent = element.transform;
		}
	}
		
	void OnTriggerExit(Collider other){
		if(other.gameObject.tag == "SlidingElement"){
			SimpleMovableElement element = other.gameObject.GetComponentInParent<SimpleMovableElement>();
			element.SetPlayerOnTop (false);
			transform.parent = element.getLevel().transform;
		}
			
	}
	public void disableCharchterController(){
		cc.enabled = false;
	}
	public void enableCharacterController(){
		cc.enabled = true;
	}

	void setDebugWaypoints(){
		waypoints.Enqueue (new Vector3(-1,4,0));
		waypoints.Enqueue (new Vector3(-1,4,-1));
		waypoints.Enqueue (new Vector3(-1,4,-2));
		waypoints.Enqueue (new Vector3(-1,4,-3));
		waypoints.Enqueue (new Vector3(0,4,-3));
		waypoints.Enqueue (new Vector3(1,4,-3));
		waypoints.Enqueue (new Vector3(1,4,-2));
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		foreach (Vector3 point in waypoints) {
			Gizmos.DrawSphere(point, 0.2f);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (destination, 0.3f);
	}

}
