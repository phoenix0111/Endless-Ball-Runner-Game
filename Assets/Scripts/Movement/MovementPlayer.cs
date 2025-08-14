using UnityEngine;


public class MovementPlayer : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] bool isMobile;

    [Header("Movement Settings")]
    public float forwardSpeed = 15f;   
    public float sidewaysSpeed = 10f;  
    private Rigidbody rb;
    Vector2 startTouchPos;
    bool isSwiping = false;
    [SerializeField] float maxX = 1.6f;
    

    [Header("Jump Settings")]
    [SerializeField] bool isJumping = false;
    public float jumpForce = 10f; 
   

    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
        isMobile = Application.platform == RuntimePlatform.Android ;
    }

    void FixedUpdate()
    {
        movement();
        jump();

        
            


    }

    void movement()
    {

        // Always move forward
        Vector3 velocity = new Vector3(0, rb.linearVelocity.y,forwardSpeed);

        Vector3 moveInput = isMobile ? HandleMobileInput() : HandlePCMouseInput();

        // If moving left but already at left boundary
        if (transform.position.x <= -maxX && moveInput.x < 0)
            moveInput.x = 0;

        // If moving right but already at right boundary
        if (transform.position.x >= maxX && moveInput.x > 0)
            moveInput.x = 0;

        velocity += moveInput;

        rb.linearVelocity = velocity;

       // Vector3 move = Vector3.forward * forwardSpeed * Time.deltaTime;
        //transform.Translate(move, Space.World);

    }


    Vector3 HandlePCMouseInput()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetMouseButton(0)) 
            move += Vector3.left * sidewaysSpeed;

        if (Input.GetMouseButton(1)) 
            move += Vector3.right * sidewaysSpeed;

        return move;
    }

   

    Vector3 HandleMobileInput()
    {
        Vector3 move = Vector3.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                Vector2 swipeDelta = touch.position - startTouchPos;

                // Check horizontal swipe
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) && Mathf.Abs(swipeDelta.x) > 50f) // threshold
                {
                    if (swipeDelta.x > 0)
                        move += Vector3.right * sidewaysSpeed; // Swipe right
                    else
                        move += Vector3.left * sidewaysSpeed; // Swipe left

                    isSwiping = false; // Prevent multiple triggers per swipe
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
            }
        }

        return move;
    }


    void jump()
    {

        if (Input.GetKey(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true; // Set jumping state to true
        }

        if (Input.GetMouseButton(0) && isMobile && !isJumping)
        {
           
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true; // Set jumping state to true
            
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isJumping = false;

      
    }
}
