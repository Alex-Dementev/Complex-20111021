using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;

public class CenterSpawnedObjects : MonoBehaviour
{
    public int[] ResourcesID = new int[17000];
    public Vector3[] ResourcesPositions = new Vector3[17000];
    public Vector3[] ResourcesRotations = new Vector3[17000]; // Новый массив на 17000 для углов Эйлера (X, Y, Z)
    public int[] ResourcesTypes = new int[17000];
    public int[][] ResourcesItems = new int[5000][];
    public string[] ResourcesNames = new string[5000];
    public static CenterSpawnedObjects Instance;
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
        catch (System.Exception e)
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    public void LoadAllResourcesData()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            ResourcesID = new int[17000];
            ResourcesTypes = new int[17000];
            ResourcesPositions = new Vector3[17000];
            ResourcesRotations = new Vector3[17000]; 
            ResourcesItems = new int[5000][];
            ResourcesNames = new string[5000];
            Debug.Log("CSO: Новые массивы (нет файла сохранений)");
            return; 
        }

        string[] lines = File.ReadAllLines(path);
        
        if (lines.Length < 6) 
        {
            return;
        }

        // --- 1. Парсинг IDs ---
        string[] idSplit = lines[0].Split('|');
        for (int i = 0; i < ResourcesID.Length; i++)
        {
            ResourcesID[i] = (i < idSplit.Length && !string.IsNullOrEmpty(idSplit[i])) ? int.Parse(idSplit[i]) : 0;
        }

        // --- 2. Парсинг Types ---
        string[] typeSplit = lines[1].Split('|');
        for (int i = 0; i < ResourcesTypes.Length; i++)
        {
            ResourcesTypes[i] = (i < typeSplit.Length && !string.IsNullOrEmpty(typeSplit[i])) ? int.Parse(typeSplit[i]) : 0;
        }
        
        // --- 3. Парсинг Позиций ---
        string[] posSplit = lines[2].Split('|');
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
        string[] itemsSplit = lines[3].Split('|');
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
        string[] namesSplit = lines[4].Split('|');
        ResourcesNames = new string[5000]; 

        for (int i = 0; i < ResourcesNames.Length; i++)
        {
            ResourcesNames[i] = (i < namesSplit.Length) ? namesSplit[i] : "";
        }
        // --- 6. Парсинг Ротаций ---
        string[] rotSplit = lines[5].Split('|');
        ResourcesRotations = new Vector3[17000];
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
        
        Load = true;
        
        Debug.Log($"CSO: Данные успешно восстановлены");
    }
}