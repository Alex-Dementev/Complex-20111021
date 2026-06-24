using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;

public class CenterSpawnedObjects : MonoBehaviour
{
    public static CenterSpawnedObjects Instance;


    public const int IDNotSpawnedObjects = 1500;
    public const int IDSpawnedObjects = 12000;
    public const int IDNotSpawnedBuilds = 13000;
    public const int IDSpawnedBuilds = 17000;
    public const int IDNotSpawnedEnemies = 19000;


    public int[] ResourcesID = new int[19000]; //общие ID
    public float[] ResourcesSettings = new float[17000]; //общие настройки
    public Vector3[] ResourcesPositions = new Vector3[19000]; //общие позиции
    public Vector3[] ResourcesRotations = new Vector3[19000]; //общие вращения
    public int[] ResourcesTypes = new int[17000]; //общие типы
    public int[][] ResourcesItems = new int[5000][]; //зубчатый массив построек
    public string[] ResourcesNames = new string[5000]; //названия построек
    public float[][] ResourcesItemsSettings = new float[5000][]; //зубчатый массив построек
    public int[] BiomeSpawned = new int[200]; //биом спавненных предметов
    public int[] ResourcesButteryID = new int[17000];
    public int[][] ResourcesItemsButteryID = new int[5000][]; //зубчатый массив построек
    public Vector3[] EnemyPoint = new Vector3[2000]; //общие вращения


    public static bool Load;


    

    void Start()
    {
        LoadAllResourcesData();
    }

    void Awake()
    {
        Instance = this;
    }

    private string GetSavePath()
    {
        int worldIndex = PlayerPrefs.GetInt("WorldIndex", 0);
        string fileName = $"World_{worldIndex}_Data.txt";
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save()
    {
        string mainPath = GetSavePath();
        string tempPath = mainPath.Replace(".txt", "_TEMP.txt"); 
        
        StringBuilder sb = new StringBuilder();

        // 1. Сохраняем IDs (Строка 0)
        sb.Append(string.Join("|", ResourcesID)).Append('\n');


        sb.Append(string.Join("|", ResourcesSettings)).Append('\n');
        
        // 2. Сохраняем Types (Строка 1)
        sb.Append(string.Join("|", ResourcesTypes)).Append('\n');

        // 3. Сохраняем Позиции (Строка 2)
        for (int i = 0; i < ResourcesPositions.Length; i++)
        {
            Vector3 pos = ResourcesPositions[i];
            sb.Append(pos.x.ToString(CultureInfo.InvariantCulture)).Append(';')
              .Append(pos.y.ToString(CultureInfo.InvariantCulture)).Append(';')
              .Append(pos.z.ToString(CultureInfo.InvariantCulture));
            if (i < ResourcesPositions.Length - 1) sb.Append("|");
        }
        sb.Append('\n'); 

        // 4. Сохраняем Предметы (Строка 3)
        for (int i = 0; i < ResourcesItems.Length; i++)
        {
            if (ResourcesItems[i] != null && ResourcesItems[i].Length > 0)
            {
                sb.Append(string.Join(" ", ResourcesItems[i]));
            }
            if (i < ResourcesItems.Length - 1) sb.Append("|");
        }
        sb.Append('\n'); 

        // 5. Сохраняем Названия (Строка 4)
        for (int i = 0; i < ResourcesNames.Length; i++)
        {
            string name = ResourcesNames[i] ?? "";
            if (name.Contains("|")) name = name.Replace("|", ""); 

            sb.Append(name);
            if (i < ResourcesNames.Length - 1) sb.Append("|");
        }
        sb.Append('\n'); 

        // 6. Сохраняем Ротации (Строка 5)
        for (int i = 0; i < ResourcesRotations.Length; i++)
        {
            Vector3 rot = ResourcesRotations[i];
            sb.Append(rot.x.ToString(CultureInfo.InvariantCulture)).Append(';')
              .Append(rot.y.ToString(CultureInfo.InvariantCulture)).Append(';')
              .Append(rot.z.ToString(CultureInfo.InvariantCulture));
            if (i < ResourcesRotations.Length - 1) sb.Append("|");
        }
        sb.Append('\n'); 

        for (int i = 0; i < ResourcesItemsSettings.Length; i++)
        {
            if (ResourcesItemsSettings[i] != null && ResourcesItemsSettings[i].Length > 0)
            {
                sb.Append(string.Join(" ", ResourcesItemsSettings[i]));
            }
            if (i < ResourcesItemsSettings.Length - 1) sb.Append("|");
        }
        sb.Append('\n'); 

        sb.Append(string.Join("|", BiomeSpawned)).Append('\n');

        sb.Append(string.Join("|", ResourcesButteryID)).Append('\n');

        for (int i = 0; i < ResourcesItemsButteryID.Length; i++)
        {
            if (ResourcesItemsButteryID[i] != null && ResourcesItemsButteryID[i].Length > 0)
            {
                sb.Append(string.Join(" ", ResourcesItemsButteryID[i]));
            }
            if (i < ResourcesItemsButteryID.Length - 1) sb.Append("|");
        }
        sb.Append('\n');

        for (int i = 0; i < EnemyPoint.Length; i++)
        {
            Vector3 pos = EnemyPoint[i];
            sb.Append(pos.x.ToString(CultureInfo.InvariantCulture)).Append(';')
              .Append(pos.y.ToString(CultureInfo.InvariantCulture)).Append(';')
              .Append(pos.z.ToString(CultureInfo.InvariantCulture));
            if (i < EnemyPoint.Length - 1) sb.Append("|");
        }
        sb.Append('\n');  

        try
        {
            File.WriteAllText(tempPath, sb.ToString());

            if (File.Exists(mainPath))
            {
                File.Replace(tempPath, mainPath, null);
            }
            else
            {
                File.Move(tempPath, mainPath);
            }
        }
        catch (System.Exception)
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    public void LoadAllResourcesData()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            ResourcesID = new int[19000];
            ResourcesSettings = new float[17000];
            ResourcesTypes = new int[17000];
            ResourcesPositions = new Vector3[19000];
            ResourcesRotations = new Vector3[19000]; 
            ResourcesItems = new int[5000][];
            ResourcesItemsSettings = new float[5000][];
            ResourcesNames = new string[5000];
            BiomeSpawned = new int[200];
            ResourcesButteryID = new int[17000];
            ResourcesItemsButteryID = new int[5000][];
            EnemyPoint = new Vector3[2000];
            Debug.Log("CSO: Новые массивы (нет файла сохранений)");
            Load = true;
            return; 
        }

        string[] lines = File.ReadAllLines(path);
        
        if (lines.Length < 11) 
            return;

        // --- 1. Парсинг IDs ---
        string[] idSplit = lines[0].Split('|');
        for (int i = 0; i < ResourcesID.Length; i++)
        {
            ResourcesID[i] = (i < idSplit.Length && !string.IsNullOrEmpty(idSplit[i])) ? int.Parse(idSplit[i]) : 0;
        }

        string[] settingsSplit = lines[1].Split('|');
        for (int i = 0; i < ResourcesSettings.Length; i++)
        {
            ResourcesSettings[i] = (i < settingsSplit.Length && !string.IsNullOrEmpty(settingsSplit[i])) ? float.Parse(settingsSplit[i]) : 0;
        }

        // --- 2. Парсинг Types ---
        string[] typeSplit = lines[2].Split('|');
        for (int i = 0; i < ResourcesTypes.Length; i++)
        {
            ResourcesTypes[i] = (i < typeSplit.Length && !string.IsNullOrEmpty(typeSplit[i])) ? int.Parse(typeSplit[i]) : 0;
        }
        
        // --- 3. Парсинг Позиций ---
        string[] posSplit = lines[3].Split('|');
        for (int i = 0; i < ResourcesPositions.Length; i++)
        {
            if (i < posSplit.Length && !string.IsNullOrEmpty(posSplit[i]))
            {
                string[] xyz = posSplit[i].Split(';');
                if (xyz.Length >= 3)
                {
                    ResourcesPositions[i] = new Vector3(
                        float.Parse(xyz[0], CultureInfo.InvariantCulture),
                        float.Parse(xyz[1], CultureInfo.InvariantCulture),
                        float.Parse(xyz[2], CultureInfo.InvariantCulture)
                    );
                }
                else ResourcesPositions[i] = Vector3.zero;
            }
            else ResourcesPositions[i] = Vector3.zero;
        }

        // --- 4. Парсинг Предметов ---
        string[] itemsSplit = lines[4].Split('|');
        ResourcesItems = new int[5000][];

        for (int i = 0; i < ResourcesItems.Length; i++)
        {
            // Если в файле есть данные для этого индекса и они не пустые
            if (i < itemsSplit.Length && !string.IsNullOrWhiteSpace(itemsSplit[i]))
            {
                string trimmedLine = itemsSplit[i].Trim();
                
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    // Нам похуй какой тип шкафа, если в сейве пусто, 
                    // просто даем ему временный пустой массив, Closet сам его расширит под себя!
                    ResourcesItems[i] = null; 
                    continue;
                }

                string[] singleContainerItems = trimmedLine.Split(' ');
                ResourcesItems[i] = new int[singleContainerItems.Length];

                for (int j = 0; j < singleContainerItems.Length; j++)
                {
                    ResourcesItems[i][j] = int.Parse(singleContainerItems[j]);
                }
            }
            else
            {
                ResourcesItems[i] = new int[0];
            }
        }

        // --- 5. Парсинг Названий ---
        string[] namesSplit = lines[5].Split('|');
        ResourcesNames = new string[5000]; 

        for (int i = 0; i < ResourcesNames.Length; i++)
        {
            ResourcesNames[i] = (i < namesSplit.Length) ? namesSplit[i] : "";
        }
        // --- 6. Парсинг Ротаций ---
        string[] rotSplit = lines[6].Split('|');
        ResourcesRotations = new Vector3[19000];
        for (int i = 0; i < ResourcesRotations.Length; i++)
        {
            if (i < rotSplit.Length && !string.IsNullOrEmpty(rotSplit[i]))
            {
                string[] xyz = rotSplit[i].Split(';');
                if (xyz.Length >= 3)
                {
                    ResourcesRotations[i] = new Vector3(
                        float.Parse(xyz[0], CultureInfo.InvariantCulture),
                        float.Parse(xyz[1], CultureInfo.InvariantCulture),
                        float.Parse(xyz[2], CultureInfo.InvariantCulture)
                    );
                }
                else ResourcesRotations[i] = Vector3.zero;
            }
            else ResourcesRotations[i] = Vector3.zero;
        }

        itemsSplit = lines[7].Split('|');
        ResourcesItemsSettings = new float[5000][];

        for (int i = 0; i < ResourcesItemsSettings.Length; i++)
        {
            // Если в файле есть данные для этого индекса и они не пустые
            if (i < itemsSplit.Length && !string.IsNullOrWhiteSpace(itemsSplit[i]))
            {
                string trimmedLine = itemsSplit[i].Trim();
                
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    // Нам похуй какой тип шкафа, если в сейве пусто, 
                    // просто даем ему временный пустой массив, Closet сам его расширит под себя!
                    ResourcesItemsSettings[i] = null; 
                    continue;
                }

                string[] singleContainerItems = trimmedLine.Split(' ');
                ResourcesItemsSettings[i] = new float[singleContainerItems.Length];

                for (int j = 0; j < singleContainerItems.Length; j++)
                {
                    ResourcesItemsSettings[i][j] = float.Parse(singleContainerItems[j]);
                }
            }
            else
            {
                ResourcesItemsSettings[i] = new float[0];
            }
        }

        string[] biomeSplit = lines[8].Split('|');
        for (int i = 0; i < BiomeSpawned.Length; i++)
        {
            BiomeSpawned[i] = (i < biomeSplit.Length && !string.IsNullOrEmpty(biomeSplit[i])) ? int.Parse(biomeSplit[i]) : 0;
        }

        string[] ResourcesButteryIDSplit = lines[9].Split('|');
        for (int i = 0; i < ResourcesButteryID.Length; i++)
        {
            ResourcesButteryID[i] = (i < ResourcesButteryIDSplit.Length && !string.IsNullOrEmpty(ResourcesButteryIDSplit[i])) ? int.Parse(ResourcesButteryIDSplit[i]) : 0;
        }

        string[] itemsButteryIDSplit = lines[10].Split('|');
        ResourcesItemsButteryID = new int[5000][];

        for (int i = 0; i < ResourcesItemsButteryID.Length; i++)
        {
            // Если в файле есть данные для этого индекса и они не пустые
            if (i < itemsButteryIDSplit.Length && !string.IsNullOrWhiteSpace(itemsButteryIDSplit[i]))
            {
                string trimmedLine = itemsButteryIDSplit[i].Trim();
                
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    // Нам похуй какой тип шкафа, если в сейве пусто, 
                    // просто даем ему временный пустой массив, Closet сам его расширит под себя!
                    ResourcesItemsButteryID[i] = null; 
                    continue;
                }

                string[] singleContainerItems = trimmedLine.Split(' ');
                ResourcesItemsButteryID[i] = new int[singleContainerItems.Length];

                for (int j = 0; j < singleContainerItems.Length; j++)
                {
                    ResourcesItemsButteryID[i][j] = int.Parse(singleContainerItems[j]);
                }
            }
            else
            {
                ResourcesItemsButteryID[i] = new int[0];
            }
        }

        string[] pointSplit = lines[11].Split('|');
        for (int i = 0; i < EnemyPoint.Length; i++)
        {
            if (i < pointSplit.Length && !string.IsNullOrEmpty(posSplit[i]))
            {
                string[] xyz = pointSplit[i].Split(';');
                if (xyz.Length >= 3)
                {
                    EnemyPoint[i] = new Vector3(
                        float.Parse(xyz[0], CultureInfo.InvariantCulture),
                        float.Parse(xyz[1], CultureInfo.InvariantCulture),
                        float.Parse(xyz[2], CultureInfo.InvariantCulture)
                    );
                }
                else EnemyPoint[i] = Vector3.zero;
            }
            else EnemyPoint[i] = Vector3.zero;
        }
        
        Load = true;
        
        Debug.Log($"CSO: Данные успешно восстановлены");
    }
}