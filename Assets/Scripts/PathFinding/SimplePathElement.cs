using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Barrier{Walkable, NotWalkable, Stairs};


public class SimplePathElement : PathElement {

	[Header("Barriers")]
	[Header("Top")]
	public Barrier topBarrier;
	[Header("Bottom")]
	public Barrier bottomBarrier;
	[Header("Front")]
	public Barrier frontBarrier;
	[Header("Back")]
	public Barrier backBarrier;
	[Header("Left")]
	public Barrier leftBarrier;
	[Header("Right")]
	public Barrier rightBarrier;

	public override void add(){
		if (gameObject.activeSelf) {
			getLevel ().addToAllElements (transform.position, this);
		}
	}

	public override void remove(){
		if(gameObject != null && getLevel() != null){
				getLevel ().removeFromAllElements(gameObject.transform.position);
			}
		
	}

	public Barrier getBarrierByDirection(Vector3 direction){
		direction =  Quaternion.Inverse(transform.rotation) * direction;
		if (direction == Vector3.forward) {
			return frontBarrier;
		}
		if (direction == Vector3.back) {
			return backBarrier;
		}
		if (direction == Vector3.left) {
			return leftBarrier;
		}
		if (direction == Vector3.right) {
			return rightBarrier;
		}
		if (direction == Vector3.up) {
			return topBarrier;
		}
		if (direction == Vector3.down) {
			return bottomBarrier;
		}
		return frontBarrier;
	}

	void BarrierColor(Barrier barrier){

		if (barrier == Barrier.NotWalkable) {
			Gizmos.color = Color.red;
		} else if (barrier == Barrier.Stairs) {
			Gizmos.color = Color.yellow;
		} else {
			Gizmos.color = Color.green;
		}

	}
		

	void OnDrawGizmos()
	{
		BarrierColor (topBarrier);
		Gizmos.DrawWireCube(transform.position + transform.up * 0.4f, transform.rotation * new Vector3(0.7f,0.01f,0.7f));
		BarrierColor (bottomBarrier);
		Gizmos.DrawWireCube(transform.position + transform.up * -0.4f, transform.rotation * new Vector3(0.7f,0.01f,0.7f));
		BarrierColor (frontBarrier);
		Gizmos.DrawWireCube(transform.position + transform.forward * 0.4f, transform.rotation * new Vector3(0.7f,0.7f,0.01f));
		BarrierColor (backBarrier);
		Gizmos.DrawWireCube(transform.position + transform.forward * -0.4f, transform.rotation * new Vector3(0.7f,0.7f,0.01f));
		BarrierColor (leftBarrier);
		Gizmos.DrawWireCube(transform.position + transform.right * -0.4f, transform.rotation * new Vector3(0.01f,0.7f,0.7f));
		BarrierColor (rightBarrier);
		Gizmos.DrawWireCube(transform.position + transform.right * 0.4f, transform.rotation * new Vector3(0.01f,0.7f,0.7f));
	}
		

}


