using UnityEngine;
using UnityEngine.InputSystem;

public class RotatorMover : MonoBehaviour
{
    public Camera cam;
    public GameObject rotator;

    Vector2 mousePos;


    void Update()
    {
        Vector3 rotatorScreenPos = cam.WorldToScreenPoint(rotator.transform.position);

        Vector2 direction = mousePos - (Vector2)rotatorScreenPos;

        rotator.transform.up = direction;
    }

    public void OnLook(InputValue input)
    {
        mousePos = input.Get<Vector2>();
    }
}
