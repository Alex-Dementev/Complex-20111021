using UnityEngine;

public class Revive : MonoBehaviour
{
    public PlayerController PlayerController;
    public SystemsController SystemsController;
    public InventorySlots InventorySlots;


    public void RevivePlayer()
    {
        SystemsController.Revive();
        PlayerController.Revive();
    }
    
    public void OnDeath()
    {
        InventorySlots.OnDeath();
    }
}
