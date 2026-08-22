using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 15f;

    [Header("Zoom")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float zoomSpeed = 0.2f;
    [SerializeField] private float minFOV = 20f;
    [SerializeField] private float maxFOV = 70f;

    [Header("参照")]
    [SerializeField] private WaveManager waveManager;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Start()
    {
        waveManager.onPhaseChanged += ChangePhase;

        ChangePhase(waveManager.currentPhase);
    }

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        Vector2 move = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            move.y += 1;

        if (Keyboard.current.sKey.isPressed)
            move.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            move.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            move.x += 1;

        Vector3 direction = new Vector3(move.x, 0f, move.y).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void ZoomCamera()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        targetCamera.fieldOfView -= scroll * zoomSpeed;
        targetCamera.fieldOfView = Mathf.Clamp(
            targetCamera.fieldOfView,
            minFOV,
            maxFOV
        );
    }

    private void ChangePhase(WaveManager.GamePhase phase)
    {
        if (phase == WaveManager.GamePhase.PrepPhase)
        {
            transform.position = new Vector3(
                transform.position.x,
                40f,
                0f
            );
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else if (phase == WaveManager.GamePhase.WavePhase)
        {
            transform.position = new Vector3(
                transform.position.x,
                35f,
                -20f
            );
            transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        }
    }
}