using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public Player player;
    public Text coinText, timeText;
    public Image[] hearts;
    public Sprite isLife, nonLife;
    public GameObject pauseScreen, winScreen, looseScreen, inventoryPannel;
    float timer = 0f;
    public TimeWork timeWork;
    public float countdown;
    public SoundEffector soundEffector;
    public AudioSource musicSource, soundSource;

    public void ReloadLvl()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Start()
    {
        musicSource.volume = (float)PlayerPrefs.GetInt("MusicVolume") / 10;
        soundSource.volume = (float)PlayerPrefs.GetInt("SoundVolume") / 10;
        if ((int)timeWork == 2)
            timer = countdown;
    }

    public void Update()
    {
        coinText.text = player.GetCoins().ToString();

        for (int i = 0; i < hearts.Length; i++)
        {
            if (player.GetHP() > i)
                hearts[i].sprite = isLife;
            else
                hearts[i].sprite = nonLife;
        }

        if ((int)timeWork == 1)
        {
            timer += Time.deltaTime;
            timeText.text = timer.ToString("F2").Replace(",", ":");
        }
        else if ((int)timeWork == 2)
        {
            timer -= Time.deltaTime;
            timeText.text = ((int)timer / 60).ToString("D2") + ":" + ((int)timer - (int)timer / 60 * 60).ToString("D2");
            if (timer <= 0)
            {
                Loose();
                timeText.gameObject.SetActive(false);
                timer = 1;
            }
        }
        else
            timeText.gameObject.SetActive(false);
    }

    public void PauseOn()
    {
        Time.timeScale = 0f;
        player.enabled = false;
        pauseScreen.SetActive(true);
    }

    public void PauseOff()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        pauseScreen.SetActive(false);
    }

    public void Win()
    {
        Time.timeScale = 0f;
        player.enabled = false;
        winScreen.SetActive(true);
        soundEffector.PlayWinSound();

        if (!PlayerPrefs.HasKey("Lvl") || PlayerPrefs.GetInt("Lvl") < SceneManager.GetActiveScene().buildIndex)
            PlayerPrefs.SetInt("Lvl", SceneManager.GetActiveScene().buildIndex);

        if (PlayerPrefs.HasKey("Coins"))
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + player.GetCoins());
        else
            PlayerPrefs.SetInt("Coins", player.GetCoins());

        inventoryPannel.SetActive(false);
        GetComponent<Inventory>().RecountItems();
    }

    public void Loose()
    {
        Time.timeScale = 0f;
        player.enabled = false;
        looseScreen.SetActive(true);
        soundEffector.PlayLooseSound();

        inventoryPannel.SetActive(false);
        GetComponent<Inventory>().RecountItems();
    }

    public void MenuLvl()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene(0);
    }

    public void NextLvl()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

public enum TimeWork
{
    None,
    Stopwatch,
    Timer
}