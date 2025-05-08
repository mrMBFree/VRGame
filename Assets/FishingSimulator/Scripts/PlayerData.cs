using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private static PlayerData instance;

    public int level = 1;
    public int experience = 0;
    public int money = 0;
    public bool hasBetterRod = false; // 🟢 informacja o kupionej lepszej wędce

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        if (experience >= 100 + (level - 1) * 10)
        {
            level++;
            experience = 0;
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("Added " + amount + " money. Total: " + money);
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log("Spent " + amount + " money. Remaining: " + money);
            return true;
        }
        Debug.Log("Not enough money!");
        return false;
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Experience", experience);
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetInt("HasBetterRod", hasBetterRod ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        level = PlayerPrefs.GetInt("Level", 1);
        experience = PlayerPrefs.GetInt("Experience", 0);
        money = PlayerPrefs.GetInt("Money", 0);
        hasBetterRod = PlayerPrefs.GetInt("HasBetterRod", 0) == 1;
    }

    public Vector3 GetSavedPlayerPosition()
    {
        float x = PlayerPrefs.GetFloat("PlayerX", 302f);
        float y = PlayerPrefs.GetFloat("PlayerY", 34f);
        float z = PlayerPrefs.GetFloat("PlayerZ", 530f);
        return new Vector3(x, y, z);
    }

    public void SavePlayerPosition(Vector3 position)
    {
        PlayerPrefs.SetFloat("PlayerX", position.x);
        PlayerPrefs.SetFloat("PlayerY", position.y);
        PlayerPrefs.SetFloat("PlayerZ", position.z);
        PlayerPrefs.Save();
    }
}
