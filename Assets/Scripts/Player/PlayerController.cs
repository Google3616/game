using UnityEngine;
using UnityEngine.InputSystem;
// By B0N3head 
// All yours, use this script however you see fit, feel free to give credit if you want
[AddComponentMenu("Player Movement and Camera Controller")]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 4f;
    public float maxJumpSpeed = 3f;
    [Space]
    public float height = 2f;
    [Space]
    public float jumpForce = 10f;
    [Space]
    public float sensitivityX;
    public float sensitivityY;
    [Space]

    private Rigidbody rb;

    private float forwardBackward1D;
    private float strafe1D;
    public bool jump;
    private Vector2 look;

    private float jumpTimer;
    private float speed;
    private Transform camTransform;
    void Start()
    {
        speed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        jumpTimer = 0f;
        camTransform = transform.Find("Camera").transform;
        Debug.Log(camTransform.gameObject.name);
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor

    }

    private void FixedUpdate()
    {
        Movement();
        ButtonManage();
    }

    void Update()
    {
//        Debug.Log(1f / Time.deltaTime);
    }
    void Movement()
    {
        rb.AddForce(strafe1D * speed * 10 * transform.right);
        rb.AddForce(forwardBackward1D * speed * 10 * transform.forward);

        rb.AddTorque(0, (Mathf.Sign(look.x) * look.normalized.x * look.x * 100) / sensitivityX, 0);

        rb.velocity = new Vector3(rb.velocity.x * 0.9f, rb.velocity.y, rb.velocity.z * 0.9f);
        rb.angularVelocity *= 0.8f;

        if (!IsGrounded())
        {
            rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -maxJumpSpeed, maxJumpSpeed),rb.velocity.y,Mathf.Clamp(rb.velocity.z, -maxJumpSpeed, maxJumpSpeed));
        }

        float rotationX = camTransform.rotation.eulerAngles.x - ((Mathf.Sign(look.y) * look.normalized.y * look.y) / sensitivityY);
        rotationX = (rotationX > 180f) ? rotationX - 360f : rotationX; // Ensure rotation is within -180 to 180 degrees
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        Quaternion q = Quaternion.Euler(rotationX, camTransform.rotation.eulerAngles.y, camTransform.rotation.eulerAngles.z);
        q.Normalize();
        camTransform.rotation = q;

    }

    void ButtonManage()
    {
        if (jump && jumpTimer < 1f)
        {
            rb.AddForce(0, jumpForce, 0);
            jumpTimer += Time.deltaTime;
            jump = false;
            
        }else
        {
            jumpTimer = 0f;
            jump = false;
        }
    }

    bool IsGrounded()
    {
        return (-0.01f <= rb.velocity.y && rb.velocity.y <= 0.01f) && Physics.Raycast(transform.position, -Vector3.up, 5f);
    }

    #region InputMethods
    public void OnForwardBackward(InputValue inputValue)
    {
        forwardBackward1D = inputValue.Get<float>();
    }

    public void OnStrafe(InputValue inputValue)
    {
        strafe1D = inputValue.Get<float>();
    }

    public void OnJumpStart()
    {
        if (IsGrounded())
        {
            jump = true;
        }
    }
    public void OnJumpFinish()
    {
        jump = false;
    }

    public void OnLook(InputValue inputValue)
    {
        look = inputValue.Get<Vector2>();
        if (look.x > 1f || look.y > 1f)
        {
            look = new Vector2(look.x / 10, look.y / 10);
        }
    }

    #endregion
}