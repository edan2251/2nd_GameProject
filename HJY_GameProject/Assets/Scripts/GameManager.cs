using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("게임 상태")]
    public int playerScore = 0;
    public int itemsCollected = 0;

    [Header("UI참조")]
    public Text scoreText;
    public Text itemnCountText;
    public Text gameStatusText;

    public static GameManager Instance;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectItem()
    {
        itemsCollected++;
        Debug.Log($"아이템 수집 ! ( 총: {itemsCollected} 개");
    }

    public void UpdateUI()
    {
        if(scoreText != null)
        {
            scoreText.text = "점수 : " + playerScore;
        }

        if(itemnCountText != null)
        {
            itemnCountText.text = "아이템 : " + itemsCollected + "개";
        }
    }
}
