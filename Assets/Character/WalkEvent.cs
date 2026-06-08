using UnityEngine;

public class WalkEvent : MonoBehaviour
{
    public void Walk()
    {
        AudioManager.Instance.PlayWalkSound();
    }
}
