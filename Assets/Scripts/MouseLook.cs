using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    private InputMaster controls;
    
    [Header("Setting")]
    public float mouseSensitivity = 100f;

    float xRotation = 0f;

    private Transform playerBody;

    void Awake()
    {
        playerBody = transform.parent;

        controls = new InputMaster();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();  
    }

    private void Look()
    {
        Vector2 mouseLook = controls.PlayerActions.Look.ReadValue<Vector2>();

        float mouseX = mouseLook.x * Time.deltaTime * mouseSensitivity;
        float mouseY = mouseLook.y * Time.deltaTime * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -87.5f, 87.5f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
