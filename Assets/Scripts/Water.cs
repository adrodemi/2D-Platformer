using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    float timer = 0f;
    public bool isHit = false;
    float timerHit = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            timer = 0;
        }
        else if (timer >= 0.5f)
            transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<Player>().inWater = true;

            if (isHit)
            {
                timerHit += Time.deltaTime;
                if (timerHit >= 1f)
                {
                    collision.gameObject.GetComponent<Player>().RecountHP(-1);
                    timerHit = 0;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<Player>().inWater = false;
            timerHit = 0;
        }
    }
}