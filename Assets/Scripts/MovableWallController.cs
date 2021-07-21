using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MovableWallController : MonoBehaviour
{
    public static MovableWallController instance;
    public bool isSolved;
    private bool solved;
    private float velocityY;
    private Vector3 passedPosition;
    private Vector3 unPassedPosition;
    void Awake()
    {
        instance=this;
        passedPosition=new Vector3(12f,19f,0);
        unPassedPosition=new Vector3(12f,17f,0);
        transform.localPosition=isSolved?passedPosition:unPassedPosition;
    }
    void Update()
    {
        if(!solved)
        {
            if(isSolved)
            {
                solved=true;
                transform.DOLocalMoveY(19f,2f,false);
            }
        }
    }
}
