using UnityEngine;

public class Builder : MonoBehaviour, IBeforeDestroy
{
    public Animator Animator;


    void Start()
    {
        DestroyPreviewModels.Destroy += VoidSpawn;

        ChoiceOfBuilding.Instance.Open();
    }

    private void OnDisable(){DestroyPreviewModels.Destroy -= VoidSpawn;}

    public void BeforeDestroy()
    {
        DestroyPreviewModels.Destroy -= VoidSpawn;

        DestroyPreviewModels.Destroy?.Invoke(false);

        ChoiceOfBuilding.Instance.Close();
    }

    public void VoidSpawn(bool Build = false)
    {
        if(Build)
            Animator.Play("Build");
    }
    public void EventSpawn()
    {
        BuildManager.Instance.Build();

        ChoiceOfBuilding.Instance.Open();
    }
}
