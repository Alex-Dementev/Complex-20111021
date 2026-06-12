using UnityEngine;
using UnityEngine.UI;

public class SystemsController : MonoBehaviour
{
    [Header("Oxygen")]
    private float Oxygen;
    public Slider SliderOxygen;
    public Slider SliderBallonOxygen;
    public Animator AnimatorZeroOxygen;
    private bool ZeroOxygen;
    public float MinusOxygen;
    private bool InAOxygen;
    private float CurrentSettingsBallon1;
    private float CurrentSettingsBallon2;
    private int CurrentIndexBallon1;
    private int CurrentIndexBallon2;
    private float Ballon;
    private float Speed = 1.3f;



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
    private int OldVisibility = -1;
    public float VisibilityToSlider = 0;
    private float MinusVisibility;
    private float DelayFastMinusVisibility = 2;
    public static int CurrentVisibilityLevel;


    public Animator AnimatorBlackScreen;
    private bool Death;
    public Material EyesMaterial;
    public AllID AllID;



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



    private void UpdateBallon()
    {
        Ballon = 0;

        if(CurrentIndexBallon1 != 0)
            Ballon += AllID.Settings[CurrentIndexBallon1];

        if(CurrentIndexBallon2 != 0)
            Ballon += AllID.Settings[CurrentIndexBallon2];

        SliderBallonOxygen.maxValue = Ballon;
    }

    private void UpdateOxygen()
    {
        if(CurrentSettingsBallon1 != InventorySlots.Instance.SettingsSlots[53] || CurrentIndexBallon1 != InventorySlots.Instance.IndexSlots[53])
        {
            CurrentSettingsBallon1 = InventorySlots.Instance.SettingsSlots[53];
            CurrentIndexBallon1 = InventorySlots.Instance.IndexSlots[53];
            UpdateBallon();
        }
        if(CurrentSettingsBallon2 != InventorySlots.Instance.SettingsSlots[54] || CurrentIndexBallon2 != InventorySlots.Instance.IndexSlots[54])
        {
            CurrentSettingsBallon2 = InventorySlots.Instance.SettingsSlots[54];
            CurrentIndexBallon2 = InventorySlots.Instance.IndexSlots[54];
            UpdateBallon();
        }

        if (InAOxygen)
        {
            Oxygen += Speed * Time.deltaTime;

            if (ZeroOxygen)
            {
                AnimatorZeroOxygen.CrossFade("Normal", 0.2f);
                ZeroOxygen = false;
            }

            SliderBallonOxygen.value = CurrentSettingsBallon1 + CurrentSettingsBallon2;
        }
        else
        {
            if(CurrentSettingsBallon2 + CurrentSettingsBallon1 >= 1f)
            {
                float Amount = 0;

                if(CurrentIndexBallon2 != 0 && CurrentSettingsBallon2 >= 0.5f)
                {
                    Amount = Mathf.Min(Speed * Time.deltaTime, 35 - Oxygen, CurrentSettingsBallon2);
                    CurrentSettingsBallon2 -= Amount;
                    CurrentSettingsBallon2 -= MinusOxygen * Time.deltaTime;
                    InventorySlots.Instance.SettingsSlots[54] = CurrentSettingsBallon2;
                }
                else if(CurrentIndexBallon1 != 0 && CurrentSettingsBallon1 >= 0.5f)
                {
                    Amount = Mathf.Min(Speed * Time.deltaTime, 35 - Oxygen, CurrentSettingsBallon1);
                    CurrentSettingsBallon1 -= Amount;
                    CurrentSettingsBallon1 -= MinusOxygen * Time.deltaTime;
                    InventorySlots.Instance.SettingsSlots[53] = CurrentSettingsBallon1;
                }

                Oxygen += Amount;
                SliderBallonOxygen.value = CurrentSettingsBallon1 + CurrentSettingsBallon2;

                if (ZeroOxygen && Oxygen >= 16)
                {
                    AnimatorZeroOxygen.CrossFade("Normal", 0.2f);
                    ZeroOxygen = false;
                }
            }
            else
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
        
        if(InAOxygen)
        {
            if(DelayFastMinusVisibility <= 0) MinusVisibility = Mathf.MoveTowards(MinusVisibility, 0.6f, Time.deltaTime * 0.1f);
            else MinusVisibility = Mathf.MoveTowards(MinusVisibility, 0.1f, Time.deltaTime * 0.2f);
        }
        else
        {
            if(DelayFastMinusVisibility <= 0) MinusVisibility = Mathf.MoveTowards(MinusVisibility, 0.25f, Time.deltaTime * 0.1f);
            else MinusVisibility = Mathf.MoveTowards(MinusVisibility, 0.065f, Time.deltaTime * 0.2f);
        }

        Visibility = Visibility - MinusVisibility * Time.deltaTime;
        Visibility = Mathf.Clamp(Visibility, 0, 100);
        
        VisibilityToSlider = Mathf.MoveTowards(VisibilityToSlider, Visibility, Time.deltaTime * 2);
        SliderVisibility.value = VisibilityToSlider;

        if(VisibilityToSlider >= 90)
        {
            if(OldVisibility == 3)
                return;

            OldVisibility = 3;
            AnimatorVisibility.CrossFade("3", 0.1f);
            Systems.VisibilityLevel?.Invoke(3);
            CurrentVisibilityLevel = 3;
            EyesMaterial.SetColor("_BaseColor", Color.red);
        }
        else if(VisibilityToSlider >= 60)
        {
            if(OldVisibility == 2)
                return;

            OldVisibility = 2;
            AnimatorVisibility.CrossFade("2", 0.1f);
            Systems.VisibilityLevel?.Invoke(2);
            CurrentVisibilityLevel = 2;
            EyesMaterial.SetColor("_BaseColor", Color.red);
        }
        else if(VisibilityToSlider >= 30)
        {
            if(OldVisibility == 1)
                return;

            OldVisibility = 1;
            AnimatorVisibility.CrossFade("1", 0.1f);
            Systems.VisibilityLevel?.Invoke(1);
            CurrentVisibilityLevel = 1;
            EyesMaterial.SetColor("_BaseColor", Color.red);
        }
        else if(VisibilityToSlider <= 30)
        {
            if(OldVisibility == 0)
                return;

            OldVisibility = 0;
            AnimatorVisibility.CrossFade("0", 0.1f);
            Systems.VisibilityLevel?.Invoke(0);
            CurrentVisibilityLevel = 0;
            EyesMaterial.SetColor("_BaseColor", Color.white);
        }
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

        if(vVisibility <= 0.05 && DelayFastMinusVisibility <= 0.5f)
            DelayFastMinusVisibility = 0.5f;
        else if(vVisibility >= 0.06)
            DelayFastMinusVisibility = 2f;
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
    public static System.Action<float> Visibility;
    public static System.Action<float> Oxygen;
    public static System.Action<int> Heals;
    public static System.Action<bool> InAOxygen;

    public static System.Action<int> VisibilityLevel;
}
