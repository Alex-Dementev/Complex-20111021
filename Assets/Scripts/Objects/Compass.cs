using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform arrow;
    public bool isDropped = false;
    private float targetY;

    void Update()
    {
        if (!isDropped)
            targetY = CharacterController.PlayerTransform.eulerAngles.y;
        else
            targetY = transform.eulerAngles.y;

        arrow.localRotation = Quaternion.Euler(-90, 0, -targetY);
    }
}