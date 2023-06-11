using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button[] lvls;
    public Text coinText;
    public Slider musicSlider, soundSlider;
    public Text musicText, soundText;
    public Text timeForLevel;

    void Start()
    {
        if (PlayerPrefs.HasKey("Lvl"))
            for (int i = 0; i < lvls.Length; i++)
            {
                if (i <= PlayerPrefs.GetInt("Lvl"))
                    lvls[i].interactable = true;
                else
                    lvls[i].interactable = false;
            }

        if (!PlayerPrefs.HasKey("HP"))
            PlayerPrefs.SetInt("HP", 0);
        if (!PlayerPrefs.HasKey("GG"))
            PlayerPrefs.SetInt("GG", 0);

        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetInt("MusicVolume", 4);
        if (!PlayerPrefs.HasKey("SoundVolume"))
            PlayerPrefs.SetInt("SoundVolume", 8);

        musicSlider.value = PlayerPrefs.GetInt("MusicVolume");
        soundSlider.value = PlayerPrefs.GetInt("SoundVolume");
    }

    void Update()
    {
        PlayerPrefs.SetInt("MusicVolume", (int)musicSlider.value);
        PlayerPrefs.SetInt("SoundVolume", (int)soundSlider.value);
        musicText.text = musicSlider.value.ToString();
        soundText.text = soundSlider.value.ToString();

        if (PlayerPrefs.HasKey("Coins"))
            coinText.text = PlayerPrefs.GetInt("Coins").ToString();
        else
            coinText.text = "0";
    }

    public void OpenScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void DeleteKeys()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Buy_HP(int cost)
    {
        if (PlayerPrefs.GetInt("Coins") >= cost)
        {
            PlayerPrefs.SetInt("HP", PlayerPrefs.GetInt("HP") + 1);
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - cost);
        }
    }

    public void Buy_GG(int cost)
    {
        if (PlayerPrefs.GetInt("Coins") >= cost)
        {
            PlayerPrefs.SetInt("GG", PlayerPrefs.GetInt("GG") + 1);
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - cost);
        }
    }
}