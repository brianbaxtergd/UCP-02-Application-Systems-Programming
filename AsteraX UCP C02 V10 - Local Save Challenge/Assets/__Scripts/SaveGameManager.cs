using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

static public class SaveGameManager
{
    static private SaveFile saveFile;
    static private string filePath;
    // LOCK, if true, prevents the game from saving. This avoids issues that can happen while loading files.
    static public bool LOCK
    {
        get;
        private set;
    }

    static SaveGameManager()
    {
        LOCK = false;
        filePath = Application.persistentDataPath + "/AsteraX.save";
        saveFile = new SaveFile();
    }

    static public void Save()
    {
        // IF this is LOCKed, don't save.
        if (LOCK)
            return;

        saveFile.stepRecords = AchievementManager.GetStepRecords();
        saveFile.achievements = AchievementManager.GetAchievements();

        string jsonSaveFile = JsonUtility.ToJson(saveFile, true);

        File.WriteAllText(filePath, jsonSaveFile);
    }

    static public void Load()
    {
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            try
            {
                saveFile = JsonUtility.FromJson<SaveFile>(dataAsJson);
            }
            catch
            {
                Debug.LogWarning("SaveGameManager:Load() - SaveFile was malformed.\n" + dataAsJson);
                return;
            }

            LOCK = true;

            // Load the Achievements.
            AchievementManager.LoadDataFromSaveFile(saveFile);

            LOCK = false;
        }
    }

    static public void DeleteSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            saveFile = new SaveFile();
            Debug.Log("SaveGameManager:DeleteSave() - Successfully deleted save file.");
        }
        else
        {
            Debug.LogWarning("SaveGameManager:DeleteSave() - Unable to find and delete save file. This is fine if you've never saved or have just deleted the file.");
        }

        AchievementManager.ClearStepsAndAchievements();
    }

    static internal bool CheckHighScore(int score)
    {
        if (score > saveFile.highScore)
        {
            saveFile.highScore = score;
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class SaveFile
{
    public StepRecord[] stepRecords;
    public Achievement[] achievements;
    public int highScore = 5000;
}
