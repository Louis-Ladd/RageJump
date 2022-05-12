using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Enemy : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRender;
    [SerializeField] BoxCollider2D colidSelf;
    [SerializeField] Rigidbody2D physSelf;
    [SerializeField] GameObject player;
    [SerializeField] ParticleSystem bloodEffect;
    [SerializeField] ParticleSystem jumpEffect;
    [SerializeField, Range(0, 50)] float viewRange;
    [SerializeField, Range(500, 5000)] int attackTiming = 1200;

    public bool amDead = false;

    public void killMe()
    {
        amDead = true;
        bloodEffect.Play();
        colidSelf.enabled = false;
        spriteRender.enabled = false;
        StartCoroutine(deathAnim());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (amDead)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool isLeft = player.transform.position.x - transform.position.x < 0? true : false;
        if(distance < viewRange)
        {
            
            spriteRender.flipX = isLeft ? false : true;
            if(Time.frameCount % attackTiming == 0 && Time.frameCount != 0)
            {
                jumpEffect.Play();
                if (isLeft)
                {
                    physSelf.AddForce(new Vector2(-2 * distance /2, 5), ForceMode2D.Impulse);
                }
                else
                {
                    physSelf.AddForce(new Vector2(2 * distance /2, 5), ForceMode2D.Impulse);
                }
            }
        }
    }
    IEnumerator deathAnim() //Waits to destroy so particle system doesn't disapear.
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
        yield return null;
    }
}
