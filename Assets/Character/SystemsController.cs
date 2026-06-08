using UnityEngine;
using UnityEngine.UI;

public class SystemsController : MonoBehaviour
{
    [Header("Oxygen")]
    private float Oxygen;
    public Slider SliderOxygen;
    public Animator AnimatorZeroOxygen;
    private bool ZeroOxygen;
    public float MinusOxygen;
    private bool InAOxygen;

    [Header("Heals")]
    public Slider SliderHeals;
    public Animator AnimatorZeroHeals;
    private bool ZeroHeals;
    public int Heals = 100;
    public float HealsToSlider = 100;

    [Header("Visibility")]
    public Slider SliderVisibility;
    public Animator AnimatorVisibility;
    public float Visibility = 0;
    private float OldVisibility = 0;
    public float VisibilityToSlider = 0;
    private float MinusVisibility;
    private float DelayFastMinusVisibility = 2;

    public Animator AnimatorBlackScreen;
    private bool Death;



    private void OnEnable()
    {
        Systems.Oxygen += SetOxygen;
        Systems.Visibility += ApplyVisibity;
        Systems.Heals += ApplyDamage;
        Systems.InAOxygen += VoidInAOxygen;
    }
    private void OnDisable()
    {
        Systems.Oxygen -= SetOxygen;
        Systems.Visibility -= ApplyVisibity;
        Systems.Heals -= ApplyDamage;
        Systems.InAOxygen -= VoidInAOxygen;
    }
    void Start()
    {
        if(PlayerPrefs.HasKey("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0)))
            Oxygen = PlayerPrefs.GetFloat("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0));
        else
            Oxygen = 35;

        if(PlayerPrefs.HasKey("Heals" + PlayerPrefs.GetInt("WorldIndex", 0)))
        {
            Heals = PlayerPrefs.GetInt("Heals" + PlayerPrefs.GetInt("WorldIndex", 0));
            HealsToSlider = Heals;
        }
        else
            Heals = 100;

        if(PlayerPrefs.HasKey("Visibility" + PlayerPrefs.GetInt("WorldIndex", 0)))
        {
            Visibility = PlayerPrefs.GetFloat("Visibility" + PlayerPrefs.GetInt("WorldIndex", 0), 0);
            DelayFastMinusVisibility = PlayerPrefs.GetFloat("DelayFastMinusVisibility" + PlayerPrefs.GetInt("WorldIndex"), 1);
            MinusVisibility = PlayerPrefs.GetFloat("MinusVisibility" + PlayerPrefs.GetInt("WorldIndex"), 0.065f);
        }
        else
        {
            Visibility = 0;
            DelayFastMinusVisibility = 2;
            MinusVisibility = 0.065f;
        }
    }

    void Update()
    {
        UpdateOxygen();
        UpdateHeals();
        UpdateVisibility();
    }



    private void UpdateOxygen()
    {
        if (InAOxygen)
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
            Oxygen -= MinusOxygen * Time.deltaTime;

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

        SliderOxygen.value = Oxygen;
    }

    private void UpdateHeals()
    {
        Heals = Mathf.Clamp(Heals, 0, 100);

        if(HealsToSlider - Heals >= 50)
            HealsToSlider = Mathf.MoveTowards(HealsToSlider, Heals, Time.deltaTime * 110);
        else
            HealsToSlider = Mathf.MoveTowards(HealsToSlider, Heals, Time.deltaTime * 40);
            
        SliderHeals.value = HealsToSlider;

        if(Heals <= 0f && !Death)
        {
            Death = true;
            AnimatorBlackScreen.Play("Death");
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

    private void UpdateVisibility()
    {
        DelayFastMinusVisibility -= Time.deltaTime;
        
        if(DelayFastMinusVisibility <= 0) MinusVisibility = Mathf.MoveTowards(MinusVisibility, 0.3f, Time.deltaTime * 0.1f);
        else MinusVisibility = Mathf.MoveTowards(MinusVisibility, 0.065f, Time.deltaTime * 0.2f);

        Visibility = Visibility - MinusVisibility * Time.deltaTime;
        Visibility = Mathf.Clamp(Visibility, 0, 100);
        
        VisibilityToSlider = Mathf.MoveTowards(VisibilityToSlider, Visibility, Time.deltaTime);
        SliderVisibility.value = VisibilityToSlider;
    }



    private void ApplyDamage(int vDamage = 0)
    {
        Heals -= vDamage;
        Heals = Mathf.Clamp(Heals, 0, 100);
    }
    private void ApplyVisibity(float vVisibility = 0)
    {
        Visibility += vVisibility;
        Visibility = Mathf.Clamp(Visibility, 0, 100);
        DelayFastMinusVisibility = 2;
    }
    private void SetOxygen(float vOxygen = 0)
    {
        MinusOxygen = vOxygen;
    }
    private void VoidInAOxygen(bool vInAOxygen)
    {
        InAOxygen = vInAOxygen;
    }


    public void Save()
    {
        PlayerPrefs.SetFloat("Oxygen" + PlayerPrefs.GetInt("WorldIndex", 0), Oxygen);
        PlayerPrefs.SetInt("Heals" + PlayerPrefs.GetInt("WorldIndex", 0), Heals);
        PlayerPrefs.SetFloat("MinusVisibility" + PlayerPrefs.GetInt("WorldIndex", 0), MinusVisibility);
        PlayerPrefs.SetFloat("Visibility" + PlayerPrefs.GetInt("WorldIndex", 0), Visibility);
        PlayerPrefs.SetFloat("DelayFastMinusVisibility" + PlayerPrefs.GetInt("WorldIndex", 0), DelayFastMinusVisibility);
    }

    public void Revive()
    {
        Death = false;
        Heals = 65;
        Oxygen = SliderOxygen.maxValue;
    }
}


public static class Systems
{
    public static System.Action<float> Visibility = null;
    public static System.Action<float> Oxygen = null;
    public static System.Action<int> Heals = null;
    public static System.Action<bool> InAOxygen = null;
}
