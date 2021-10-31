using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mashroom : MonoBehaviour
{   

    #region 
    [Header("移動速度_")] public float speed;
    [Header("重力")]public float gravity;
    [Header("画面外でも行動するか")]public bool nonVisible;
    [Header("接触判定")]public EnemyCollisionCheck checkCollision;
    #endregion

    #region //プレイベート変数
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private Animator anim = null;
    private ObjectCollision oc = null;
    private BoxCollider2D col = null;
    private PolygonCollider2D pol = null;
    private bool rightTleftF = false;
    private bool isDead = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        oc = GetComponent<ObjectCollision>();
        col = GetComponent<BoxCollider2D>();
        pol = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       if(!oc.playerStepOn)
       { 
            if(sr.isVisible || nonVisible)
            {   
                if(checkCollision.isOn)
                {
                    Debug.Log("aa");
                    rightTleftF = !rightTleftF;
                }
             int xVecter ;
             if(rightTleftF)
                {
                    xVecter = -1;
                     transform.localScale = new Vector3(-4,3,1);
                }
             else
                {  
                    xVecter = 1;
                    transform.localScale = new Vector3(4,3,1);      
                }
             rb.velocity = new Vector2(xVecter * speed, -gravity);
            }
            else
            {
            rb.Sleep();
            }
       } 
       else
       {
           if(!isDead)
           {
               anim.Play("Mashroom_death");
               rb.velocity = new Vector2(0, -gravity);
               isDead = true;
               col.enabled = false;
               pol.enabled = false;
               Destroy(gameObject,3f);

           }
           else
           {
               transform.Rotate(new Vector3(0,0,5));
           }
       }
    }
}
