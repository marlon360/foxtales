using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PathElement : MonoBehaviour {


	Level level;

	public void setLevel(Level level){
		this.level = level;
	}

	public Level getLevel(){
		return level;
	}


	public void roundPosition(){
		transform.position = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y), Mathf.Round (transform.position.z));
	}
	public void roundRotation(){
		Vector3 newRot;
		newRot.x = Mathf.Round (((gameObject.transform.eulerAngles.x) / 90f)) * 90f;
		newRot.y = Mathf.Round (((gameObject.transform.eulerAngles.y) / 90f)) * 90f;
		newRot.z = Mathf.Round (((gameObject.transform.eulerAngles.z) / 90f)) * 90f;

		transform.eulerAngles = newRot;
	}

	public abstract void add ();
	public abstract void remove ();


	public void SetActive (bool active){
		if (!active) {
			remove ();
		} else {
			add ();
		}
		gameObject.SetActive (active);
	}


	public Vector3 getPosition(){
		return transform.position;
	}
		
		
}
