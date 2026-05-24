using UnityEngine;
using UnityEngine.InputSystem;

public class Spawn : MonoBehaviour
{
    public int Type = 0;
    public Closet[] Prefab;
    public InputActionAsset inputActions;
    private InputAction SpawnAction;
    public Transform Player;

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
                        if (InventorySlots.Instance.IndexSlots[b] == 2)
                            t++;
                    }

                    if (t >= 3)
                    {
                        int d = 0;

                        for (int i = 0; i < 24; i++)
                        {
                            if (InventorySlots.Instance.IndexSlots[i] == 2)
                            {
                                InventorySlots.Instance.IndexSlots[i] = 0;
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

                        for (int i = 13000; i < CenterSpawnedObjects.Instance.ResourcesID.Length; i++)
                        {
                            if (CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
                            {
                                closet.ID = -12000 + i;
                                CenterSpawnedObjects.Instance.ResourcesID[i] = 1;
                                CenterSpawnedObjects.Instance.ResourcesTypes[i] = 0;
                                CenterSpawnedObjects.Instance.ResourcesItems[i - 12000] = new int[24];
                                break;
                            }
                        }

                        closet.Spawned = true;
                        closet.ClosetType = 0;

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
                        if (InventorySlots.Instance.IndexSlots[b] == 2)
                            t++;
                    }

                    if (t >= 2)
                    {
                        int d = 0;

                        for (int i = 0; i < 24; i++)
                        {
                            if (InventorySlots.Instance.IndexSlots[i] == 2)
                            {
                                InventorySlots.Instance.IndexSlots[i] = 0;
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

                        for (int i = 13000; i < CenterSpawnedObjects.Instance.ResourcesID.Length; i++)
                        {
                            if (CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
                            {
                                closet.ID = -12000 + i;
                                CenterSpawnedObjects.Instance.ResourcesID[i] = 1;
                                CenterSpawnedObjects.Instance.ResourcesTypes[i] = Type;
                                CenterSpawnedObjects.Instance.ResourcesItems[i - 12000] = new int[12];
                                break;
                            }
                        }

                        closet.Spawned = true;
                        closet.ClosetType = 1;

                        closet.TotalSlots = 12;
                        closet.Slots = new int[12];
                    }

                    return;
                }
            }
        }
    }
}
