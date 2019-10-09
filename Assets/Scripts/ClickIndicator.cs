using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickIndicator : MonoBehaviour {

	Animator anim;
	SpriteRenderer srenderer;

	public Sprite SuccessSprite;
	public Sprite FailSprite;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		srenderer = GetComponent<SpriteRenderer> ();
	}

	public void clicked(Vector3 position, bool success){
		transform.position = position + new Vector3 (0f, 0.51f, 0f);
		if (success) {
			srenderer.sprite = SuccessSprite;
		} else {
			srenderer.sprite = FailSprite;
		}
		anim.SetBool ("success", success);
		anim.SetTrigger ("clicked");
	}
	

}
