using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float CameraSensitivity = 90;
    public float NormalMoveSpeed = 10;
    public float SlowMoveFactor = 0.25f;
    public float FastMoveFactor = 5;

    private float rotationX;
    private float rotationY;
    private float rotationZ;

    private CursorLockMode cursorMode;

    void Start()
    {
        cursorMode = CursorLockMode.Locked;
        SetCursorState();
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * CameraSensitivity, Space.Self);
            transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * CameraSensitivity, Space.Self);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.forward * CameraSensitivity);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(-Vector3.forward * CameraSensitivity);
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            MoveCamera(FastMoveFactor);
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            MoveCamera(SlowMoveFactor);
        }
        else
        {
            MoveCamera(1f);
        }
    }

    void MoveCamera(float moveFactor)
    {
        transform.position += transform.forward * (NormalMoveSpeed * moveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * (NormalMoveSpeed * moveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
    }

    // Apply requested cursor state
    void SetCursorState()
    {
        Cursor.lockState = cursorMode;
        Cursor.visible = (CursorLockMode.Locked != cursorMode);
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = cursorMode = CursorLockMode.None;
        }

        switch (Cursor.lockState)
        {
            case CursorLockMode.None:
                GUILayout.Label("Cursor is normal");
                if (GUILayout.Button("Lock cursor"))
                {
                    cursorMode = CursorLockMode.Locked;
                }

                if (GUILayout.Button("Confine cursor"))
                {
                    cursorMode = CursorLockMode.Confined;
                }

                break;
            case CursorLockMode.Confined:
                GUILayout.Label("Cursor is confined");
                if (GUILayout.Button("Lock cursor"))
                {
                    cursorMode = CursorLockMode.Locked;
                }

                if (GUILayout.Button("Release cursor"))
                {
                    cursorMode = CursorLockMode.None;
                }

                break;
            case CursorLockMode.Locked:
                GUILayout.Label("Cursor is locked");
                if (GUILayout.Button("Unlock cursor"))
                {
                    cursorMode = CursorLockMode.None;
                }

                if (GUILayout.Button("Confine cursor"))
                {
                    cursorMode = CursorLockMode.Confined;
                }

                break;
        }

        GUILayout.EndVertical();

        SetCursorState();
    }

    void OnPreRender()
    {
        GL.wireframe = true;
    }
    void OnPostRender()
    {
        GL.wireframe = false;
    }
}