using UnityEngine;
using UnityEngine.UI;

public class SystemsController : MonoBehaviour
{
    private float Oxygen;

    public Slider SliderOxygen;

    private CapsuleCollider capsule;

    void Start()
    {
        capsule = GetComponent<CapsuleCollider>();
        
        if(PlayerPrefs.HasKey("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0)))
            Oxygen = PlayerPrefs.GetFloat("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0));

        SliderOxygen.maxValue = 35;
    }

    void Update()
    {
        if (InAOxygen())
        {
            Oxygen += 1.2f * Time.deltaTime;
        }
        else
        {
            Oxygen -= 1f * Time.deltaTime;
        }

        Oxygen = Mathf.Clamp(Oxygen, 0, SliderOxygen.maxValue);

        SliderOxygen.value = Oxygen;
    }

    bool InAOxygen()
    {
        Vector3 point1 = transform.position + Vector3.up * (capsule.height / 2f - capsule.radius);
        Vector3 point2 = transform.position - Vector3.up * (capsule.height / 2f - capsule.radius);

        return Physics.CheckCapsule(point1, point2, capsule.radius, LayerMask.GetMask("Oxygen"));
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0), Oxygen);
    }
}