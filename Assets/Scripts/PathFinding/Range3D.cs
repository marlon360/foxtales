using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Range3D {

	public Vector3 min;
	public Vector3 max;

	public void setRangeFromArray(Dictionary<Vector3, SimplePathElement> elements){
		Vector3[] positions = new Vector3[elements.Count];
		elements.Keys.CopyTo(positions, 0);
		setRangeFromArray (positions);
	}

	public void setRangeFromArray(Vector3[] elements){
		if (elements.Length > 0) {
			max.x = elements [0].x;
			min.x = elements [0].x;
			max.y = elements [0].y;
			min.y = elements [0].y;
			max.z = elements [0].z;
			min.z = elements [0].z;

			foreach (Vector3 rangePosition in elements) {
				if (rangePosition.x < min.x)
					min.x = rangePosition.x;
				if (rangePosition.x > max.x)
					max.x = rangePosition.x;
				if (rangePosition.y < min.y)
					min.y = rangePosition.y;
				if (rangePosition.y > max.y)
					max.y = rangePosition.y;
				if (rangePosition.z < min.z)
					min.z = rangePosition.z;
				if (rangePosition.z > max.z)
					max.z = rangePosition.z;

			}
		}
	}
		

	public bool isPointInRange(Vector3 point){
		if (point.x <= max.x && point.x >= min.x) {
			if (point.y <= max.y && point.y >= min.y) {
				if (point.z <= max.z && point.z >= min.z) {
					return true;
				}
			}
		}
		return false;
	}

	public Vector3 getLastInRange(Vector3 point){
		if (point.x < min.x || point.y < min.y || point.z < min.z) {
			return min;
		} else {
			return max;
		}
	}
		
		
	public override string ToString(){
		return "x [" + min.x + "," + max.x + "]" + "y [" + min.y + "," + max.y + "]" + "z [" + min.y + "," + max.y + "]";
	}
	
}
