using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Movement{Slidable, NotSlidable};

public class SimpleMovableElement : SimplePathElement {

	[Header("Sliding")]
	[Header("Left/Right")]
	public Movement xAxis = Movement.NotSlidable;
	[Header("Up/Down")]
	public Movement yAxis = Movement.NotSlidable;
	[Header("Forward/Backward")]
	public Movement zAxis = Movement.NotSlidable;



	bool playerOnTop;

	Vector3 startPosition;

	Range3D movementRange = new Range3D();
	Range3D playerOnTopMovementRange = new Range3D ();


	SimpleMovableElement sliderOnMinWay;
	bool sliderOnMinWayActive;
	SimpleMovableElement sliderOnMaxWay;
	bool sliderOnMaxWayActive;

	SimpleMovableElement sliderOnMinWayX;
	SimpleMovableElement sliderOnMinWayY;
	SimpleMovableElement sliderOnMinWayZ;

	SimpleMovableElement sliderOnMaxWayX;
	SimpleMovableElement sliderOnMaxWayY;
	SimpleMovableElement sliderOnMaxWayZ;

	//Variablen für Veränderung der Emission
	Renderer stoneRenderer;
	Material mat;
	Color color;
	float fadeTime = 0.25f;
	private float tColor = 1;
	private bool colorActive = false;

	SimpleMovableElement parent;

	private CameraOrbit cameraOrbit;

	void Start(){
		updateStartPosition ();
		stoneRenderer = GetComponentInChildren<Renderer> ();
		mat = stoneRenderer.material;
		parent = this;
		cameraOrbit = FindObjectOfType<CameraOrbit>();
	}


	void Update(){
		if (tColor < 1) {
			if (colorActive) {
				tColor += Time.deltaTime / fadeTime;
				color = Color.Lerp (Color.red, Color.white, tColor);
				mat.SetColor ("_EmissionColor", color);
			} else {
				tColor += Time.deltaTime / fadeTime;
				color = Color.Lerp (Color.white, Color.red, tColor);
				mat.SetColor ("_EmissionColor", color);
			}
		}



	}

	void LateUpdate(){
		if (sliderOnMinWay != null) {
			if (sliderOnMinWayActive) {
				sliderOnMinWay.transform.position = transform.position - parent.getPositiveDirectionOfAxis ();
			}
		}
		if (sliderOnMaxWay != null) {
			if (sliderOnMaxWayActive) {
				sliderOnMaxWay.transform.position = (transform.position + parent.getPositiveDirectionOfAxis ());

			}
		}
	}

	#if UNITY_EDITOR
	void OnValidate(){
		turnModelToDirection ();
	}
	#endif

	public void turnModelToDirection(){
		foreach(Transform element in gameObject.transform){
			if(element.tag == "Model"){
				if (xAxis == Movement.Slidable) {
					element.transform.eulerAngles = new Vector3 (0, 90, 0);
				}
				if (yAxis == Movement.Slidable) {
					element.transform.eulerAngles = new Vector3 (90, 0, 0);
				}
				if (zAxis == Movement.Slidable) {
					element.transform.eulerAngles = new Vector3 (0, 0, 0);
				}
			}
		}
	}

	public void setColorActive(){
		tColor = 0;
		colorActive = true;
	}

	public void setColorInactive(){
		tColor = 0;
		colorActive = false;
	}
		

	public void updateStartPosition(){
		startPosition = transform.position;
	}

	public void SetPlayerOnTop(bool isOnTop){
		playerOnTop = isOnTop;
	}

	public bool IsPlayerOnTop(){
		return playerOnTop;
	}


	public void hasClicked(){
		setColorActive ();
	}

	public void hasReleased(){
		setColorInactive ();
	}

	public void startDragging(){
		if (sliderOnMinWay != null) {
			sliderOnMinWay.sliderOnMaxWayActive = false;
		}
		if (sliderOnMaxWay != null) {
			sliderOnMaxWay.sliderOnMinWayActive = false;
		}
		setCurrentSlider ();
		this.parent = this;
	}
		
		

	public void pushInMin(Vector3 dragPosition){
		if (parent.getAxisOfVector(dragPosition) - parent.getAxisOfVector(transform.position) < 0) {

			if (sliderOnMaxWayActive) {
				sliderOnMaxWayActive = false;
				this.parent = this;
				setCurrentSlider ();
			}

			if (sliderOnMinWay != null) {
				if(parent.getAxisOfVector(dragPosition) < parent.getAxisOfVector(sliderOnMinWay.transform.position) + 1 && Mathf.Abs(parent.getAxisOfVector(getPosition()) - parent.getAxisOfVector(sliderOnMinWay.getPosition())) < 1.5){
					sliderOnMinWayActive = true;
					sliderOnMinWay.setCurrentSlider (this.parent);
					sliderOnMinWay.parent = parent;
					sliderOnMinWay.pushInMin (dragPosition - parent.getPositiveDirectionOfAxis ());
					if (parent.getAxisOfVector (sliderOnMinWay.GetCurrentRange ().min) >= parent.getAxisOfVector (GetCurrentRange ().min)) {
						GetCurrentRange ().min = parent.setAxisOfVector (GetCurrentRange ().min, parent.getAxisOfVector (sliderOnMinWay.GetCurrentRange ().min) + 1);
					}
				}
			}

		}
	}


	public void pushInMax(Vector3 dragPosition){
		if (parent.getAxisOfVector(dragPosition) - parent.getAxisOfVector(transform.position) > 0) {

			if (sliderOnMinWayActive) {
				sliderOnMinWayActive = false;
				this.parent = this;
				setCurrentSlider ();
			}


			if (sliderOnMaxWay != null) {
				if(parent.getAxisOfVector(dragPosition) > parent.getAxisOfVector(sliderOnMaxWay.transform.position) - 1 && Mathf.Abs(parent.getAxisOfVector(getPosition()) - parent.getAxisOfVector(sliderOnMaxWay.getPosition())) < 1.5){
					sliderOnMaxWayActive = true;
					sliderOnMaxWay.setCurrentSlider (this.parent);
					sliderOnMaxWay.parent = parent;
					sliderOnMaxWay.pushInMax (dragPosition + parent.getPositiveDirectionOfAxis ());
					if (parent.getAxisOfVector(sliderOnMaxWay.GetCurrentRange ().max) <= parent.getAxisOfVector(GetCurrentRange ().max)) {
						GetCurrentRange ().max = parent.setAxisOfVector (GetCurrentRange ().max, parent.getAxisOfVector (sliderOnMaxWay.GetCurrentRange ().max) - 1);
					}
				}
			}
		}
	}

		

	public void isDragging (Vector3 dragPosition)
	{

		pushInMax (dragPosition);
		pushInMin (dragPosition);


		setPosition (dragPosition);

		if (IsPlayerOnTop()) {
			cameraOrbit.enabled = false;
		}

	}

	public void updateMin(){
		if (sliderOnMinWay != null) {
			sliderOnMinWay.roundPosition();
			sliderOnMinWay.updatePosition ();
			sliderOnMinWay.updateStartPosition();

			if (sliderOnMinWay.IsPlayerOnTop ()) {
				getLevel().setPlayerPosition(sliderOnMinWay.getPosition () + Vector3.up);
			}

			sliderOnMinWay.updateMin ();

		}
	}

	public void updateMax(){
		if (sliderOnMaxWay != null) {			
			sliderOnMaxWay.roundPosition();
			sliderOnMaxWay.updatePosition ();
			sliderOnMaxWay.updateStartPosition();

			if (sliderOnMaxWay.IsPlayerOnTop ()) {
				getLevel().setPlayerPosition(sliderOnMaxWay.getPosition () + Vector3.up);
			}

			sliderOnMaxWay.updateMax ();

		}
	}

	public void endDragging(){

		roundPosition();
		updatePosition ();
		updateStartPosition();

		if (IsPlayerOnTop ()) {
			getLevel().setPlayerPosition(getPosition () + Vector3.up);
		}

		updateMin ();
		updateMax ();


		sliderOnMinWayActive = false;
		sliderOnMaxWayActive = false;

		getLevel ().updateAllMoveRanges ();

		cameraOrbit.enabled = true;
	}
		

	public void setPosition(Vector3 dragValue){

		if (xAxis == Movement.Slidable) {
			setNewPosition(new Vector3 (dragValue.x, getPosition().y, getPosition().z));
		}
		if (yAxis == Movement.Slidable) {
			setNewPosition(new Vector3 (getPosition().x, dragValue.y, getPosition().z));
		}
		if (zAxis == Movement.Slidable) {
			setNewPosition(new Vector3 (getPosition().x, getPosition().y, dragValue.z));
		}
	}

	void setNewPosition(Vector3 newPos){
		if (GetCurrentRange().isPointInRange (newPos)) {
			transform.position = newPos;
		}else{
			if (xAxis == Movement.Slidable) {
				transform.position = new Vector3 (GetCurrentRange().getLastInRange (newPos).x, getPosition().y, getPosition().z);
			}
			if (yAxis == Movement.Slidable) {
				transform.position = new Vector3 (getPosition().x,GetCurrentRange().getLastInRange (newPos).y, getPosition().z);
			}
			if (zAxis == Movement.Slidable) {
				transform.position = new Vector3 (getPosition().x, getPosition().y, GetCurrentRange().getLastInRange (newPos).z);
			}
		}
	}

	public override void add(){
		base.add ();
		getLevel ().addToMovableElements (this);
	}
	public override void remove(){
		getLevel ().removeFromAllElements(transform.position);
		getLevel ().removeFromMovableElements (this);
	}

	public void updatePosition(){
		getLevel ().updateElementPosition (startPosition, transform.position,this);

	}

	public void updateMoveRange(){
		Vector3[] directions = {
			Vector3.forward,
			Vector3.back,
			Vector3.left,
			Vector3.right,
			Vector3.up,
			Vector3.down
		};
		List<Vector3> moveRange = new List<Vector3> ();
		moveRange.Add (transform.position);

		bool playerInsideElement = false;
		List<Vector3> playerOnTopRange = new List<Vector3> ();

		SimplePathElement firstPlayerPositionElement = null;
		getLevel().GetAllElements().TryGetValue (transform.position + Vector3.up, out firstPlayerPositionElement);
		if (firstPlayerPositionElement == null) {
			playerOnTopRange.Add (transform.position);
		} else {
			playerInsideElement = true;
		}

		sliderOnMaxWayX = null;
		sliderOnMinWayX = null;
		sliderOnMaxWayY = null;
		sliderOnMinWayY = null;
		sliderOnMaxWayZ = null;
		sliderOnMinWayZ = null;

		foreach (Vector3 direction in directions) {
			bool hasFoundWay;
			Vector3 newDirection = direction;

			bool playerRangeEnded = playerInsideElement;

				do {
					hasFoundWay = false;
					

					Vector3 walkPosition = transform.position + newDirection;
					Vector3 playerElementPosition = walkPosition + Vector3.up;

					SimplePathElement walkElement = null;
					SimplePathElement playerPositionElement = null;

					getLevel().GetAllElements().TryGetValue (walkPosition, out walkElement);
					getLevel().GetAllElements().TryGetValue (playerElementPosition, out playerPositionElement);


					if(direction != Vector3.up && getLevel().GetPlayer().isPositionInWalkingPath(walkPosition)){
						hasFoundWay = false;
						continue;
					}


					if(walkElement != null){
						if(walkElement.gameObject.GetComponent<SimpleMovableElement>() != null){
							if(getSliderOnWayByDirection(direction) == null && !walkElement.gameObject.Equals(gameObject)){
								setSliderOnWayByDirection(direction, walkElement.gameObject.GetComponent<SimpleMovableElement>());
							}
						}
					}

					//Normal Walk
					if (walkElement == null || (walkElement.gameObject.GetComponent<SimpleMovableElement>() != null)) {
						
						if (getLevel().getRange().isPointInRange(walkPosition)) {

						if(direction != Vector3.down && !playerRangeEnded){
							if(playerPositionElement == null){
								playerOnTopRange.Add (transform.position + newDirection);
							}else{
								playerRangeEnded = true;
							}
						}
						if(direction == Vector3.down){
							if(playerPositionElement == this || playerPositionElement == null){
								playerOnTopRange.Add (transform.position + newDirection);
							}else{
								playerRangeEnded = true;
							}
						}


								moveRange.Add (transform.position + newDirection);
								hasFoundWay = true;
							
						}
					}
					newDirection = newDirection + direction;

				} while(hasFoundWay);
		}
		movementRange.setRangeFromArray (moveRange.ToArray ());


		playerOnTopMovementRange.setRangeFromArray (playerOnTopRange.ToArray ());

	}

	public Range3D GetCurrentRange(){
		if (playerOnTop) {
			return playerOnTopMovementRange;
		} else {
			return movementRange;
		}
	}

	public Movement getMovementByDirection(Vector3 direction){
		if (direction == Vector3.forward) {
			return zAxis;
		}
		if (direction == Vector3.back) {
			return zAxis;
		}
		if (direction == Vector3.left) {
			return xAxis;
		}
		if (direction == Vector3.right) {
			return xAxis;
		}
		if (direction == Vector3.up) {
			return yAxis;
		}
		if (direction == Vector3.down) {
			return yAxis;
		}
		return zAxis;
	}

	public SimplePathElement getSliderOnWayByDirection(Vector3 direction){
		if (direction == Vector3.forward) {
			return sliderOnMaxWayZ;
		}
		if (direction == Vector3.back) {
			return sliderOnMinWayZ;
		}
		if (direction == Vector3.left) {
			return sliderOnMinWayX;
		}
		if (direction == Vector3.right) {
			return sliderOnMaxWayX;
		}
		if (direction == Vector3.up) {
			return sliderOnMaxWayY;
		}
		if (direction == Vector3.down) {
			return sliderOnMinWayY;
		}
		return sliderOnMaxWayZ;
	}
	public void setSliderOnWayByDirection(Vector3 direction, SimpleMovableElement element){
		if (direction == Vector3.forward) {
			sliderOnMaxWayZ = element;
		}
		if (direction == Vector3.back) {
			sliderOnMinWayZ = element;
		}
		if (direction == Vector3.left) {
			sliderOnMinWayX = element;
		}
		if (direction == Vector3.right) {
			sliderOnMaxWayX = element;
		}
		if (direction == Vector3.up) {
			sliderOnMaxWayY = element;
		}
		if (direction == Vector3.down) {
			sliderOnMinWayY = element;
		}
	}

	public void setCurrentSlider(SimpleMovableElement element){
		if (element.xAxis == Movement.Slidable) {
			sliderOnMaxWay = sliderOnMaxWayX;
			sliderOnMinWay = sliderOnMinWayX;
		}
		if (element.yAxis == Movement.Slidable) {
			sliderOnMaxWay = sliderOnMaxWayY;
			sliderOnMinWay = sliderOnMinWayY;
		}
		if (element.zAxis == Movement.Slidable) {
			sliderOnMaxWay = sliderOnMaxWayZ;
			sliderOnMinWay = sliderOnMinWayZ;
		}
	}
	public void setCurrentSlider(){
		setCurrentSlider (this);
	}

	Vector3 getPositiveDirectionOfAxis(){
		if (xAxis == Movement.Slidable) {
			return Vector3.right;
		}
		if (yAxis == Movement.Slidable) {
			return Vector3.up;
		}
		if (zAxis == Movement.Slidable) {
			return Vector3.forward;
		}
		return Vector3.right;
	}

	float getAxisOfVector(Vector3 vec){
		if (xAxis == Movement.Slidable) {
			return vec.x;
		}
		if (yAxis == Movement.Slidable) {
			return vec.y;
		}
		if (zAxis == Movement.Slidable) {
			return vec.z;
		}
		return vec.x;
	}
	public Vector3 setAxisOfVector(Vector3 vec, float value){
		if (xAxis == Movement.Slidable) {
			vec.x = value;
		}
		if (yAxis == Movement.Slidable) {
			vec.y = value;
		}
		if (zAxis == Movement.Slidable) {
			vec.z = value;
		}
		return vec;
	}
}
