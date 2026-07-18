using UnityEngine;

public class SaveSystem
{
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString("jsonData", json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        string json = PlayerPrefs.GetString("jsonData");
        if(json == string.Empty)
        {
            return new SaveData();
        }
        return JsonUtility.FromJson<SaveData>(json);
    }
}
