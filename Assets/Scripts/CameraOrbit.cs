using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
	protected Transform _xForm_Camera;
	protected Transform _xForm_Parent;

	protected Vector3 _LocalRotation;
	protected Vector3 _LocalPosition;
	protected float _CameraDistance;

	Vector3 NewPosition;

	public float RotationSensitivity;
	public float MouseSensitivity = 4f;
	public float TouchSensitivity = 0.3f;
	public float MousePositionSensitivity = 0.12f;
	public float ScrollSensitivity = 2f;
	public float OrbitDamping = 10f;
	public float MoveDamping = 5f;
	public float ScrollDamping = 6f;

	public bool CameraDisabled = true;
	public float pointer_x;
	public float pointer_y;

	public float maxCameraDistance = 40f;

	public bool autoRotate = false;

	public Transform target;
    public Vector3 offset;

	void Start(){
		this._xForm_Camera = this.transform;
		this._xForm_Parent = this.transform.parent;

		_LocalRotation.y = _xForm_Parent.rotation.eulerAngles.x;
		_LocalRotation.x = _xForm_Parent.rotation.eulerAngles.y;
		_CameraDistance = _xForm_Camera.localPosition.z * -1f;

		RotationSensitivity = MouseSensitivity;

	}

	void LateUpdate(){

		if (Input.touchCount > 0) {
			RotationSensitivity = TouchSensitivity;
		} else {
			RotationSensitivity = MouseSensitivity;
		}

		if (!CameraDisabled && Input.touchCount != 2 ) {
			if (pointer_x != 0 || pointer_y != 0) {

				_LocalRotation.x += pointer_x * RotationSensitivity;
				_LocalRotation.y -= pointer_y * RotationSensitivity;

				_LocalRotation.y = Mathf.Clamp (_LocalRotation.y, 0f, 90f);

			}
		}

		if (autoRotate) {
			_LocalRotation.x = Mathf.Lerp(_LocalRotation.x, _LocalRotation.x+15f, Time.deltaTime);

		}

			if (Input.GetAxis ("Mouse ScrollWheel") != 0f) {

				float ScrollAmount = Input.GetAxis ("Mouse ScrollWheel") * ScrollSensitivity;

				ScrollAmount *= (this._CameraDistance * 0.3f);

				this._CameraDistance += ScrollAmount * -1f;

				this._CameraDistance = Mathf.Clamp (this._CameraDistance, 1.5f, maxCameraDistance);

			}

		if (Input.touchCount == 2) {
			// Store both touches.
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


			float ScrollAmount = deltaMagnitudeDiff * 0.01f * -1f;

			ScrollAmount *= (this._CameraDistance * 0.3f);

			this._CameraDistance += ScrollAmount * -1f;

			this._CameraDistance = Mathf.Clamp (this._CameraDistance, 1.5f, 100f);


		}

//		if (Input.GetMouseButton (1)) {
//			if (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0) {
//
//				_LocalPosition.x -= Input.GetAxis ("Mouse X") * MousePositionSensitivity;
//				_LocalPosition.z -= Input.GetAxis ("Mouse Y") * MousePositionSensitivity;
//
//				NewPosition = Quaternion.Euler (_LocalRotation.y, _LocalRotation.x, 0) * _LocalPosition;
//				NewPosition.y = 0f;
//			}
//
//		}

			Quaternion QT = Quaternion.Euler (_LocalRotation.y, _LocalRotation.x, 0);
			this._xForm_Parent.rotation = Quaternion.Lerp (this._xForm_Parent.rotation, QT, Time.deltaTime * OrbitDamping);

            NewPosition = target.position + offset;

			this._xForm_Parent.position = Vector3.Lerp (this._xForm_Parent.position, NewPosition, Time.deltaTime * MoveDamping);

			if (this._xForm_Camera.localPosition.z != this._CameraDistance * -1f) {
				this._xForm_Camera.localPosition = new Vector3 (0f, 0f, Mathf.Lerp (this._xForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDamping));
			}
		
		CameraDisabled = true;
	}
}
