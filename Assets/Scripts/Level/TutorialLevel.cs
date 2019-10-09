using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLevel : Level
{

	public RectTransform circle;
	public Text description;

	bool movingStage = false;
	bool movingStageStart = false;
	public SimplePathElement firstPath;


	bool slideDownStage = false;
	bool slideDownStageStart = false;
	public SimpleMovableElement movableElement;

	bool secSlideStage = false;
	bool secSlideStageStart = false;
	public SimpleMovableElement movableElement2;


	bool moveStoneWithOtherStage = false;
	bool moveStoneWithOtherStageStart = false;
	public SimpleMovableElement movableElement3;

	bool goOnTopStage = false;
	bool goOnTopStageStart = false;
	bool movedOnTopStage = false;
	bool movedOnTopStageStart = false;
	public SimpleMovableElement movableElement5;

	bool exitStage = false;
	bool exitStageStart = false;
	public PathElement exit;

	bool finished = false;

	new void Start() {
		base.Start ();
	}
		

	void Update ()
	{

		if (!movingStage) {
			if (!movingStageStart) {
				description.text = "Klicke auf einen Stein, um dich dort hinzubewegen!";
				movingStageStart = true;
			}

			Vector2 screenPos = Camera.main.WorldToScreenPoint (firstPath.getPosition ());
			circle.position = Vector3.Lerp (circle.position, new Vector3 (screenPos.x, screenPos.y, 0), Time.deltaTime * 3f);

			if (base.player.getPosition () == firstPath.getPosition () + Vector3.up) {
				movingStage = true;
			}
		} else if (!slideDownStage) {
			if (!slideDownStageStart) {
				UIManager.GetInstance ().LevelNamePanel.SetActive (false);
				UIManager.GetInstance ().ShowSmallMessage ("Steine lassen sich in die Richtung bewegen, in die das rote Quadrat zeigt!");
				description.text = "Ziehe den Stein runter, um drüber zugehen!";
				slideDownStageStart = true;
			}

			Vector2 screenPos = Camera.main.WorldToScreenPoint (movableElement.getPosition ());
			circle.position = Vector3.Lerp (circle.position, new Vector3 (screenPos.x, screenPos.y, 0), Time.deltaTime * 3f);

			if (movableElement.getPosition ().y == 0f) {
				slideDownStage = true;
			}
		}else if (!secSlideStage) {
			if (!secSlideStageStart) {
				UIManager.GetInstance ().HideSmallMessage();
				description.text = "Ziehe den Stein aus dem Weg!";
				secSlideStageStart = true;
			}

			Vector2 screenPos = Camera.main.WorldToScreenPoint (movableElement2.getPosition ());
			circle.position = Vector3.Lerp (circle.position, new Vector3 (screenPos.x, screenPos.y, 0), Time.deltaTime * 3f);

			if (movableElement2.getPosition ().x == 5f) {
				secSlideStage = true;
			}
		}else if (!goOnTopStage) {
			if (!goOnTopStageStart) {
				UIManager.GetInstance ().ShowSmallMessage ("Klicke auf meinen Kopf. um das Menü zu öffnen!");
				description.text = "Gehe auf den Stein!";
				goOnTopStageStart = true;
			}

			Vector2 screenPos = Camera.main.WorldToScreenPoint (movableElement5.getPosition ());
			circle.position = Vector3.Lerp (circle.position, new Vector3 (screenPos.x, screenPos.y, 0), Time.deltaTime * 3f);

			if (movableElement5.IsPlayerOnTop()) {
				goOnTopStage = true;
			}

		}else if (!movedOnTopStage) {
			if (!movedOnTopStageStart) {
				UIManager.GetInstance ().HideSmallMessage();
				description.text = "Du kannst auch Steine bewegen, während du auf ihnen stehst!";
				movedOnTopStageStart = true;
			}

			Vector2 screenPos = Camera.main.WorldToScreenPoint (movableElement5.getPosition ());
			circle.position = Vector3.Lerp (circle.position, new Vector3 (screenPos.x, screenPos.y, 0), Time.deltaTime * 3f);

			if (movableElement5.getPosition().z == 2f) {
				movedOnTopStage = true;
			}

		}else if (!moveStoneWithOtherStage) {
			if (!moveStoneWithOtherStageStart) {
				UIManager.GetInstance ().ShowSmallMessage ("Ziehe irgendwo, um die Ansicht zu drehen! 2 Finger pinch, um zu zoomen!");
				description.text = "Bewege mit einem Stein einen anderen!";
				moveStoneWithOtherStageStart = true;
			}

			Vector2 screenPos = Camera.main.WorldToScreenPoint (movableElement3.getPosition ());
			circle.position = Vector3.Lerp (circle.position, new Vector3 (screenPos.x, screenPos.y, 0), Time.deltaTime * 3f);

			if (movableElement3.getPosition ().x != 18f) {
				moveStoneWithOtherStage = true;
			}

		}else if (!exitStage) {
			if (!exitStageStart) {
				description.text = "Gehe zum Ausgang!";
				exitStageStart = true;
			}

			Vector2 screenPos = Camera.main.WorldToScreenPoint (exit.getPosition ());
			circle.position = Vector3.Lerp (circle.position, new Vector3 (screenPos.x, screenPos.y, 0), Time.deltaTime * 3f);



		}  else if (!finished) {
				UIManager.GetInstance ().ShowSmallMessage ("Sehr gut!", 3f);
				finished = true;
				circle.gameObject.SetActive (false);

		} else {
			//FINISHED
		}
	}
		



}
