using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour {

    [Header("Controls")]
    [SerializeField] private string _lookHorizontalAxis = "Mouse X";
    [SerializeField] private string _lookVerticalAxis = "Mouse Y";

    [Header("Settings")]
    [SerializeField] private float _sensitivity = 10.0f;
    [SerializeField] private bool _invertXAxis = false;
    [SerializeField] private bool _invertYAxis = false;
    [SerializeField] [Range(-180, 180)] private float _minAngle = -90;
    [SerializeField] [Range(-180, 180)] private float _maxAngle = 90;
    [SerializeField] private bool _lockCursor = true;

    Transform _transform;

    private float _xAngle;
    private float _yAngle;


    // Use this for initialization
    void Start () {
	    _transform = transform;

        if (_lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180) angle = 360 - angle;
        angle = Mathf.Clamp(angle, min, max);
        return angle;
    }

    // Update is called once per frame
    void Update () {
        float horizontal = Input.GetAxisRaw(_lookHorizontalAxis) * _sensitivity * Time.deltaTime;
        float vertical = Input.GetAxisRaw(_lookVerticalAxis) * _sensitivity * Time.deltaTime;
        horizontal *= _invertXAxis ? -1 : 1;
        vertical *= _invertYAxis ? -1 : 1;


        _yAngle += horizontal;
        _xAngle -= vertical;

        // print(eulerAngles);
        _xAngle = ClampAngle(_xAngle, _minAngle, _maxAngle);

        _transform.rotation = Quaternion.Euler(new Vector3(_xAngle, _yAngle, 0));
    }
}
