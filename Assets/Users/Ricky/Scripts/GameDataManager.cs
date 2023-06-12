using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;
    
    private int current_save_slot;
    
    public void SetSaveSlot(int num)
    {
        num = Mathf.Clamp(num, 1, 3);
        current_save_slot = num;
    }
    
    private void Awake()
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
    void Start()
    {
        current_save_slot = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData" + current_save_slot + ".dat"); 
        
        WorldData[] stage_data = StageDataManager.instance.worlds;
        bf.Serialize(file, stage_data);
        
        file.Close();
        Debug.Log("Game data saved!");
    }
    
    public void LoadGame()
    {
        if (CheckForExistingFile())
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveData" + current_save_slot + ".dat", FileMode.Open);
            WorldData[] data = (WorldData[])bf.Deserialize(file);
            file.Close();
            
            StageDataManager.instance.worlds = data;
            
            Debug.Log("Game data loaded!");
        }
        else
        {
            SaveGame();
            Debug.LogError("There is no save data!");
        }
    }
    
    public void ResetGame()
    {
        if (CheckForExistingFile())
        {
            File.Delete(Application.persistentDataPath + "/SaveData" + current_save_slot + ".dat");
            Debug.Log("Data reset complete!");
        }
        else
        {
            Debug.LogError("No save data to delete.");
        }
        
        SaveGame();
    }
    
    public bool CheckForExistingFile()
    {
        return File.Exists(Application.persistentDataPath + "/SaveData" + current_save_slot + ".dat");
    }
}

[Serializable]
class SaveData
{
    public int score;
}