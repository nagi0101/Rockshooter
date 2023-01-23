using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : NetworkBehaviour
{
    [Header("Position")]
    [SerializeField] private Vector3 cameraOffset;

    [Header("Input")]
    [SerializeField] private float verticalSensitive;
    [SerializeField] private float horizontalSensitive;
    [SerializeField] private bool invertVerticalRotation;
    [SerializeField] private float verticalUpwardLimit;
    [SerializeField] private float verticalDownwardLimit;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner || !Camera.main) return;

        Camera.main.transform.parent = transform;
        Camera.main.transform.position = cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float horizontalDelta = Input.GetAxis("Mouse X") * horizontalSensitive * Time.deltaTime;
        float verticalDelta = -Input.GetAxis("Mouse Y") * verticalSensitive * Time.deltaTime;
        if (invertVerticalRotation) verticalDelta *= -1;


        transform.Rotate(new Vector3(0, horizontalDelta, 0));

        Vector3 currentRotation = Camera.main.transform.eulerAngles;
        currentRotation.x = currentRotation.x > 180.0f ? currentRotation.x - 360 : currentRotation.x;
        float clampedAngle = Mathf.Clamp(currentRotation.x + verticalDelta, -verticalUpwardLimit, verticalDownwardLimit);
        Vector3 newRotation = new Vector3(clampedAngle, currentRotation.y, currentRotation.z);
        Camera.main.transform.rotation = Quaternion.Euler(newRotation);
    }
}
