using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public Vector2 MovementValue { get; private set; }
    public Vector2 LastMoveDirection { get; private set; }
    public bool IsShooting1 { get; private set; }
    public bool IsShooting2 { get; private set; }
    public bool IsReloading { get; private set; }

    private PlayerInputActions inputActions;
    private Camera mainCamera;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        mainCamera = Camera.main;

        // Movimiento
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        // Disparo 1
        inputActions.Player.Shoot1.performed += OnShoot1Performed;
        inputActions.Player.Shoot1.canceled += OnShoot1Canceled;

        // Disparo 2
        inputActions.Player.Shoot2.performed += OnShoot2Performed;
        inputActions.Player.Shoot2.canceled += OnShoot2Canceled;

        // Recarga
        inputActions.Player.Reload.performed += OnReloadPerformed;
        inputActions.Player.Reload.canceled += OnReloadCanceled;
    }

    private void Update()
    {
        UpdateLastMoveDirection();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MovementValue = Vector2.zero;
    }

    private void OnShoot1Performed(InputAction.CallbackContext context)
    {
        IsShooting1 = true;
    }

    private void OnShoot1Canceled(InputAction.CallbackContext context)
    {
        IsShooting1 = false;
    }

    private void OnShoot2Performed(InputAction.CallbackContext context)
    {
        IsShooting2 = true;
    }

    private void OnShoot2Canceled(InputAction.CallbackContext context)
    {
        IsShooting2 = false;
    }

    private void OnReloadPerformed(InputAction.CallbackContext context)
    {
        IsReloading = true;
    }

    private void OnReloadCanceled(InputAction.CallbackContext context)
    {
        IsReloading = false;
    }

    private void UpdateLastMoveDirection()
    {
        if (Mouse.current != null && mainCamera != null)
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            Vector2 directionToMouse = (mouseWorldPos - transform.position);

            if (directionToMouse.sqrMagnitude > 0.01f)
                LastMoveDirection = directionToMouse.normalized;
        }
        else
        {
            if (MovementValue.sqrMagnitude > 0.01f)
                LastMoveDirection = MovementValue.normalized;
        }
    }

    private void OnDestroy()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Shoot1.performed -= OnShoot1Performed;
        inputActions.Player.Shoot1.canceled -= OnShoot1Canceled;

        inputActions.Player.Shoot2.performed -= OnShoot2Performed;
        inputActions.Player.Shoot2.canceled -= OnShoot2Canceled;

        inputActions.Player.Reload.performed -= OnReloadPerformed;
        inputActions.Player.Reload.canceled -= OnReloadCanceled;

        inputActions.Player.Disable();
    }
}
