using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer thisSprite;
    [Header("时间控制参数")]
    public float activeTime;
    public float activeStart;

    [Header("不透明度参数")]
    private float alpha;
    public float alphaSet;
    public float alphaMultiplier;

    void Awake()
    {

    }

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        alpha=alphaSet;

        transform.position = player.position;
        transform.localScale=player.localScale;
        transform.rotation = player.rotation;

        activeStart=Time.time;
    }
    void Update()
    {
        alpha*=alphaMultiplier;

        thisSprite.color = new Color(1,1,1,alpha);
        if(Time.time>=activeTime+activeStart)
        {
            ShadowPool.instance.ReturnPool(gameObject);     
        }
    }
}
