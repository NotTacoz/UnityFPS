using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBobbing : MonoBehaviour
{
    [SerializeField] private bool _enable = true;

    [Header("View Bobbing Settings")]
    [SerializeField] private float _amplitude = 0.015f;
    [SerializeField] private float _frequency = 10.0f;

    [Header("Cameras")]
    [SerializeField] private Transform _camera = null;
    // [SerializeField] private Transform _cameraHolder = null;

    [Header("Weapon")]
    // [SerializeField] private Transform _weapon = null;

    private float _togglespeed = 5.0f;
    private Vector3 _startPosition;
    private Rigidbody _controller;

    public float maxSlopeAngle = 35f;

    public bool grounded;
    public LayerMask whatIsGround;

    private void Awake(){
        _controller = GetComponent<Rigidbody>();
        _startPosition = _camera.localPosition;
    }

    private Vector3 FootStepMotion() {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude;
        return pos;
    }
    
    private void PlayMotion(Vector3 motion) {
        //lerp the localposition to new motion
        // _camera.localPosition = Vector3.Lerp(_camera.localPosition, _camera.localPosition + motion, Time.deltaTime * 1);
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        _camera.localPosition += (motion / _togglespeed * Mathf.Clamp(speed, 0, _togglespeed));
    }

    private bool cancellingGrounded;
        private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }
    
    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }

    private void CheckMotion() {
        ResetPosition();
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

        // if (speed < _togglespeed) return;
        if (!grounded) return;

        PlayMotion(FootStepMotion());
    }

    private void ResetPosition() {
        if (_camera.localPosition == _startPosition) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPosition, Time.deltaTime * 1);
        // _weapon.localPosition = Vector3.Lerp(_weapon.localPosition, _startPosition, Time.deltaTime * 1);
    }

    // private Vector3 FocusTarget() {
    //     Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    //     pos += _cameraHolder.forward * 15.0f;
    //     return pos;
    // }


    // Update is called once per frame
    void Update()
    {
        if (!_enable) return;

        CheckMotion();
        // _camera.LookAt(FocusTarget());

    }
}
