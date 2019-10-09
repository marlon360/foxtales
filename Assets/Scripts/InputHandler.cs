using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour {

	public static event Action<PathElement> hasClickedPathElement;

	public static event Action<SimpleMovableElement> startedDragging;
	public static event Action<SimpleMovableElement, Vector3> isDraggingMovableElement;
	public static event Action<float, float> isDragging;
	public static event Action<SimpleMovableElement> endedDragging;

	public static event Action<SimpleMovableElement> startedHoldingMovableElement;
	public static event Action<SimpleMovableElement> endedHoldingMovableElement;


	Camera mainCamera;
	int layer_mask;

	SimpleMovableElement currentSlidingElement;
	float dragStartDistance;

	Vector2 dragStartPosition;
	bool dragging;
	bool draggingNotMovable;

	float pointer_x;
	float pointer_y;

	SimplePathElement clickedPathElement;
	SimpleMovableElement slidingElement;

	Vector3 dragPosition;



	// Use this for initialization
	void Start () {
		layer_mask = LayerMask.GetMask("PathElementLayer");
		mainCamera = Camera.main;
	}

	// Update is called once per frame
	void Update () {

		//Wenn man auf UI klickt, keine Inputs annehmen
		if (!IsPointerOverUIObject()) {

			clickedPathElement = null;

			if (Input.GetMouseButtonUp (0)) {
				if (!dragging && !draggingNotMovable) {
					RaycastHit hit;
					Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
					if (Physics.Raycast (ray, out hit, 1000, layer_mask)) {
						if (hit.collider.gameObject.GetComponent<SimplePathElement> () != null) {

							clickedPathElement = hit.collider.gameObject.GetComponent<SimplePathElement> ();
							if (hasClickedPathElement != null) {
								hasClickedPathElement (clickedPathElement);
							}

						}
					}
				}
				if (dragging) {
					//EndDragging
					if (slidingElement != null) {
						slidingElement.endDragging ();
						if (endedDragging != null) {
							endedDragging (slidingElement);
						}
					}


				}
				dragging = false;
				draggingNotMovable = false;
				if (currentSlidingElement != null) {
					currentSlidingElement = null;

					//ReleasedSlidingElement
					slidingElement.hasReleased ();
					if (endedHoldingMovableElement != null) {
						endedHoldingMovableElement (slidingElement);
					}
				}

			}

			if (Input.GetMouseButtonDown (0)) {
				dragStartPosition = Input.mousePosition;
				RaycastHit hit;
				Ray ray = mainCamera.ScreenPointToRay (dragStartPosition);
				if (Physics.Raycast (ray, out hit, 1000, layer_mask)) {
					if (hit.collider.gameObject.GetComponent<SimpleMovableElement> () != null) {
						currentSlidingElement = hit.collider.gameObject.GetComponent<SimpleMovableElement> ();
						if (currentSlidingElement != null) {
							slidingElement = currentSlidingElement;
							//startHoldingSlidingElement
							slidingElement.hasClicked ();
							if (startedHoldingMovableElement != null) {
								startedHoldingMovableElement (slidingElement);
							}
						}
						dragStartDistance = (mainCamera.transform.position - currentSlidingElement.transform.position).magnitude;
					}
				}
			}

			pointer_x = Input.GetAxisRaw ("Mouse X");
			pointer_y = Input.GetAxisRaw ("Mouse Y");

			if (Input.touchCount > 0) {
				pointer_x = Input.touches [0].deltaPosition.x;
				pointer_y = Input.touches [0].deltaPosition.y;
			}

			if (pointer_x != 0 || pointer_y != 0) {
				float dragDistance = (dragStartPosition - new Vector2 (Input.mousePosition.x, Input.mousePosition.y)).magnitude;
				if (currentSlidingElement != null) {
					if (dragDistance > 5f) {
						if (dragging == false) {
							//StartDragging
							slidingElement.startDragging ();
							if (startedDragging != null) {
								startedDragging (slidingElement);
							}
						}
						dragging = true;
						dragPosition = mainCamera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, dragStartDistance));
						slidingElement.isDragging (dragPosition);
						if (isDraggingMovableElement != null) {
							isDraggingMovableElement (slidingElement, dragPosition);
						}
					} else {
						dragging = false;
					}
				} else {
					//Not Sliding a movalble Element
					if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) || Input.GetMouseButton (0)) {
						if (isDragging != null) {
							if (dragDistance > 5f) {
								isDragging (pointer_x, pointer_y);
								draggingNotMovable = true;
							}
						}
					}
				}

			}
		}

	}

	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

}
