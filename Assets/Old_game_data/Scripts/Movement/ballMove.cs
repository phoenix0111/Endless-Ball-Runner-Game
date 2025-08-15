using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ballMove : MonoBehaviour
{
    [Header("Essentials")]
   private GameManager gameManager;
   // private InputActions inputActions;


    [Header("Lane Settings")]
    [SerializeField] float laneDistance = 3f;
    [SerializeField] float laneChangeSpeed = 10f;
    private int currentLane = 1;
   

    [Header("Movement")]
    public float baseSpeed = 5f;
    public float boostSpeed = 10f;
    public float currentSpeed;
    private Vector3 targetPosition;
    private Rigidbody rb;
    private Animator animator;

    [Header("Swipe Settings")]
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private float swipeThreshold = 50f; // pixels
    private bool isSwipe = false;


    [Header("Jump Settings")]
    [SerializeField] float minJumpForce = 6f;
    [SerializeField] float maxJumpForce = 12f;
    [SerializeField] float jumpChargeRate = 5f;
    private bool isHolding = false;
    private bool isChargingJump = false;
    private bool isGrounded = true;
    private float touchStartTime;
    private float tapThreshold = 0.01f;
    private float holdJumpForce;







    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
     //   inputActions = new InputActions();
        gameManager = FindAnyObjectByType<GameManager>();

    }

   

    void Start()
    {
        currentSpeed = baseSpeed;
        UpdateTargetPosition();
        
    }

    void FixedUpdate()
    {
        // Forward movement with gradual speed change
        float targetSpeed = isHolding ? boostSpeed : baseSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 5f * Time.deltaTime);
        rb.MovePosition(rb.position + Vector3.forward * currentSpeed * Time.deltaTime);


        Vector3 moveDirection = targetPosition - transform.position;

        moveDirection.y = 0;
        moveDirection.z = 0;
        rb.MovePosition(rb.position + moveDirection * laneChangeSpeed * Time.deltaTime);         //  Move smoothly towards the target lane (only in X)

        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Jump charge
        if (isChargingJump && isGrounded)
        {
            holdJumpForce += jumpChargeRate * Time.deltaTime;
            holdJumpForce = Mathf.Clamp(holdJumpForce, minJumpForce, maxJumpForce);
        }

        Debug.Log("Current Speed: " + currentSpeed);

        if (animator != null && isGrounded)
        {
            animator.SetBool("jump", false);
        }
    }

   

    void OnTouchStart()
    {
        touchStartTime = Time.time;
        isHolding = true;
        isChargingJump = true;
        holdJumpForce = minJumpForce;

        // Save swipe start position (touch or mouse)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            startTouchPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            startTouchPos = Mouse.current.position.ReadValue();
        }
    }


    void OnTouchEnd()
    {
        float heldTime = Time.time - touchStartTime;

        // Read end touch position (touch or mouse)
        if (Touchscreen.current != null)
        {
            endTouchPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            endTouchPos = Mouse.current.position.ReadValue();
        }

        Vector2 swipeDelta = endTouchPos - startTouchPos;

        // Check if it's a swipe
        if (Mathf.Abs(swipeDelta.x) > swipeThreshold)
        {
           

            isSwipe = true;
        }
        else
        {
            isSwipe = false;
        }

        // Only jump if not a swipe
        if (isGrounded && !isSwipe)
        {
            if (heldTime < tapThreshold)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Jump(minJumpForce);
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Jump(holdJumpForce);
            }
        }

        isHolding = false;
        isChargingJump = false;
        holdJumpForce = 0f;
    }



    void Jump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset vertical velocity
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        // Debug.Log("Jump with force: " + force);
        if(animator != null)
        {
            animator.SetBool("jump", true);
        }
    }

    public void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            UpdateTargetPosition();
        }
    }

    public void MoveRight()
    {
        if (currentLane < 2)
        {
            currentLane++;
            UpdateTargetPosition();
        }
    }

    void UpdateTargetPosition()
    {
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
       

        if(other.gameObject.CompareTag("checkpoint"))
        {
            gameManager.Restartpos = other.transform.position;
        }

    }


}
