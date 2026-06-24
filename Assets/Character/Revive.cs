using UnityEngine;

public class Revive : MonoBehaviour
{
    public CharacterController CharacterController;
    public SystemsController SystemsController;
    public InventorySlots InventorySlots;


    public void RevivePlayer()
    {
        SystemsController.Revive();
        CharacterController.Revive();
    }
    
    public void OnDeath()
    {
        InventorySlots.OnDeath();
    }
}
