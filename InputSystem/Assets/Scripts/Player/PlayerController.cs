using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Get;

    public PlayerInput playerInput;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private string currentControlScheme;

    void Awake()
    {
        Get = this;
        playerInput = GetComponent<PlayerInput>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        currentControlScheme = playerInput.currentControlScheme;

        InputActionMap actionMap = playerInput.currentActionMap;
        InputAction movement = actionMap.FindAction("Movement");
        movement.performed += Movement_performed;
        InputAction look = actionMap.FindAction("Look");
        //look.performed += Look_performed;
        InputAction attack = actionMap.FindAction("Attack");
        attack.performed += Attack_performed;
        InputAction pause = actionMap.FindAction("TogglePause");
        //pause.performed += Pause_performed;
        InputAction jump = actionMap.FindAction("Jump");
        jump.performed += Jump_performed;
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        //Debug.Log("Movement");

        animator.CrossFade("Run", 0.1f);
    }

    private void Look_performed(InputAction.CallbackContext obj)
    {
        //Debug.Log("Look");
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        //Debug.Log("Attack");

        // TODO: 改成状态机切换
        animator.CrossFade("Attack", 0.1f);
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        //Debug.Log("Pause");
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        //Debug.Log("Jump");

        animator.CrossFade("Jump", 0.1f);
    }
}