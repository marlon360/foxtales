using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexPathElement : PathElement {

	public override void add(){
		foreach (Transform cube in transform) {
			PathElement element = cube.GetComponent<PathElement> ();
			if (element != null && cube.gameObject.activeSelf) {
				element.setLevel (this.getLevel());
				element.add ();
			}
		}
	}

	public override void remove(){

		foreach (Transform cube in transform) {
			PathElement element = cube.GetComponent<PathElement> ();
			if (element != null && element.gameObject.activeSelf) {
				element.remove();
			}
		}

	}
}
