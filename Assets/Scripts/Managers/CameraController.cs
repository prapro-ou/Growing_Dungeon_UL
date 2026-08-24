using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 15f;

    [Header("Transform Limits")]
    [SerializeField] private float minX = 5f;
    [SerializeField] private float maxX = 15f;
    [SerializeField] private float minZ = -17f;
    [SerializeField] private float maxZ = 7f;

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
        if (waveManager != null)
        {
            waveManager.onPhaseChanged += ChangePhase;
            ChangePhase(waveManager.currentPhase);
        }
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.onPhaseChanged -= ChangePhase;
        }
    }

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        Vector2 move = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) move.y += 1;
        if (Keyboard.current.sKey.isPressed) move.y -= 1;
        if (Keyboard.current.aKey.isPressed) move.x -= 1;
        if (Keyboard.current.dKey.isPressed) move.x += 1;

        if (move == Vector2.zero) return;

        Vector3 direction = new Vector3(move.x, 0f, move.y).normalized;
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        transform.position = newPosition;
    }

    private void ZoomCamera()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) < 0.01f) return;

        targetCamera.fieldOfView -= scroll * zoomSpeed;
        targetCamera.fieldOfView = Mathf.Clamp(targetCamera.fieldOfView, minFOV, maxFOV);
    }

    private void ChangePhase(WaveManager.GamePhase phase)
    {
        if (phase == WaveManager.GamePhase.InitialSetup || phase == WaveManager.GamePhase.PrepPhase)
        {
            // 建築フェーズ：真上視点（X=10, Y=40, Z=5）
            minZ = 3f;
            maxZ = 7f;

            Vector3 pos = transform.position;
            pos.x = 10f;
            pos.y = 40f;
            pos.z = Mathf.Clamp(5f, minZ, maxZ); // Z = 5 に指定

            transform.position = pos;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else if (phase == WaveManager.GamePhase.WavePhase)
        {
            // 戦闘フェーズ：斜め視点（X=10, Y=35, Z=-15）
            minZ = -17f;
            maxZ = -13f;

            Vector3 pos = transform.position;
            pos.x = 10f;
            pos.y = 35f;
            pos.z = Mathf.Clamp(-15f, minZ, maxZ); // Z = -15 に指定

            transform.position = pos;
            transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        }
    }
}