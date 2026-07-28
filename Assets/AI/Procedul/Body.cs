using UnityEngine;

public class Body : MonoBehaviour
{
    public Transform[] PawTransforms;
    public Transform BodyTransform;
    public float BodyHeight = 1f;
    private float BodyY;
    public Rigidbody rb;


    
    void Update()
    {
        BodyY = 0f;

        foreach (var paw in PawTransforms)
        {
            BodyY += paw.position.y;   
        }

        BodyY /= PawTransforms.Length;

        BodyTransform.position = Vector3.Lerp(BodyTransform.position, new Vector3(BodyTransform.position.x, BodyY + BodyHeight, BodyTransform.position.z), Time.deltaTime * 3f);
    }
}
