using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeController : MonoBehaviour
{
    private InputActions inputActions;
    private Vector2 startPos;
    private Vector2 endPos;
    private float swipeThreshold = 50f;

    public System.Action OnSwipeLeft;
    public System.Action OnSwipeRight;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Gameplay.TouchPress.started += ctx => StartSwipe();
        inputActions.Gameplay.TouchPress.canceled += ctx => EndSwipe();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.TouchPress.started -= ctx => StartSwipe();
        inputActions.Gameplay.TouchPress.canceled -= ctx => EndSwipe();

        inputActions.Disable();
    }

    void StartSwipe()
    {
        startPos = inputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
    }

    void EndSwipe()
    {
        endPos = inputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
        Vector2 delta = endPos - startPos;

        if (Mathf.Abs(delta.x) > swipeThreshold && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
            {
                Debug.Log("Swipe Right");
                OnSwipeRight?.Invoke();
            }
            else
            {
                Debug.Log("Swipe Left");
                OnSwipeLeft?.Invoke();
            }
        }
    }
}
