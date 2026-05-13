using UnityEngine;
using UnityEngine.UI;

public class SystemsController : MonoBehaviour
{
    private float Oxygen;
    public Slider SliderOxygen;
    public Slider SliderHeals;
    private CapsuleCollider capsule;
    public Animator AnimatorZeroOxygen;
    public Animator AnimatorZeroHeals;
    public Animator AnimatorBlackScreen;
    private bool ZeroOxygen;
    private bool ZeroHeals;
    private bool Death;
    public int Heals = 100;
    public float MinusOxygen;
    public CharacterContorller CharacterContorller;

    void Start()
    {
        capsule = GetComponent<CapsuleCollider>();
        
        if(PlayerPrefs.HasKey("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0)))
            Oxygen = PlayerPrefs.GetFloat("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0));
        else
            Oxygen = 35;

        if(PlayerPrefs.HasKey("Heals" + PlayerPrefs.GetInt("WorldIndex", 0)))
            Heals = PlayerPrefs.GetInt("Heals" + PlayerPrefs.GetInt("WorldIndex", 0));
        else
            Heals = 100;

        SliderOxygen.maxValue = 35;
    }

    void Update()
    {
        if (InAOxygen())
        {
            Oxygen += 1.2f * Time.deltaTime;

            if (ZeroOxygen)
            {
                AnimatorZeroOxygen.CrossFade("Normal", 0.2f);
                ZeroOxygen = false;
            }
        }
        else
        {
            Oxygen -= 1f * Time.deltaTime;

            if(Oxygen <= 15 && !ZeroOxygen)
            {
                AnimatorZeroOxygen.CrossFade("Zero", 0.2f);
                ZeroOxygen = true;
            }
            else if (ZeroOxygen && Oxygen >= 16)
            {
                AnimatorZeroOxygen.CrossFade("Normal", 0.2f);
                ZeroOxygen = false;
            }

            if(Oxygen <= 0.2f && !Death)
            {
                Death = true;
                AnimatorBlackScreen.Play("Death");
            }
        }

        Oxygen = Mathf.Clamp(Oxygen, 0, SliderOxygen.maxValue);

        SliderHeals.value = Heals;

        SliderOxygen.value = Oxygen;

        if(Heals <= 0f && !Death)
        {
            Death = true;
            AnimatorBlackScreen.Play("Death");
        }

        if(CharacterContorller.Swim)
            MinusOxygen = 1.3f;
        else
        {
            if(CharacterContorller.isSprinting)
                MinusOxygen = 1.52f;
            else
                MinusOxygen = 1f;
        }

        if(Heals <= 30 && !ZeroHeals)
        {
            AnimatorZeroHeals.CrossFade("Zero", 0.2f);
            ZeroHeals = true;
        }
        else if (ZeroHeals && Heals >= 31)
        {
            AnimatorZeroHeals.CrossFade("Normal", 0.2f);
            ZeroHeals = false;
        }
    }

    bool InAOxygen()
    {
        Vector3 point1 = transform.position + Vector3.up * (capsule.height / 2f - capsule.radius);
        Vector3 point2 = transform.position - Vector3.up * (capsule.height / 2f - capsule.radius);

        return Physics.CheckCapsule(point1, point2, capsule.radius, LayerMask.GetMask("Oxygen"));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {
            Heals -= 5;
        }
    }




    public void Save()
    {
        PlayerPrefs.SetFloat("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0), Oxygen);
        PlayerPrefs.SetInt("Heals" + PlayerPrefs.GetInt("WorldIndex", 0), Heals);
    }

    public void Revive()
    {
        Death = false;
        Heals = 65;
        Oxygen = SliderOxygen.maxValue;
    }
}