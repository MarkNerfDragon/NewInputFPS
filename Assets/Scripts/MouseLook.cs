using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
     private InputMaster controls;
    
    [Header("Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraHolder;

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

        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
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
