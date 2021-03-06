using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHander : MonoBehaviour
{
    private SpriteRenderer self;
    private BoxCollider2D coll;
    private float alpha;
    private bool isDead;
    void Awake()
    {
        self=GetComponent<SpriteRenderer>();
        coll=GetComponent<BoxCollider2D>();
        alpha=1;
    }
    void Update()
    {
        self.color = new Color(1,1,1,alpha);
    }
    void FixedUpdate()
    {
        
        if(isDead)
            alpha-=0.02f;
        if(alpha<0)
            Destroy(this);
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag=="Player" & PlayerController.instance.canInteract)
        {
            isDead=true;
            coll.enabled=false;
            MovableWallController.instance.isSolved=true;
        }
    }
}
