using UnityEngine;

public class Revive : MonoBehaviour
{
    public CharacterContorller CharacterContorller;
    public SystemsController SystemsController;


    public void RevivePlayer()
    {
        SystemsController.Revive();
        CharacterContorller.Revive();
    }
}
