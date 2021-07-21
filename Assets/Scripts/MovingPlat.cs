using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MovingPlat : MonoBehaviour
{
    List<GameObject> kids = new List<GameObject>();
    void Awake()
    {
        for(int i =0;i<transform.childCount;i++)
        {
            kids.Add(transform.GetChild(i).gameObject);
        }
        StartCoroutine(MovingEnabledCube(0));
    }
    IEnumerator MovingEnabledCube(int index)
    {
        kids[index].GetComponent<SpriteRenderer>().DOColor(new Color(1,1,1,0),1f);
        yield return new WaitForSeconds(1f);
        kids[index].GetComponent<BoxCollider2D>().enabled=false;
        if(index<kids.Count-1)
        {
            kids[index+1].GetComponent<SpriteRenderer>().DOColor(new Color(1,1,1,1),1f);
            kids[index+1].GetComponent<BoxCollider2D>().enabled=true;
        }
        else{
            kids[0].GetComponent<SpriteRenderer>().DOColor(new Color(1,1,1,1),1f);
            kids[0].GetComponent<BoxCollider2D>().enabled=true;
        }
        yield return new WaitForSeconds(1f);
        if(index<kids.Count-1)
            StartCoroutine(MovingEnabledCube(index+1));
        else
            StartCoroutine(MovingEnabledCube(0));

    }
}
