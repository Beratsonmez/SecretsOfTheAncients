using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.XR;
public class PlayerMovementController : NetworkBehaviour
{
    public CharacterController controller;
    [SerializeField, Header("Network Requirement")]
    public GameObject PlayerModel;
    [SerializeField, Header("Movement Properties")]
    public float speed = 5f;
    [SerializeField, Header("Mouse Properties")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    private float xRotation = 0f;

    private void Start()
    {
        
        PlayerModel.SetActive(true);

        if (!isOwned)
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SceneChanger();
        }
    }
    public void SceneChanger()
    {
            if (PlayerModel.activeSelf == false)
            {
                PlayerModel.SetActive(true);
            }

            if (isOwned)
            {
                MovePlayer();
                RotateCamera();
            }   
    }
    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5,5), 0.8f, Random.Range(-15,7)) ;
    }

    public void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);
    }

    void RotateCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
