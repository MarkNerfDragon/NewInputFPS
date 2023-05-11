using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    private InputMaster controls;
    
    [Header("Settings")]
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    [SerializeField] private Transform playerBody;

    void Awake()
    {
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

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -87.5f, 87.5f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
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
