using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class MovementPlayer : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] bool isMobile;
    [SerializeField] Gamemanager gamemanager;
  
    [SerializeField] uiManager uiManager;
    public GameObject Player1Skin;
    public GameObject Player2Skin;
    [SerializeField] CinemachineBasicMultiChannelPerlin Cinecam;
    [SerializeField] Animator animator;


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

    [Header("SpeedPath")]
    float currentspeed;
    [SerializeField] float SpecialspeedMultiplier = 2;
    [SerializeField] float BackToNormalDelay = 5f;
    [SerializeField] bool WillDestroyObject = false;
    private bool isSpeedPath= false;

    void Start()
    {
        int playerIndexChoose = PlayerPrefs.GetInt("CharIndex", 1);

        if (playerIndexChoose == 1) Player1Skin.SetActive(true); Player2Skin.SetActive(false);

        if (playerIndexChoose == 2) Player1Skin.SetActive(false); Player2Skin.SetActive(true);

        rb = GetComponent<Rigidbody>();

        isMobile = Application.platform == RuntimePlatform.Android;

       
    }

    void FixedUpdate()
    {
        movement();
        jump();


        if (!isJumping)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, 1.1f, 10f); // smooth snap
            transform.position = pos;
            rb.useGravity = false;
        }

        else
        {
            rb.useGravity = true;
        }

        // Debug.Log("max speed" + forwardSpeed + "max side speed" + sidewaysSpeed);

  
    }

    void movement()
    {

        // Always move forward
        Vector3 velocity = new Vector3(0, rb.linearVelocity.y, forwardSpeed);

        Vector3 moveInput = isMobile ? HandleMobileInput() : HandlePCMouseInput();

        // If moving left but already at left boundary
        if (transform.position.x <= -maxX && moveInput.x < 0)
            moveInput.x = 0;

        // If moving right but already at right boundary
        if (transform.position.x >= maxX && moveInput.x > 0)
            moveInput.x = 0;

        velocity += moveInput;

        rb.linearVelocity = velocity;

        animator.SetBool("run", true);
        animator.speed = 3;


    }


    Vector3 HandlePCMouseInput()
    {
        Vector3 move = Vector3.zero;

        if (EventSystem.current.IsPointerOverGameObject())
            return move;

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
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return move;
            if (touch.phase == TouchPhase.Began)
            {

                startTouchPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                Vector2 swipeDelta = touch.position - startTouchPos;

                // Check horizontal swipe
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) && Mathf.Abs(swipeDelta.x) > 20f) // threshold
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
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true; // Set jumping state to true
            animator.speed = 1;
            animator.SetBool("jump", true);
            Invoke("landinganim", 1.3f);
        }

        if (Input.GetMouseButton(0) && isMobile && !isJumping)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.speed = 1;
            isJumping = true; // Set jumping state to true
            animator.SetBool("jump", true);
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        isJumping = false;


        if (collision.gameObject.tag == "Obstacle" && !WillDestroyObject)
        {
            Cinecam.AmplitudeGain = 1;
            rb.linearVelocity = Vector3.zero;
            uiManager.OnPlayerDead();

            gameObject.SetActive(false);

            Invoke("CameraSkaeoff", 0.5f);
            uiManager.OnPlayerDead();

        }

        else if (collision.gameObject.tag == "Obstacle" && WillDestroyObject)
        {
           
            Cinecam.AmplitudeGain = 1;


            Invoke("CameraSkaeoff", 0.5f);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {

            gamemanager.IncreaseCoinCount();
            CoinsObjectPool.Instance.ReturnCoin(other.gameObject);

        }

        if (other.gameObject.tag == "checkpoint")
        {

            gamemanager.RespawnPos = other.gameObject.transform.position;
            


        }

        if (other.gameObject.tag == "Speed" && !isSpeedPath)
        {
            WillDestroyObject = true;
           
            currentspeed = forwardSpeed;

            forwardSpeed = forwardSpeed + SpecialspeedMultiplier;

           

            isSpeedPath = true;

        }
    }

    

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Speed")
        {
            isSpeedPath = false;
            Invoke("BackToNormal", BackToNormalDelay);
        }
    }

    void CameraSkaeoff()
    {
        Cinecam.AmplitudeGain = 0;
    }

    void BackToNormal()
    {
        Debug.Log("back to normal");
        forwardSpeed = currentspeed;
        
        WillDestroyObject = false;
    }

    void landinganim()
    {
        animator.SetBool("jump", false);
    }
}
