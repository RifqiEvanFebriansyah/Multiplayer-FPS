using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public GameObject PlayerModel;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // SetCursor(); // Set cursor state on start
        PlayerModel.SetActive(false); // Menyembunyikan model pemain pada awalnya
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene loaded event
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from scene loaded event
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset cursor state when a new scene is loaded
        // SetCursorState(scene.name);
    }

    // Function to handle cursor visibility and locking based on the current scene
    void SetCursorState(string sceneName = "")
    {
        if (sceneName == "Main Menu" || sceneName == "Lobby")
        {
            // Allow cursor to be visible and free when in Main Menu or Lobby
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Lock and hide cursor in gameplay scenes
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // Handle movement and rotation (same as before)
        if (SceneManager.GetActiveScene().name == "Main")
        {
            if (!PlayerModel.activeSelf)
            {
                SetPosition();
                PlayerModel.SetActive(true);
            }
        }

        #region Handles Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        #endregion
    }

    // Set random player position for example
    public void SetPosition()
    {
        // Pastikan posisi spawn tidak bentrok dengan objek lain atau atur secara manual
        Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), 0.8f, Random.Range(-15, 7));
        transform.position = spawnPosition;
    }
}
