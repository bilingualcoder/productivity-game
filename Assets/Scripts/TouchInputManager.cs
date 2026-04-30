using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputManager : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 pos = Touchscreen.current.primaryTouch.position.ReadValue();
            CheckTouch(pos);
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            CheckTouch(pos);
        }
    }

    private void CheckTouch(Vector2 screenPosition)
    {
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider == null)
            return;

        Building building = hit.collider.GetComponent<Building>();

        if (building != null)
        {
            building.OpenMission();
        }
    }
}