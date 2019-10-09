using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarObject{

	private AStarObject parent;
	private int F;
	private int G;
	private int H;
	private Vector3 position;
	private static Vector3 destination;

	public AStarObject(AStarObject parent, Vector3 position){

		this.position = position;

		if (parent != null) {
			this.parent = parent;
			G = parent.G + 1;
		} else {
			G = 0;
		}
		H = vectorLength(position - destination);
		F = G+H;

	}

	public void setParent(AStarObject parent){
		this.parent = parent;
		G = parent.G + 1;
	}

	public AStarObject getParent(){
		return parent;
	}

	public static void setDestination(Vector3 destination){
		AStarObject.destination = destination;
	}

	public static Vector3 getDestination(){
		return AStarObject.destination;
	}

	public int getF(){
		return F;
	}

	public bool greaterFThan(AStarObject other){
		return F > other.getF ();
	}

	public Vector3 getPosition(){
		return position;
	}

	public int getG(){
		return G;
	}

	int vectorLength(Vector3 position){
		return (int) (Mathf.Abs (position.x) + Mathf.Abs (position.y) + Mathf.Abs (position.z));
	}

	public override string ToString(){
		return "Position: " + position + " F: " + F + " G: " + G + " H: " + H;
	}

}
