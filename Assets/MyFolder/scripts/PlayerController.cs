using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

        RayMain();
    }

    #region プレイヤーの移動

    public void Movement()
    {
        //移動方向
        var inputAxis = new Vector3(InputMoveVector.x, 0, InputMoveVector.y);

        //カメラの方向
        var CameraForward = CameraController.transform.forward;

        var MoveVector = (inputAxis.x * CameraController.transform.right) + (inputAxis.z * CameraForward);

        MoveVector.y = 0;

        MoveVector = GetSlopeVector3(MoveVector);

        Velocity = MoveVector.normalized;

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
    private Vector3 GetSlopeVector3(Vector3 vel)
    {
        Ray terrainRay = MakeRay(transform, Vector3.down);
        bool raycast = TryGetRayCast(terrainRay, 10f, out RaycastHit hit);

        if (raycast)
        {
            if (vel.sqrMagnitude < 0.1f) return vel;

            Vector3 normal = hit.normal;
            Vector3 point = hit.point;

            Debug.DrawRay(transform.position, vel, Color.yellow, 0, false);

            Debug.DrawRay(transform.position,normal,Color.blue,0,false);

            Vector3 crossRight = Vector3.Cross(vel, normal);
            Debug.DrawRay(transform.position,crossRight,Color.yellowGreen,0,false);

            Vector3 crossFoward = -Vector3.Cross(crossRight,normal);
            Debug.DrawRay(transform.position, crossFoward, Color.yellowGreen, 0, false);

            float dot = Vector3.Dot(vel, normal);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            Debug.Log($"角度:{angle}");

            vel = crossFoward;
        }
        return vel;
    }
    #endregion

    #region 当たり判定
    private void RayMain()
    {
        Ray ray1 = MakeRay(transform, Vector3.down);
        bool isHit = TryGetRayCast(ray1, 10f, out RaycastHit hit);
        if (isHit)
        {
            Debug.Log($"{ray1}が衝突した");

            //Debug.DrawRay(hit.point,hit.normal,Color.blue,0,false);

            //Debug.DrawRay(transform.position, transform.forward, Color.yellow, 0, false);

            var dot = Vector3.Dot(transform.forward,hit.normal);
            Debug.Log($"内積:{dot}");

            var cross = Vector3.Cross(transform.forward, hit.normal);
            //Debug.DrawRay(transform.position,cross,Color.yellowGreen,0,false);
        }
    }
    private Ray MakeRay(Transform rayPoint, Vector3 direction)
    {
        //Rayを作る
        Ray ray = new Ray(rayPoint.position + Vector3.up * 0.2f, direction);
        //Rayを表示する
        Debug.DrawRay(rayPoint.position + Vector3.up * 0.2f, direction, Color.red);

        return ray;
    }
    private bool TryGetRayCast(Ray ray, float distance, out RaycastHit hit)
    {
        return Physics.Raycast(ray,out hit, distance, LayerMask.GetMask("Default"));
    }
    #endregion
}
