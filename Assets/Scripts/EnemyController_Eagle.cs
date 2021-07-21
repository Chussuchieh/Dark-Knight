using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EnemyController_Eagle : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D coll;
    private bool isDead;
    [Header("巡逻")]
    public float leftPoint;
    public float rightPoint;
    public float flySpeed;
    private float faceDir;
    public bool faceRight;
    // Start is called before the first frame update
    void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();

        faceDir= -6;
        faceRight=true;
    }
    void Start()
    {
        rb.velocity=(new Vector2(flySpeed*faceDir*-1,0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.localPosition.x > rightPoint && faceRight){
            faceRight=false;
            TurnBack();
        }
        if(transform.localPosition.x < leftPoint && !faceRight){
            faceRight=true;
            TurnBack();
        }
    }
    void TurnBack()
    {
        faceDir=-faceDir;
        transform.localScale=new Vector3(faceDir,6,1);//人物转向
        rb.velocity=(new Vector2(flySpeed*faceDir*-1,0));
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "AttackBox" && !isDead)
        {
            isDead=true;
            this.tag="DeadEnemy";
            rb.velocity=Vector2.zero;
            animator.SetBool("Dead",true);
        }
    }
    public void DestroyAfterDead()
    {
        Destroy(gameObject);
    }
}
