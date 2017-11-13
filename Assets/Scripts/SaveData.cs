using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData {

    public int version = 1;
	public bool[] levels;
	public int[] stars;
	public int[] score;

    // arrays are 0 index so it's 10;
    private int numberOfLevels;
    private static string saveFile = "fotoverite.figments.json";

	public SaveData()
	{
        //First Level is Scene Select Screen
        numberOfLevels = SceneManager.sceneCountInBuildSettings - 1;
        levels = new bool[5];
    	levels[0] = false;
    	for (int i = 1; i < 5; i++)
    	{
    		levels[i] = true;
    	}

		stars = new int[5];
		for (int i = 0; i < numberOfLevels; i++)
		{
			stars[i] = 0;
		}

		score = new int[5];
		for (int i = 0; i < numberOfLevels; i++)
    	{
    		score[i] = 0;
    	}
	

	}
	
    public static SaveData Load() {
		string saveStatePath = Path.Combine(Application.persistentDataPath, saveFile);
        if (File.Exists(saveStatePath))
        {
            string stringifyedSave = File.ReadAllText(saveStatePath);
            return JsonUtility.FromJson<SaveData>(stringifyedSave);
        }
        else {
            return new SaveData();
        }
    }

    public static void Save(SaveData savedData) {
		string saveStatePath = Path.Combine(Application.persistentDataPath, saveFile);
		File.WriteAllText(saveStatePath, JsonUtility.ToJson(savedData, true));
    }


}
