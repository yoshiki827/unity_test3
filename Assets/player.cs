using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class player : MonoBehaviour
{
   #region  //�C���X�y�N�^�[�Őݒ肷��
    [Header("移動速度_")] public float speed;
    [Header("重力")]public float gravity;
    [Header("ジャンプ速度")] public float jumpspeed;
    [Header("ジャンプできる高さ")] public float jumpHeight;
    [Header("ジャンプ制限時間")]public float jumpLimitTime;
    [Header("踏みつけ判定の高さの割合")]public float stepOnRate;
    [Header("接地判定")] public groundcheck ground;
    [Header("頭をぶつけた判定")] public groundcheck head;
    [Header("ダッシュの早さ表現")] public AnimationCurve dushCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
    #endregion

   #region //�v���C�x�[�g�ϐ�
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private CapsuleCollider2D capool= null; 
    private bool isGround = false;
    private bool isHead = false;
    private bool isJump = false;
    private bool isRun = false;
    private bool isDown = false;
    private bool isOtherJump = false;
    private float jumpPos = 0.0f;
    private float jumpTime = 0.0f;
    private float otherjumpHeight = 0.0f;
    private float dashtime = 0.0f;
    private float beforekey = 0.0f;
    private string enemyTag = "Enemy";
  #endregion
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capool = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      if(!isDown){
        isGround = ground.IsGround();
        isHead = head.IsGround();

        float xSpeed = GetXSpeed();
        float ySpeed = GetYSpeed();

        SetAnimation();

        rb.velocity = new Vector2(xSpeed, ySpeed);
      }
      else
      {
        rb.velocity = new Vector2(0, -gravity);
      }
    }

       /// <summary>
       /// Y成分で必要な計算をし、速度を返す。
       /// </summary>
       /// <returns>Y軸の速さ</returns>
      private float GetYSpeed()
        {
             float verticalkey = Input.GetAxis("Vertical");
             float yspeed = -gravity;

             if(isOtherJump)
             {

                 bool canHeight = jumpPos + otherjumpHeight > transform.position.y;

                 bool canTime = jumpLimitTime >jumpTime;
                 if( canHeight && canTime && !isHead)
                    {
                       yspeed = jumpspeed;
                        jumpTime += Time.deltaTime;
                    }
                 else 
                    {
                        isOtherJump = false;
                        jumpTime = 0.0f;
                    }
             }   

             else if(isGround)
             {
               if(verticalkey >0)
                 {
                  yspeed = jumpspeed;
                  jumpPos = transform.position.y;
                  isJump = true;
                  jumpTime = 0.0f;
                }
                else
                 {
                   isJump = false;
                 }
              }
             else if(isJump)
             {
                 bool pushUpKey = verticalkey >0;

                 bool canHeight = jumpPos + jumpHeight > transform.position.y;

                 bool canTime = jumpLimitTime >jumpTime;
                 if(pushUpKey && canHeight && canTime && !isHead)
                    {
                       yspeed = jumpspeed;
                        jumpTime += Time.deltaTime;
                    }
                 else 
                    {
                        isJump = false;
                        jumpTime = 0.0f;
                    }
             }   

             if(isJump || isOtherJump)
             {
             yspeed *=jumpCurve.Evaluate(jumpTime); 
             }
             
             return yspeed;
        }

        /// <summary>
        /// X成分で必要な計算をし、速度を返す。
        /// </summary>
        /// <returns>X軸の速さ</returns>
        private float GetXSpeed()
        {
             float horizontalkey = Input.GetAxis("Horizontal");
             float xSpeed = 0.0f;

         if(horizontalkey > 0)
         {
             transform.localScale = new Vector3(1, 1, 1);
             isRun = true;//anim.SetBool("run", true);
             dashtime += Time.deltaTime;
             xSpeed = speed;
         }
         else if (horizontalkey <0)
          {
             transform.localScale = new Vector3(-1, 1, 1);
             isRun = true;//anim.SetBool("run", true);
             dashtime += Time.deltaTime;
             xSpeed = -speed;
          }
         else
         {
             isRun = false;//anim.SetBool("run", false);
             dashtime = 0.0f;
             xSpeed = 0.0f;
         }

         //前回の入力からダッシュの方向を判断して速度を変える。
         if(horizontalkey > 0 && beforekey < 0)
         {
             dashtime = 0.0f;
         }
         else if(horizontalkey < 0 && beforekey > 0)
         {
             dashtime = 0.0f;
         }
           beforekey = horizontalkey;

           xSpeed *= dushCurve.Evaluate(dashtime);
        
         return xSpeed;   
        }    

        /// <summary>
        /// アニメーションを設定する
        /// </summary>
        private void SetAnimation()
        {
         anim.SetBool("jump",isJump　|| isOtherJump);
         anim.SetBool("ground",isGround);
         anim.SetBool("run",isRun);
        }   
        #region //敵と接触判定
        private void OnCollisionEnter2D(Collision2D collision)
        {
          if(collision.collider.tag == enemyTag)
           {
              float stepOnHeight = (capool.size.y * (stepOnRate/100f));

              float judgePos = transform.position.y /*- (capool.size.y /2f) */+ stepOnHeight;
             foreach (ContactPoint2D p in collision.contacts)
             {
               
               
                if(p.point.y < judgePos )
                  {
                    //もう一度跳ねる
                    ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                    if(o!= null)
                    {
                      
                      otherjumpHeight = o.boundHeight;
                      o.playerStepOn = true;
                      jumpPos = transform.position.y;
                      isOtherJump = true;
                      isJump = false;
                      jumpTime = 0.0f;
                    }
                    else
                    {
                      Debug.Log("ObjectCollisionがついてない");
                    }
                  }
                else
                  {
                    anim.Play("HeroKnight_Death");
                    isDown = true;
                    //break;
                  }
                 
              }
               
           
           }
        }
        #endregion
}
