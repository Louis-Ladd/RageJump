using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] Transform playerTrans;
    [SerializeField] Rigidbody2D playerColl;
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] Animator playerAnim;
    [SerializeField] BoxCollider2D boxCollide;
    [SerializeField] ParticleSystem bloodEffect;
    [SerializeField] ParticleSystem jumpEffect;
    [SerializeField] AudioSource playerSound;
    [SerializeField] Camera cam;
    [SerializeField] [Range(0.0f, 10f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 10f)] float speed = 1;
    [SerializeField] CanvasGroup alpha;
    [SerializeField] CanvasGroup alphaText;
    

    private float velocityY = 0.0f;
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;
    private bool isGrounded = true;
    private bool isAlive = true;
    private bool isLoadable = false;
    // Start is called before the first frame update
    void Start()
    {
    }
    void AnimHandle()
    {
        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
        {
            playerAnim.SetBool("isRunning", true);
        }
        else
        {
            playerAnim.SetBool("isRunning", false);
        }
        if (playerColl.velocity.y > 2)
        {
            playerAnim.SetBool("isJumping", true);
        }
        else
        {
            playerAnim.SetBool("isJumping",false);
        }
    }
    void restart()
    {
        alpha.alpha = 0;
        alphaText.alpha = 0;
        playerRenderer.enabled = true;
        bloodEffect.Clear();
        cam.GetComponentInChildren<AudioSource>().Play();
        isAlive = true;
        transform.position = Vector3.zero;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.tag == "Obstacles" || collision.gameObject.tag == "Enemy" && isAlive) // On death
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if(collision.gameObject.GetComponentInChildren<Enemy>().amDead)
                {
                    return;
                }
            }
          
            playerRenderer.enabled = false;
            playerSound.Play();
            cam.GetComponentInChildren<AudioSource>().Stop();
            bloodEffect.Play();
            isAlive = false;
            StartCoroutine(FadeIn(2, 1));
            
            //lifeHandler.LoseLife();
            /*if (collision.transform.position.x - collision.otherCollider.transform.position.x < 0)
            {
                playerColl.AddForce(new Vector2(10, 4.1f), ForceMode2D.Impulse);
                
            }
            else
            {
                playerColl.AddForce(new Vector2(-10, 4.1f), ForceMode2D.Impulse);
            }*/
        }
        if (collision.gameObject.layer == 6)
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isGrounded = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponentInChildren<Enemy>().killMe();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!isAlive) 
        { 
            if(Input.GetKeyDown("space") && isLoadable)
            {
                restart(); //Reload scene
                isLoadable = false;
            }
            return; // don't consider movement if not alive
        }
        AnimHandle();
        if (playerTrans.position.y < -15)//Correct player if he falls.
        {
            playerTrans.position = Vector3.zero;
        }

        if (Input.GetKeyDown("space"))
        {
            if (isGrounded)
            {
                jumpEffect.Play();
                playerColl.AddForce(new Vector2(0, 8), ForceMode2D.Impulse);
                playerColl.AddForce(new Vector2(3 * Input.GetAxis("Horizontal"), 0), ForceMode2D.Impulse);
                playerAnim.SetBool("isJumping", true);
            }
        }
        Vector2 targetDir = new Vector2(Input.GetAxis("Horizontal"), 0);
        targetDir.Normalize();
        if (targetDir[0] < 0)
        {
            playerRenderer.flipX = true;
        }
        else if (targetDir[0] > 0)
        {
            playerRenderer.flipX = false;
        }
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime); //Gives a floaty feel to controls
        Vector2 velocity;
        if (!isGrounded)//Changes weight of controls when in air
        {
            velocity = ((transform.right * currentDir) * speed /2 + Vector2.up * velocityY) * Time.deltaTime;
        }
        else
        {
            velocity = ((transform.right * currentDir) * speed + Vector2.up * velocityY) * Time.deltaTime;
        }
        playerTrans.Translate(velocity);
        
    }
    IEnumerator FadeIn(float duration, float targetAlpha) //Fades in the death bar
    {
        if(isAlive)
        {
            yield break;
        }
        float timer = 0f;
        float currentAlpha = alpha.alpha;
        float targetValue = Mathf.Clamp(targetAlpha, 0, 1);
        while (timer < duration)
        {
            if (isAlive)
            {
                alpha.alpha = 0;
                yield break;
            }
            timer += Time.deltaTime;
            var newAlpha = Mathf.Lerp(currentAlpha, targetValue, timer / duration);
            alpha.alpha = newAlpha;
            yield return null;
        }
        StartCoroutine(FadeInSecondary(2, 1));
    }
    IEnumerator FadeInSecondary(float duration, float targetAlpha) //Fades in the death text
    {
        if (isAlive)
        {

            yield break;
        }
        float timer = 0f;
        float currentAlpha = alphaText.alpha;
        float targetValue = Mathf.Clamp(targetAlpha, 0, 1);
        isLoadable = true;
        while (timer < duration)
        {
            if (isAlive)
            {
                alphaText.alpha = 0;
                yield break;
            }
            timer += Time.deltaTime;
            var newAlpha = Mathf.Lerp(currentAlpha, targetValue, timer / duration);
            alphaText.alpha = newAlpha;
            yield return null;
        }
    }
}
