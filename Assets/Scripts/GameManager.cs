using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Image image;
    public Image tipsPanel;
    private Text tipsContent;
    private bool gameOvered;
    void Awake()
    {
        instance=this;
        tipsContent=tipsPanel.transform.GetChild(0).gameObject.GetComponent<Text>();
    }
    void Start()
    {
        StartCoroutine(ShowTipsIE("AD(keyboard) / 左摇杆(Xbox) : 移动"));
    }
    void Update()
    {
        if(Input.GetButtonUp("Back"))
        {
            Application.Quit();
        }
    }
    public void  ReStartGame()
    {
        gameOvered=false;
        image.DOColor(new Color(0,0,0,0),1f);
    }
    public void GameOver()
    {
        if(!gameOvered)
        {
            gameOvered=true;
            image.DOColor(new Color(0,0,0,1),2f);
        }
    }
    public void ShowTips(string tipsString)
    {
        StartCoroutine(ShowTipsIE(tipsString));
    }

    IEnumerator ShowTipsIE(string tipsString)
    {
        tipsContent.text=tipsString;
        tipsPanel.DOColor(new Color(0,0,0,0.6f),.5f);
        tipsContent.DOColor(new Color(1,1,1,1),.5f);
        yield return new WaitForSeconds(3.5f);
        tipsPanel.DOColor(new Color(0,0,0,0),.5f);
        tipsContent.DOColor(new Color(1,1,1,0),.5f);
    }

}
