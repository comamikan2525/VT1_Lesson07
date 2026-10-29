using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed;

    public CameraController CameraController;

    public PlayerInput playerInput;

    [HideInInspector]public Vector2 InputMoveVector;

    [HideInInspector]public Vector3 Velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveInputEnable();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        //移動方向
        var inputAxis = new Vector3(InputMoveVector.x, 0, InputMoveVector.y);

        //カメラの方向
        var CameraForward = CameraController.transform.forward;

        var MoveVector = (inputAxis.x * CameraController.transform.right) + (inputAxis.z * CameraForward);

        MoveVector.y = 0;

        Velocity = MoveVector;

        Vector3 finalVelocity = MoveSpeed * Velocity * Time.deltaTime;
        transform.Translate(finalVelocity, Space.World);
    }
    public void MoveInputEnable()
    {
        if (playerInput == null)
        {
            Debug.LogError("インプットアクションねえぞ");
            return;
        }

        playerInput.actions["Move"].performed += OnMoveInput;
        playerInput.actions["Move"].canceled += OnMoveCanceld;

        playerInput.actions["Move"].Enable();
    }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        InputMoveVector = value;
    }
    public void OnMoveCanceld(InputAction.CallbackContext context)
    {
        InputMoveVector = Vector2.zero;
    }
}
