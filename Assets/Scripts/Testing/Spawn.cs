using UnityEngine;
using UnityEngine.InputSystem;

public class Spawn : MonoBehaviour
{
    public int Type = 0;
    public Closet[] Prefab;
    public InputActionAsset inputActions;
    private InputAction SpawnAction;
    public Transform Player;
    public CenterSpawnedObjects CenterSpawnedObjects;
    public InventorySlots InventorySlots;

    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        SpawnAction = playerMap.FindAction("Spawn");
        SpawnAction.Enable();
    }

    void Update()
    {
        if (SpawnAction.triggered)
        {
            switch (Type)
            {
                case 0:
                {
                    int t = 0;

                    for (int b = 0; b < 24; b++)
                    {
                        if (InventorySlots.IndexSlots[b] == 2)
                            t++;
                    }

                    if (t >= 3)
                    {
                        int d = 0;

                        for (int i = 0; i < 24; i++)
                        {
                            if (InventorySlots.IndexSlots[i] == 2)
                            {
                                InventorySlots.IndexSlots[i] = 0;
                                d++;

                                if (d >= 3)
                                    break;
                            }
                        }

                        Closet closet = Instantiate(
                            Prefab[0],
                            Player.position + Player.up * 0.1f + Player.forward * 2f,
                            Player.rotation * Quaternion.Euler(0, 90, 0)
                        );

                        for (int i = 13000; i < CenterSpawnedObjects.ResourcesID.Length; i++)
                        {
                            if (CenterSpawnedObjects.ResourcesID[i] == 0)
                            {
                                closet.ID = -12000 + i;
                                CenterSpawnedObjects.ResourcesID[i] = 1;
                                CenterSpawnedObjects.ResourcesTypes[i] = 0;
                                CenterSpawnedObjects.ResourcesItems[i - 12000] = new int[24];
                                break;
                            }
                        }

                        closet.Spawned = true;
                        closet.ClosetType = 0;
                        closet.CenterSpawnedObjects = CenterSpawnedObjects;
                        closet.InventorySlots = InventorySlots;

                        closet.TotalSlots = 24;
                        closet.Slots = new int[24];
                    }

                    return;
                }

                case 1:
                {
                    int t = 0;

                    for (int b = 0; b < 24; b++)
                    {
                        if (InventorySlots.IndexSlots[b] == 2)
                            t++;
                    }

                    if (t >= 2)
                    {
                        int d = 0;

                        for (int i = 0; i < 24; i++)
                        {
                            if (InventorySlots.IndexSlots[i] == 2)
                            {
                                InventorySlots.IndexSlots[i] = 0;
                                d++;

                                if (d >= 2)
                                    break;
                            }
                        }

                        Closet closet = Instantiate(
                            Prefab[1],
                            Player.position + Player.up * 0.1f + Player.forward * 2f,
                            Player.rotation * Quaternion.Euler(0, 90, 0)
                        );

                        for (int i = 13000; i < CenterSpawnedObjects.ResourcesID.Length; i++)
                        {
                            if (CenterSpawnedObjects.ResourcesID[i] == 0)
                            {
                                closet.ID = -12000 + i;
                                CenterSpawnedObjects.ResourcesID[i] = 1;
                                CenterSpawnedObjects.ResourcesTypes[i] = Type;
                                CenterSpawnedObjects.ResourcesItems[i - 12000] = new int[12];
                                break;
                            }
                        }

                        closet.Spawned = true;
                        closet.ClosetType = 1;
                        closet.CenterSpawnedObjects = CenterSpawnedObjects;
                        closet.InventorySlots = InventorySlots;

                        closet.TotalSlots = 12;
                        closet.Slots = new int[12];
                    }

                    return;
                }
            }
        }
    }
}
