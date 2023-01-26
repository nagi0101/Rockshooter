using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Transform References")]
    [SerializeField] private Transform movementOrientation;
    [SerializeField] private Transform characterMesh;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float gravitationalAcceleration;
    [SerializeField] private float jumpForce;
    [Space(10.0f)]
    [SerializeField, Range(0.0f, 1.0f)] private float lookForwardThreshold;
    [SerializeField] private float lookForwardSpeed;

    private float horizontalInput;
    private float verticalInput;
    private bool jumpFlag;

    private CharacterController m_characterController;
    private GroundChecker m_groundChecker;
    private Vector3 velocity;
    private Vector3 lastFixedPosition;
    private Quaternion lastFixedRotation;
    private Vector3 nextFixedPosition;
    private Quaternion nextFixedRotation;



    // Start is called before the first frame update
    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_groundChecker = GetComponent<GroundChecker>();
        velocity = new Vector3(0, 0, 0);
        lastFixedPosition = transform.position;
        lastFixedRotation = transform.rotation;
        nextFixedPosition = transform.position;
        nextFixedRotation = transform.rotation;

        horizontalInput = 0.0f;
        verticalInput = 0.0f;
        jumpFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpFlag = true;
        }

        float interpolationAlpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        m_characterController.Move(Vector3.Lerp(lastFixedPosition, nextFixedPosition, interpolationAlpha) - transform.position);
        characterMesh.rotation = Quaternion.Slerp(lastFixedRotation, nextFixedRotation, interpolationAlpha);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        lastFixedPosition = nextFixedPosition;
        lastFixedRotation = nextFixedRotation;

        Vector3 planeVelocity = GetXZVelocity(horizontalInput, verticalInput);
        float yVelocity = GetYVelocity();
        velocity = new Vector3(planeVelocity.x, yVelocity, planeVelocity.z);

        if (planeVelocity.magnitude / speed >= lookForwardThreshold)
        {
            nextFixedRotation = Quaternion.Slerp(characterMesh.rotation, Quaternion.LookRotation(planeVelocity), lookForwardSpeed * Time.fixedDeltaTime);
        }

        nextFixedPosition = transform.position + velocity * Time.fixedDeltaTime;
    }

    private Vector3 GetXZVelocity(float horizontalInput, float verticalInput)
    {
        Vector3 moveVelocity = movementOrientation.forward * verticalInput + movementOrientation.right * horizontalInput;
        Vector3 moveDirection = moveVelocity.normalized;
        float moveSpeed = Mathf.Min(moveVelocity.magnitude, 1.0f) * speed;

        return moveDirection * moveSpeed;
    }

    /// <remarks>
    /// This function must be called only in FixedUpdate()
    /// </remarks>
    private float GetYVelocity()
    {
        if (!m_groundChecker.IsGrounded())
        {
            return velocity.y - gravitationalAcceleration * Time.fixedDeltaTime;
        }

        if (jumpFlag)
        {
            jumpFlag = false;
            return velocity.y + jumpForce;
        }
        else
        {
            return Mathf.Max(0.0f, velocity.y);
        };
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody)
        {
            hit.rigidbody.AddForce(velocity / hit.rigidbody.mass);
        }
    }

}
