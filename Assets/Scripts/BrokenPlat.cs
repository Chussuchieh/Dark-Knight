using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BrokenPlat : MonoBehaviour
{
    private bool broken;
    public LayerMask player;
    public Transform brokenPoint;
    public Vector2 size;
    private SpriteRenderer thisSprite;
    private Collider2D coll;

    void Awake()
    {
        thisSprite=GetComponent<SpriteRenderer>();
        coll=GetComponent<BoxCollider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!broken)
        {
            if(PlayerController.instance.IsOnGround() &  Physics2D.OverlapBox((Vector2)brokenPoint.position,size,0,player))
            {
                broken=true;
                StartCoroutine(BrokenState());
            }
        }
    }
    IEnumerator BrokenState()
    {
        thisSprite.DOColor(new Color(1,1,1,0),1f);
        yield return new WaitForSeconds(.6f);
        coll.enabled=false;
        yield return new WaitForSeconds(2f);
        coll.enabled=true;
        thisSprite.DOColor(new Color(1,1,1,1),1f);
        yield return new WaitForSeconds(1f);
        broken=false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)brokenPoint.position,size);
    }
}
