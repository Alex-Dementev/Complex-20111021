using UnityEngine;

public class Builder : MonoBehaviour, IBeforeDestroy
{
    void Start()
    {
        ChoiceOfBuilding.Instance.Open();
    }

    public void BeforeDestroy()
    {
        DestroyPreviewModels.Destroy?.Invoke();

        ChoiceOfBuilding.Instance.Close();
    }

    void Update()
    {
        
    }
}
