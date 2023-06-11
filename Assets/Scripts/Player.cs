using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    public float speed, jumpHeight;
    int curHP, maxHP = 3, coins = 0;
    public bool key = false, inWater = false;
    bool isGrounded, isHit = false, canTP = true, isClimbing = false, canHit = true;
    public Main main;
    public Transform groundCheck;
    public GameObject greenGem;
    float hitTimer = 0f;
    public Image playerCountdown;
    float insideTimer = -1f;
    public float insideTimerUp = 30f;
    public Image insideCountdown;
    public Inventory inventory;
    public SoundEffector soundEffector;

    public Joystick joystick;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        curHP = maxHP;
    }

    void Update()
    {
        if (inWater && !isClimbing)
        {
            anim.SetInteger("State", 4);
            isGrounded = true;
            if (joystick.Horizontal >= 0.3f || joystick.Horizontal <= -0.3f)
                Flip();
        }
        else
        {
            CheckGround();
            if (joystick.Horizontal <= 0.3f && joystick.Horizontal >= -0.3f && (isGrounded) && !isClimbing)
                anim.SetInteger("State", 1);

            else
            {
                Flip();
                if (isGrounded && !isClimbing)
                    anim.SetInteger("State", 2);
            }
        }

        if (insideTimer >= 0f)
        {
            insideTimer += Time.deltaTime;
            if (insideTimer >= insideTimerUp)
            {
                insideTimer = 0f;
                RecountHP(-1);
            }
            else
                insideCountdown.fillAmount = 1 - (insideTimer / insideTimerUp);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
            soundEffector.PlayJumpSound();
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
            soundEffector.PlayJumpSound();
        }
    }

    void FixedUpdate()
    {
        if (joystick.Horizontal >= 0.3f)
            rb.velocity = new Vector2(speed, rb.velocity.y);
        else if (joystick.Horizontal <= -0.3f)
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        else
            rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void Flip()
    {
        if (joystick.Horizontal >= 0.3f)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (joystick.Horizontal <= -0.3f)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f);
        isGrounded = colliders.Length > 1;
        if (!isGrounded && !isClimbing)
            anim.SetInteger("State", 3);
    }

    public void RecountHP(int deltaHP)
    {
        curHP += deltaHP;
        if (deltaHP < 0 && canHit)
        {
            StopCoroutine(OnHit());
            canHit = false;
            isHit = true;
            StartCoroutine(OnHit());
        }
        else if (curHP > maxHP)
        {
            curHP = maxHP;
        }
        if (curHP <= 0)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            Invoke("Lose", 1.5f);
        }
    }

    IEnumerator OnHit()
    {
        if (isHit)
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g - 0.1f, GetComponent<SpriteRenderer>().color.b - 0.1f);
        else
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g + 0.1f, GetComponent<SpriteRenderer>().color.b + 0.1f);

        if (GetComponent<SpriteRenderer>().color.g == 1)
        {
            StopCoroutine(OnHit());
            canHit = true;
        }

        if (GetComponent<SpriteRenderer>().color.g <= 0)
            isHit = false;

        yield return new WaitForSeconds(0.02f);
        StartCoroutine(OnHit());
    }

    void Lose()
    {
        main.GetComponent<Main>().Loose();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "RespawnZone")
        {
            Lose();
        }

        if (collision.gameObject.tag == "Key")
        {
            Destroy(collision.gameObject);
            key = true;
            inventory.Add_Key();
        }

        if (collision.gameObject.tag == "Door")
        {
            if (collision.gameObject.GetComponent<Door>().isOpen && canTP)
            {
                collision.gameObject.GetComponent<Door>().Teleport(gameObject);
                canTP = false;
                StartCoroutine(TPwait());
            }
            else if (key)
                collision.gameObject.GetComponent<Door>().Unlock();
        }

        if (collision.gameObject.tag == "Coin")
        {
            soundEffector.PlayCoinSound();
            Destroy(collision.gameObject);
            coins++;
        }

        if (collision.gameObject.tag == "Heart")
        {
            Destroy(collision.gameObject);
            inventory.Add_HP();
        }
        
        if (collision.gameObject.tag == "GreenGem")
        {
            Destroy(collision.gameObject);
            inventory.Add_GG();
        }

        if (collision.gameObject.tag == "TimerButtonStart")
        {
            insideTimer = 0f;
        }

        if (collision.gameObject.tag == "TimerButtonStop")
        {
            insideTimer = -1f;
            insideCountdown.fillAmount = 0f;
        }
    }

    IEnumerator TPwait()
    {
        yield return new WaitForSeconds(2f);
        canTP = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            isClimbing = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if (Input.GetAxis("Vertical") == 0)
                anim.SetInteger("State", 5);
            else
            {
                anim.SetInteger("State", 6);
                transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 0.5f * Time.deltaTime);
            }
        }

        if (collision.gameObject.tag == "Icy")
        {
            if (rb.gravityScale == 1f)
            {
                rb.gravityScale = 6f;
                speed /= 4f;
            }
        }

        if (collision.gameObject.tag == "Lava")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= 3f)
            {
                hitTimer = 0;
                playerCountdown.fillAmount = 1f;
                RecountHP(-1);
            }
            else
                playerCountdown.fillAmount = 1 - (hitTimer / 3f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            isClimbing = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (collision.gameObject.tag == "Icy")
        {
            if (rb.gravityScale == 6f)
            {
                rb.gravityScale = 1f;
                speed *= 4f;
            }
        }

        if (collision.gameObject.tag == "Lava")
        {
            hitTimer = 0f;
            playerCountdown.fillAmount = 0f;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trampoline")
            StartCoroutine(TrampolineAnim(collision.gameObject.GetComponentInParent<Animator>()));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "QuickSand")
        {
            speed = 2f;
            jumpHeight = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "QuickSand")
        {
            speed = 6f;
            jumpHeight = 12f;
        }    
    }

    IEnumerator TrampolineAnim(Animator anim)
    {
        anim.SetBool("Jump", true);
        yield return new WaitForSeconds(0.25f);
        anim.SetBool("Jump", false);
    }

    IEnumerator SpeedBonus()
    {
        speed *= 2;
        greenGem.SetActive(true);
        greenGem.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(9f);
        StartCoroutine(Invis(greenGem.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);
        speed /= 2;
        greenGem.SetActive(false);
    }

    IEnumerator Invis(SpriteRenderer spr, float time)
    {
        spr.color = new Color(1f, 1f, 1f, spr.color.a - time * 2);
        yield return new WaitForSeconds(time);
        if (spr.color.a > 0)
            StartCoroutine(Invis(spr, time));
    }

    public int GetCoins()
    {
        return coins;
    }

    public int GetHP()
    {
        return curHP;
    }

    public void GreenGem()
    {
        StartCoroutine(SpeedBonus()); ;
    }
}