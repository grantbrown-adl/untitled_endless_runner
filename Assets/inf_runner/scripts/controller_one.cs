using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour {
    [Header("Debug")]
    [SerializeField] bool _movePositionForwardMode;
    [SerializeField] bool _addForceTurnMode;

    [Header("Components")]
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Transform _model;
    [SerializeField] Transform _skiLeft, _skiRight;

    [Header("Computed Variables | Don't Modify")]
    [SerializeField] Vector2 _input;
    [SerializeField] Vector3 _turnForce;
    [SerializeField] Vector3 _forwardMovement;

    [Header("Controller Settings")]
    [SerializeField] float _rotationAmount;
    [SerializeField] float _skiRotationAmount;
    [SerializeField] float _turnSpeed;
    [Min(1.0f)][SerializeField] float _turnSpeedAirDivider;

    [Header("Move Position Mode")]
    [SerializeField] float _forwardSpeed;

    [Header("Controller Jump")]
    [SerializeField] float _playerHeight;
    [SerializeField] float _playerHeightOffset;
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpForceMultiplier;
    [SerializeField] bool _isGrounded;
    [SerializeField] LayerMask _groundLayerMask;
    [SerializeField] RaycastHit hitInfo;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _playerHeight = GetComponent<Collider>().bounds.size.y;
    }

    private void Update() {
        _model.transform.rotation = Quaternion.Euler(0, _rigidbody.velocity.x * _rotationAmount, 0);
        // _skiLeft.transform.rotation = _skiRight.transform.rotation = Quaternion.Euler(0, _model.transform.rotation.x, _rigidbody.velocity.x * _skiRotationAmount);
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out hitInfo, _playerHeight / 2 + _playerHeightOffset, _groundLayerMask);
        Debug.DrawRay(transform.position, Vector3.down * (_playerHeight / 2 + _playerHeightOffset), Color.green);

        // Debug.Log(hitInfo.collider.gameObject.name);
    }

    private void FixedUpdate() {
        Turn();
        MoveForward();
    }

    public void SetInput(Vector2 inputVector) {
        inputVector.Normalize();

        _input = inputVector;
    }

    void MoveForward() {
        if (_movePositionForwardMode) {
            _forwardMovement = _forwardSpeed * Time.fixedDeltaTime * transform.forward;
            _rigidbody.MovePosition(_rigidbody.position + _forwardMovement);
        }
    }

    void Turn() {
        if (Mathf.Abs(_input.x) > 0) {
            if (_addForceTurnMode) {

                _turnForce = (_isGrounded ? _turnSpeed : _turnSpeed / _turnSpeedAirDivider) * _input.x * _rigidbody.transform.right;
                _rigidbody.AddForce(_turnForce);
            }
        }
    }
}
