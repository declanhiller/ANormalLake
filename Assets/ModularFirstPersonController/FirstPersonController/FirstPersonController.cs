// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    private bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    private bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    private bool isWalking = false;
    
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Set internal variables
        playerCamera.fieldOfView = fov;
        jointOriginalPos = joint.localPosition;
        cameraCanMove = false;
        playerCanMove = false;

    }

    private Vector3 movementInput;
    public void OnMove(InputAction.CallbackContext context) {
        Vector2 readValue = context.ReadValue<Vector2>();
        movementInput = new Vector3(readValue.x, 0, readValue.y);
    }
    
    private Vector3 lookInput;
    public void OnLook(InputAction.CallbackContext context) {
        lookInput = context.ReadValue<Vector2>();
    }

    void Start()
    {

    }

    float camRotation;

    public void EnablePlayer() {
        Cursor.lockState = CursorLockMode.Locked;
        cameraCanMove = true;
        playerCanMove = true;
    }

    public bool isZoomKeyDown;
    public void Zoom(InputAction.CallbackContext context) {
        if (context.performed) {
            isZoomKeyDown = true;
        } else if (context.canceled) {
            isZoomKeyDown = false;
        }
    }
    
    private void Update()
    {
        #region Camera

        // Control camera movement
        if(cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + lookInput.x * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * lookInput.y;
            }
            else
            {
                // Inverted Y
                pitch += mouseSensitivity * lookInput.y;
            }

            // Clamp pitch between lookAngle
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        #region Camera Zoom

        
        
        
        if (enableZoom)
        {

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if(holdToZoom)
            {
                if(isZoomKeyDown)
                {
                    isZoomed = true;
                }
                else
                {
                    isZoomed = false;
                }
            }

            // Lerps camera.fieldOfView to allow for a smooth transistion
            if(isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if(!isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }

        #endregion
        #endregion

        
        if(enableHeadBob)
        {
            HeadBob();
        }
    }

    void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = movementInput;

            // Checks if player is walking and isGrounded
            // Will allow head bob
            if (movementInput.x != 0 || movementInput.z != 0)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            
        }

        #endregion
    }


    private void HeadBob()
    {
        if(isWalking)
        {
            timer += Time.deltaTime * bobSpeed;
            
            // Applies HeadBob movement
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }
}