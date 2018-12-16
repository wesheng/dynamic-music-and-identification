using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {

    [Header("Controls")]
    [SerializeField] private string _horizontalAxis = "Horizontal";
    [SerializeField] private string _verticalAxis = "Vertical";
    [SerializeField] private string _jumpButton = "Jump";

    [Header("Settings")]
    [SerializeField] private float _raycastDistance;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _strafeSpeed = 4;
    [SerializeField] private float _forwardSpeed = 7;
    [SerializeField] private float _jumpForce = 7;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private Transform _objectForward;

    public bool CanJump { get { return _canJump; } set { _canJump = value; } }


    private Rigidbody _rigidBody;
    private Transform _transform;

    // Use this for initialization
    void Start () {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = transform;
	}
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis(_horizontalAxis);
        float vertical = Input.GetAxis(_verticalAxis);

        //Vector3 forward = _objectForward.forward;
        //Vector3 right = _objectForward.right;

        Vector3 forward = _objectForward.forward * (vertical * _strafeSpeed);
        Vector3 right = _objectForward.right * (horizontal * _strafeSpeed);

        Vector3 force = forward + right;

        _rigidBody.AddForce(force, ForceMode.Impulse);

        //Vector3 forwardForce = new Vector3(forward.x, 0, forward.z).normalized * vertical;
        //forwardForce *= (vertical > 0 ? _forwardSpeed : _strafeSpeed);

        //Vector3 rightForce = new Vector3(right.x, 0, right.z).normalized * horizontal * _strafeSpeed;

        //Vector3 translatePos = new Vector3(horizontal, 0, vertical);
        
        // todo switch to raycasting
        //RaycastHit raycastHit;
        //if (Physics.Raycast(new Ray(_transform.position, Vector3.down), out raycastHit, _raycastDistance, _layerMask))
        //{
        //    translatePos.y = -raycastHit.distance;
        //    // _transform.Translate(new Vector3(horizontal, 0, vertical));
        //}

        // _transform.Translate(translatePos);
        //_rigidBody.AddForce(forwardForce);
        //_rigidBody.AddForce(rightForce);

        if (_canJump && Input.GetButtonDown(_jumpButton))
            _rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
	}
}
