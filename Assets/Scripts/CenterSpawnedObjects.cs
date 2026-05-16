using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;

public class CenterSpawnedObjects : MonoBehaviour
{
    [HideInInspector]public int[] ResourcesID = new int[6000];
    [HideInInspector]public Vector3[] ResourcesPositions = new Vector3[6000];
    [HideInInspector]public int[] ResourcesTypes = new int[6000];

    void Start()
    {
        LoadAllResourcesData();
    }

    private string GetSavePath()
    {
        int worldIndex = PlayerPrefs.GetInt("WorldIndex", 0);
        
        string fileName = $"World_{worldIndex}_Data.txt";
        
        // Application.persistentDataPath на ПК ведет в AppData/LocalLow/НазваниеКомпании/НазваниеИгры
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save()
    {
        string mainPath = GetSavePath();
        string tempPath = mainPath.Replace(".txt", "_TEMP.txt"); 
        
        StringBuilder sb = new StringBuilder();

        //Собираем данные в строку
        sb.AppendLine(string.Join("|", ResourcesID));
        sb.AppendLine(string.Join("|", ResourcesTypes));

        for (int i = 0; i < ResourcesPositions.Length; i++)
        {
            Vector3 pos = ResourcesPositions[i];
            string x = pos.x.ToString(CultureInfo.InvariantCulture);
            string y = pos.y.ToString(CultureInfo.InvariantCulture);
            string z = pos.z.ToString(CultureInfo.InvariantCulture);

            sb.Append(x).Append(';').Append(y).Append(';').Append(z);
            if (i < ResourcesPositions.Length - 1) sb.Append("|");
        }

        try
        {
            //записываем временный файл
            File.WriteAllText(tempPath, sb.ToString());

            // Если старый сейв уже существовал, перезаписываем временным
            if (File.Exists(mainPath))
            {
                File.Replace(tempPath, mainPath, null);
            }
            else
            {
                File.Move(tempPath, mainPath);
            }

            Debug.Log($"[SaveSystem] Атомное сохранение успешно завершено: {mainPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Критическая ошибка при атомном сохранении: {e.Message}");
            
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    public void LoadAllResourcesData()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            ResourcesID = new int[6000];
            ResourcesTypes = new int[6000];
            ResourcesPositions = new Vector3[6000];
            Debug.LogWarning($"[SaveSystem] Файл сохранения {path} не найден. Созданы пустые массивы.");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        
        if (lines.Length < 3)
        {
            Debug.LogError("[SaveSystem] Файл сохранения поврежден! Недостаточно строк данных.");
            return;
        }

        string[] idSplit = lines[0].Split('|');
        for (int i = 0; i < ResourcesID.Length; i++)
        {
            if (i < idSplit.Length && !string.IsNullOrEmpty(idSplit[i]))
                ResourcesID[i] = int.Parse(idSplit[i]);
            else
                ResourcesID[i] = 0;
        }

        string[] typeSplit = lines[1].Split('|');
        for (int i = 0; i < ResourcesTypes.Length; i++)
        {
            if (i < typeSplit.Length && !string.IsNullOrEmpty(typeSplit[i]))
                ResourcesTypes[i] = int.Parse(typeSplit[i]);
            else
                ResourcesTypes[i] = 0;
        }
        
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
                else
                {
                    ResourcesPositions[i] = Vector3.zero;
                }
            }
            else
            {
                ResourcesPositions[i] = Vector3.zero;
            }
        }

        Debug.Log(ResourcesID[10]);
    }
}