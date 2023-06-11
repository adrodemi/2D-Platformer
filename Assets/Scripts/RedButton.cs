using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedButton : MonoBehaviour
{
    public GameObject[] blocks;
    public Sprite buttonDown;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MarkBox")
        {
            GetComponent<SpriteRenderer>().sprite = buttonDown;
            GetComponent<CircleCollider2D>().enabled = false;
            foreach (GameObject obj in blocks)
            {
                Destroy(obj);
            }
        }
    }
}