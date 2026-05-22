using UnityEngine;

public class Revive : MonoBehaviour
{
    public CharacterContorller CharacterContorller;
    public SystemsController SystemsController;
    public InventorySlots InventorySlots;


    public void RevivePlayer()
    {
        SystemsController.Revive();
        CharacterContorller.Revive();
    }
    
    public void OnDeath()
    {
        InventorySlots.OnDeath();
    }
}
