using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    int hp = 0, gg = 0;
    public Sprite[] numbers;
    public Sprite is_hp, no_hp, is_gg, no_gg, is_key, no_key;
    public Image hp_img, gg_img, key_img;
    public Player player;

    public void Start()
    {
        if (PlayerPrefs.GetInt("HP") > 0)
        {
            hp = PlayerPrefs.GetInt("HP");
            hp_img.sprite = is_hp;
            hp_img.transform.GetChild(0).GetComponent<Image>().sprite = numbers[hp];
        }

        if (PlayerPrefs.GetInt("GG") > 0)
        {
            gg = PlayerPrefs.GetInt("GG");
            gg_img.sprite = is_gg;
            gg_img.transform.GetChild(0).GetComponent<Image>().sprite = numbers[gg];
        }
    }

    public void Add_HP()
    {
        hp++;
        hp_img.sprite = is_hp;
        hp_img.transform.GetChild(0).GetComponent<Image>().sprite = numbers[hp];
        if (hp > 9)
            hp = 9;
    }

    public void Add_GG()
    {
        gg++;
        gg_img.sprite = is_gg;
        gg_img.transform.GetChild(0).GetComponent<Image>().sprite = numbers[gg];
        if (gg > 9)
            gg = 9;
    }

    public void Add_Key()
    {
        key_img.sprite = is_key;
    }

    public void Use_HP()
    {
        if (hp > 0)
        {
            hp--;
            player.RecountHP(1);
            hp_img.transform.GetChild(0).GetComponent<Image>().sprite = numbers[hp];
            if (hp == 0)
                hp_img.sprite = no_hp;
        }
    }

    public void Use_GG()
    {
        if (gg > 0)
        {
            gg--;
            player.GreenGem();
            gg_img.transform.GetChild(0).GetComponent<Image>().sprite = numbers[gg];
            if (gg == 0)
                gg_img.sprite = no_gg;
        }
    }

    public void RecountItems()
    {
        PlayerPrefs.SetInt("HP", hp);
        PlayerPrefs.SetInt("GG", gg);
    }
}