using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private Rigidbody2D rb;
    private Animator anim; 
    private Collider2D boxColl;
    private Vector3 bornPoint;
    [Header("新技能获取")]
    public bool JumpSkill;
    public bool AttackSkill;
    public bool DashSkill;
    public bool GrabSkill;

    [Header("移动")]
    public bool canMove;
    public float moveSpeed;
    private float velocityX;
    private float faceDir;
    [Header("触底判定")]
    public Transform groundPoint;
    public Vector2 size;
    public Vector2 offSet;
    public Vector2 jumpingOffSet;
    public Vector2 normalOffSet;
    [Header("跳跃")]
    public bool canJump;
    public float jumpForce;
    public float jumpDragMultiplier;
    public float fallMultiplier;
    public bool jumped;
    public bool isJumping;
    public bool isFalling;
    public float fallingVelocity;
    public float jumpCoolDown;
    private float jumpCoolDownTime;
    private Vector2 jumpingBoxColliderOffset;
    private Vector2 jumpingCircleColliderOffset;
    private Vector2 boxColliderOffsetStore;
    private Vector2 circleColliderOffsetStore;
    public LayerMask ground;
    [Header("冲刺")]
    public bool canDash; 
    public bool isDashing;
    private bool wasDashed;
    public float dashForce;
    public float dashCoolDown;
    public float dashCoolDownTime;
    private float getShadowCoolDown;
    private float getShadowCoolDownTime;
    [Header("攻击")]
    public bool canAttack;
    public bool isAttacking;
    public float attackCoolDown;
    public float attackCoolDownTime;
    [Header("死亡/受伤")]
    public int bloodCount;
    public bool isHurt;
    public float hurtForce;
    public float hurtProTime;
    private float hurtProCoolDown;
    public bool isDead;
    public float fallHurtVelocity_deleted;
    public float fallDeadVelocity;
    [Header("交互")]
    public bool interRactMood;
    public bool canInteract;
    [Header("攀墙")]
    public Transform grabPoint;
    public bool canGrab;
    public  bool isGrabbing;
    public LayerMask wall;
    void Awake()
    {
        //组件获取
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxColl=GetComponent<BoxCollider2D>();
        bornPoint=transform.localPosition;
        //初始化
        instance=this;
        faceDir = 1;
        dashCoolDownTime=dashCoolDown;
        attackCoolDownTime = attackCoolDown;
        hurtProCoolDown = hurtProTime;
        getShadowCoolDown=.08f;
        getShadowCoolDownTime=getShadowCoolDown;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        AnimationSwitch();
        ChangeFaceDir();
        FallingHandle();
        //触地判定框移动
        offSet = isJumping||isFalling?jumpingOffSet:normalOffSet;
        
        canInteract = !isHurt & !isDead & !isAttacking & !isGrabbing & !isJumping & !isFalling;
        canMove = !isDashing & !isAttacking & !isHurt & !isDead & !interRactMood & !isGrabbing;
        if(JumpSkill)
            canJump = !isDashing & !isAttacking & !isHurt & !isDead & !interRactMood & !isGrabbing;
        if(DashSkill)
            canDash = !isAttacking & !isHurt & !isDead & !interRactMood;
        if(AttackSkill)
            canAttack = !isJumping & !isFalling & !isDashing & !isHurt & !isDead & !interRactMood &!isGrabbing;
        if(GrabSkill)
            canGrab = Physics2D.OverlapCircle(grabPoint.position,.1f,wall) & !isFalling & !isAttacking & !isHurt & !isDead;
        //掉落死亡
        if (rb.velocity.y < -fallDeadVelocity)
        {
            isDead = true;
            rb.velocity=new Vector2(0,rb.velocity.y);
        }
        if(isDead)
        {
            GameManager.instance.GameOver();
        }
    }
    void FixedUpdate()
    {
        //冷却计时器组
        dashCoolDownTime-=Time.fixedDeltaTime;
        attackCoolDownTime-=Time.fixedDeltaTime;
        hurtProCoolDown-=Time.fixedDeltaTime;
        getShadowCoolDownTime-=Time.fixedDeltaTime;

        Move();        
        
        //冲刺残影
        if(isDashing & !isGrabbing & getShadowCoolDownTime < 0)
        {
            ShadowPool.instance.GetFromPool();
        }

    }
    void Move()
    {     
        #region 左右移动
        if(canMove)
        {
            float horizontalmove=Input.GetAxisRaw("Horizontal");
            if(horizontalmove > 0.2)//防止摇杆回弹
            {
                rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x,moveSpeed*Time.fixedDeltaTime*60,ref velocityX,0.09f), rb.velocity.y);
            }
            if(horizontalmove < -0.2)
            {
                rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x,moveSpeed*Time.fixedDeltaTime*60*-1,ref velocityX,0.09f), rb.velocity.y);
            }
            if(horizontalmove == 0){
                rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x,0,ref velocityX,0.05f), rb.velocity.y);
            }
        }
        #endregion
        #region 跳跃
        if(canJump)
        {
            if(Input.GetAxis("Jump") == 1 && !jumped)
            {
                isJumping=true;
                jumped=true;
                rb.velocity=new Vector2(rb.velocity.x,jumpForce);
            }
            if(IsOnGround() && Input.GetAxis("Jump") == 0){
                isJumping=false;
                jumped=false;
            }
            if(rb.velocity.y>0&&Input.GetAxis("Jump") != 1)//跳跃后松开跳跃键
            {
                rb.velocity += Vector2.up*Physics2D.gravity.y*jumpDragMultiplier*Time.fixedDeltaTime;//减缓上升
            }
        }
        #endregion
        #region 冲刺
            if(canDash)
            {
                if(Input.GetAxisRaw("Dash") ==1 &&  !wasDashed && dashCoolDownTime < 0)
                {
                    wasDashed=true;
                    isDashing=true;
                    EscapeGravity();
                    StopMoving();
                    StartCoroutine(Dash());               
                    //施加冲刺力
                    rb.velocity = new Vector2(faceDir,0)*dashForce; 
                }
                if(IsOnGround() && Input.GetAxisRaw("Dash") == 0)
                {
                    wasDashed=false;
                }
            }
        #endregion
        #region 攀墙
            isGrabbing=false;
            if(canGrab & !IsOnGround())
            {
                if(InputTowardFace())
                {
                    isGrabbing=true;
                    isJumping=false;
                }
                if(isGrabbing)
                {
                    EscapeGravity();
                    StopMoving();
                    if(Input.GetAxisRaw("Jump") ==0)
                    {
                        jumped=false;
                    }
                }
                else//情况比较特殊
                {
                    ResumeGravity();
                }
            }     
        #endregion
    } 
    void Attack()
    {
        if(canAttack)
        {
            if(Input.GetButtonDown("Attack") && attackCoolDownTime < 0)
            {
                isAttacking=true;
                rb.velocity=Vector2.zero;
                anim.SetBool("Attack1",true);
            }
        }
    }
    
    #region 辅助功能函数组
        IEnumerator Dash()
        {
            yield return new WaitForSeconds(0.2f);
            DOVirtual.Float(8f,0,0.1f,RigidbodyDrag);
            isDashing=false;
            ResumeGravity();
            dashCoolDownTime=dashCoolDown;
        }
        public bool IsOnGround()
        {
            return Physics2D.OverlapBox((Vector2)groundPoint.position+offSet,size,0,ground);
        }
        void RigidbodyDrag(float x)
        {
            rb.drag=x;
        }
        void EscapeGravity()
        {
            rb.gravityScale=0;
        }
        void ResumeGravity()
        {
            rb.gravityScale=2;
        }
        void StopMoving()
        {
            rb.velocity=Vector2.zero;
        }
        bool InputTowardFace()
        {
            return faceDir < 0 && Input.GetAxisRaw("Horizontal")<0 || faceDir > 0 && Input.GetAxisRaw("Horizontal")>0;
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(grabPoint.position,.1f);
            Gizmos.DrawWireCube((Vector2)groundPoint.position+offSet,size);
        }
        void ChangeFaceDir()
        {
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal"))>0.2f & canMove)
            {
                faceDir = Input.GetAxisRaw("Horizontal")>0?1:-1;
                transform.localScale = new Vector3(faceDir,1,1);
            }
        }
        void FallingHandle()
        {
            isFalling = rb.velocity.y < fallingVelocity & !IsOnGround();
            if(isFalling)
            {
                isJumping=false;
                rb.velocity += Vector2.up*Physics2D.gravity.y*fallMultiplier*Time.fixedDeltaTime;
            }
        }
    #endregion
    
    #region 动画事件
        private void AnimationSwitch()
        {
            anim.SetBool("Run",IsOnGround() & Input.GetAxisRaw("Horizontal")!=0);
            anim.SetBool("Jump",isJumping);
            anim.SetBool("Fall",isFalling);
            anim.SetBool("Attack1",isAttacking);
            anim.SetBool("Dash",isDashing);
            anim.SetBool("Grab",isGrabbing);
            anim.SetBool("Hurt",isHurt);
            anim.SetBool("Dead",isDead);
            anim.SetBool("NewAbility",interRactMood);
        }
        public void StopAttack()
        {
            attackCoolDownTime=attackCoolDown;
            isAttacking=false;
        }
        public void StopHurt()
        {
            isHurt = false;
        }
        public void StopDead()
        {
            transform.localPosition=bornPoint;
            StopMoving();
            isDead=false;
            GameManager.instance.ReStartGame();
            
        }
        public void StopNewAbility()
        {
            interRactMood=false;
            ResumeGravity();
        }
    #endregion
    public void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Enemy" && hurtProCoolDown < 0)
        {
            if(bloodCount > 1)
            {
                isHurt = true;
                isAttacking=false;
                rb.velocity=Vector2.zero;
                //rb.AddForce(new Vector2(faceDir*hurtForce*(-1),0),ForceMode2D.Force);
                hurtProCoolDown = hurtProTime;
                bloodCount--;
            }
            else
            {
                isDead = true;
                rb.velocity=Vector2.zero;
                hurtProCoolDown = hurtProTime;
            }
            
        }
        if(other.tag=="Interation" & canInteract){

            interRactMood=true;
            StopMoving();
            EscapeGravity();
            switch(other.name)
            {
                case "Jump":
                    JumpSkill=true;
                    break;
                case "Attack":
                    AttackSkill=true;
                    break;
                case "Dash":
                    DashSkill=true;
                    break;
                case "Grab":
                    GrabSkill=true;
                    break;
                default:
                    break;
            }   
        }
        
    }

}
