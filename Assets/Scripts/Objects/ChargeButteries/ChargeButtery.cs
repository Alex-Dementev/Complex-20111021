using UnityEngine;

public class ChargeButtery : MonoBehaviour, IInteractable
{
    public int ID;
    public ChargeButteriesBase buttery;

    public void RightClick()
    {
        buttery.RightClick();
    }

    public string GetName()
    {
        if(buttery.AllID.Settings[buttery.ButteryIndex[ID]] != 0)
            return buttery.AllID.SettingsStrings[buttery.ButteryIndex[ID]] + ": " + (int)buttery.ButterySettings[ID] + "/" + buttery.AllID.Settings[buttery.ButteryIndex[ID]];
        else
            return "Пусто";
    }

    public void LeftClick()
    {
        buttery.LeftClickToSlot(ID);
    }
}
