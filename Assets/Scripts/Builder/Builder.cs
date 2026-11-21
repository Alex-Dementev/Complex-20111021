using UnityEngine;

public class Builder : MonoBehaviour
{
    public Animator Animator;


    void Start()
    {
        DestroyPreviewModels.Destroy += VoidSpawn;

        ChoiceOfBuilding.Instance.Open();
    }

    private void OnDisable()
    {
        ChoiceOfBuilding.Instance.DestroyBuildLCM();

        DestroyPreviewModels.Destroy?.Invoke(false);

        DestroyPreviewModels.Destroy -= VoidSpawn;

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
